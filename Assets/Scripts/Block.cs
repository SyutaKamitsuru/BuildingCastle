using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("ブロック情報")]
    public ItemData blockItemData; // このブロックに対応するItemData
    
    [Header("採掘設定")]
    [SerializeField] private float hardness = 1f; // 硬さ（採掘にかかる時間）
    
    private float currentHealth;
    
    void Start()
    {
        currentHealth = hardness;
        
        // 地面として配置されたブロックはRigidbody2Dを削除
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Destroy(rb);
        }
    }
    
    // ブロックを破壊
    public void Mine(float damage = 1f)
    {
        currentHealth -= damage;
        
        // ダメージエフェクト（オプション）
        // TODO: ヒビのアニメーション等
        
        if (currentHealth <= 0)
        {
            Break();
        }
    }
    
    // ブロックが破壊された時
    void Break()
    {
        // アイテムをドロップ
        if (blockItemData != null)
        {
            DroppedItem.Create(blockItemData, 1, transform.position);
        }
        
        // ブロックを削除
        Destroy(gameObject);
    }
    
    // マウスクリックで掘る（仮実装）
    void OnMouseDown()
    {
        Mine(1f);
    }
}