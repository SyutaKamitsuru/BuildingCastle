using UnityEngine;

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
    
    [Header("ツール設定")]
    public int miningPower = 0; // 採掘力（ツルハシなど用）
}

public enum ItemType
{
    Block,      // 設置可能なブロック（土、石など）
    Tool,       // ツール（ツルハシ、斧など）
    Material,   // クラフト素材
    Consumable  // 消費アイテム
}