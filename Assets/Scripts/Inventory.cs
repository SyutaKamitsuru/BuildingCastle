using UnityEngine;
using System.Collections.Generic;

public class Inventory : MonoBehaviour
{
    [Header("インベントリ設定")]
    [SerializeField] private int slotCount = 40; // テラリアライクに40スロット
    
    public InventorySlot[] slots;
    
    void Awake()
    {
        // スロットを初期化
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            slots[i] = new InventorySlot();
        }
    }
    
    // アイテムを追加
    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        
        int remaining = quantity;
        
        // 既存のスロットに追加を試みる
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty() && slots[i].item == item)
            {
                remaining = slots[i].AddItem(item, remaining);
                if (remaining <= 0)
                {
                    Debug.Log($"{item.itemName} x{quantity} を追加しました");
                    return true;
                }
            }
        }
        
        // 空のスロットに追加
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty())
            {
                remaining = slots[i].AddItem(item, remaining);
                if (remaining <= 0)
                {
                    Debug.Log($"{item.itemName} x{quantity} を追加しました");
                    return true;
                }
            }
        }
        
        // 追加しきれなかった
        if (remaining < quantity)
        {
            Debug.Log($"{item.itemName} x{quantity - remaining} を追加しました（{remaining}個追加できませんでした）");
            return true;
        }
        
        Debug.Log($"インベントリが満杯で {item.itemName} を追加できませんでした");
        return false;
    }
    
    // アイテムを削除
    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        
        int remaining = quantity;
        
        // 後ろから検索して削除
        for (int i = slots.Length - 1; i >= 0; i--)
        {
            if (!slots[i].IsEmpty() && slots[i].item == item)
            {
                int removed = slots[i].RemoveItem(remaining);
                remaining -= removed;
                
                if (remaining <= 0)
                {
                    Debug.Log($"{item.itemName} x{quantity} を削除しました");
                    return true;
                }
            }
        }
        
        Debug.Log($"{item.itemName} が不足しています");
        return false;
    }
    
    // アイテムの所持数を取得
    public int GetItemCount(ItemData item)
    {
        if (item == null) return 0;
        
        int count = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty() && slots[i].item == item)
            {
                count += slots[i].quantity;
            }
        }
        return count;
    }
    
    // 指定したアイテムを持っているか
    public bool HasItem(ItemData item, int quantity = 1)
    {
        return GetItemCount(item) >= quantity;
    }
    
    // インベントリをデバッグ表示
    [ContextMenu("インベントリを表示")]
    public void DebugPrintInventory()
    {
        Debug.Log("=== インベントリ内容 ===");
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty())
            {
                Debug.Log($"スロット {i}: {slots[i].item.itemName} x{slots[i].quantity}");
            }
        }
    }
}