using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("アイテムデータ")]
    public ItemData itemData;
    public int quantity = 1;
    
    [Header("拾う設定")]
    [SerializeField] private float pickupRange = 3f; // 拾える範囲を広げる
    [SerializeField] private float magnetSpeed = 5f; // プレイヤーに引き寄せられる速度
    
    [Header("見た目")]
    private SpriteRenderer spriteRenderer;
    
    private Transform player;
    private bool canPickup = false;
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // アイテムのアイコンを表示
        if (itemData != null && itemData.icon != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
        else
        {
            Debug.LogWarning("DroppedItem: itemData または icon が null です");
        }
        
        // プレイヤーを検索
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("DroppedItem: プレイヤーを発見");
        }
        else
        {
            Debug.LogError("DroppedItem: プレイヤーが見つかりません（Tagが'Player'か確認）");
        }
        
        // 少し待ってから拾えるようにする（掘った直後に即拾わないため）
        Invoke("EnablePickup", 0.5f);
    }
    
    void EnablePickup()
    {
        canPickup = true;
        Debug.Log($"DroppedItem: {itemData.itemName} が拾えるようになりました");
    }
    
    void Update()
    {
        if (!canPickup)
        {
            return;
        }
        
        if (player == null)
        {
            Debug.LogWarning("DroppedItem: playerがnullです");
            return;
        }
        
        float distance = Vector2.Distance(transform.position, player.position);
        
        // デバッグ: 距離を常に表示（重いので後で削除）
        Debug.Log($"DroppedItem: プレイヤーとの距離 = {distance:F2}, 拾える距離 = {pickupRange}");
        
        // プレイヤーが範囲内にいる場合
        if (distance <= pickupRange)
        {
            Debug.Log($"DroppedItem: プレイヤーが範囲内！引き寄せます");
            
            // プレイヤーに引き寄せられる
            transform.position = Vector2.MoveTowards(
                transform.position, 
                player.position, 
                magnetSpeed * Time.deltaTime
            );
            
            // プレイヤーに触れたら拾う
            if (distance <= 0.5f)
            {
                Debug.Log("DroppedItem: 拾う処理を実行");
                TryPickup();
            }
        }
    }
    
    void TryPickup()
    {
        Inventory inventory = player.GetComponent<Inventory>();
        
        if (inventory == null)
        {
            Debug.LogError("DroppedItem: プレイヤーに Inventory コンポーネントがありません");
            return;
        }
        
        Debug.Log($"DroppedItem: {itemData.itemName} x{quantity} をインベントリに追加を試みます");
        
        // インベントリに追加を試みる
        bool success = inventory.AddItem(itemData, quantity);
        
        if (success)
        {
            // 拾えた場合は削除
            Debug.Log("DroppedItem: アイテムを拾いました！削除します");
            Destroy(gameObject);
        }
        else
        {
            // インベントリが満杯の場合
            Debug.Log("DroppedItem: インベントリが満杯です");
        }
    }
    
    // 初期化用の静的メソッド
    public static DroppedItem Create(ItemData itemData, int quantity, Vector3 position)
    {
        // プレハブまたは基本オブジェクトを生成
        GameObject obj = new GameObject($"DroppedItem_{itemData.itemName}");
        obj.transform.position = position;
        obj.tag = "Item"; // Tagを設定しておくと便利
        obj.layer = LayerMask.NameToLayer("Item"); // Itemレイヤーに設定
        
        // 必要なコンポーネントを追加
        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = itemData.icon;
        sr.sortingOrder = 5; // プレイヤーやブロックより前面に
        
        // 小さく表示
        obj.transform.localScale = Vector3.one * 0.5f;
        
        // 物理演算用
        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.radius = 0.3f;
        // Triggerは外す（物理衝突を有効にする）
        
        // DroppedItemスクリプトを追加
        DroppedItem droppedItem = obj.AddComponent<DroppedItem>();
        droppedItem.itemData = itemData;
        droppedItem.quantity = quantity;
        
        return droppedItem;
    }
    
    // デバッグ用：拾える範囲を可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}