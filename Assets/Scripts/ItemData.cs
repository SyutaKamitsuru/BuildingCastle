using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("基本情報")]
    public int itemId = 0;
    public string itemName = "アイテム名";
    public Sprite icon;
    [TextArea(3, 5)]
    public string description = "アイテムの説明";
    
    [Header("アイテムタイプ")]
    public ItemType itemType = ItemType.Block;
    
    [Header("スタック設定")]
    public int maxStack = 999;
    
    [Header("ブロック設定")]
    public bool isPlaceable = true;
    public GameObject blockPrefab; // 設置時に生成するブロックのプレハブ
    
    [Header("採掘設定")]
    public float hardness = 1f; // ブロックの硬さ

    [Header("ドロップ設定")]
    public List<DropItemEntry> dropItems = new List<DropItemEntry>();
    
    [Header("ツール設定")]
    public int miningPower = 0; // 採掘力（ツルハシなど用）

    // ドロップアイテムを取得
    public List<ItemDrop> GetDrops()
    {
        List<ItemDrop> drops = new List<ItemDrop>();

        foreach (DropItemEntry entry in dropItems)
        {
            if (entry.item != null && entry.ShouldDrop())
            {
                int quantity = entry.GetDropQuantity();
                drops.Add(new ItemDrop(entry.item, quantity));

                 Debug.Log($"ドロップ: {entry.item.itemName} x{quantity} (確率: {entry.dropChance}%)");
            }
        }
        return drops;
    }
}

// アイテムタイプ
public enum ItemType
{
    Block,      // 設置可能なブロック（土、石など）
    Tool,       // ツール（ツルハシ、斧など）
    Material,   // クラフト素材
    Consumable  // 消費アイテム
}

// ドロップアイテムのエントリ
[System.Serializable]
public class DropItemEntry
{
    [Header("ドロップアイテム")]
    public ItemData item;

    [Header("ドロップ数")]
    public int minQuantity = 1;
    public int maxQuantity = 1;

    [Header("ドロップ確率")]
    [Range(0f, 100f)]
    public float dropChance = 100f;     // パーセント

    // ドロップするかどうかの判定
    public bool ShouldDrop()
    {
        return Random.Range(0f, 100f) <= dropChance;
    }

    // ドロップする個数を取得
    public int GetDropQuantity()
    {
        return Random.Range(minQuantity, maxQuantity + 1);
    }
}

// ドロップされたアイテムの情報
[System.Serializable]
public class ItemDrop
{
    public ItemData item;
    public int quantity;

    public ItemDrop(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}