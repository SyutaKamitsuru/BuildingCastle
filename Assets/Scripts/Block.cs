using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("ブロック情報")]
    public ItemData blockItemData; // このブロックに対応するItemData
    
    private float currentHealth;
    
    void Start()
    {
        // ブロックの硬さを設定
        if (blockItemData != null)
        {
            currentHealth = blockItemData.hardness;
        }
        else
        {
            currentHealth = 1f; // デフォルト値
        }
        
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
        if (blockItemData != null)
        {
            // ドロップテーブルからアイテムを生成
            var drops = blockItemData.GetDrops();
            
            if (drops.Count > 0)
            {
                foreach (var drop in drops)
                {
                    DroppedItem.Create(drop.item, drop.quantity, transform.position);
                }
            }
            else
            {
                // ドロップ設定がない場合は自分自身をドロップ（後方互換）
                DroppedItem.Create(blockItemData, 1, transform.position);
            }
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