using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;
    
    public InventorySlot()
    {
        item = null;
        quantity = 0;
    }
    
    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
    
    // スロットが空かどうか
    public bool IsEmpty()
    {
        return item == null || quantity <= 0;
    }
    
    // スロットをクリア
    public void Clear()
    {
        item = null;
        quantity = 0;
    }
    
    // アイテムを追加（スタック可能な場合）
    // 戻り値: 追加できなかった余り
    public int AddItem(ItemData itemToAdd, int amount)
    {
        // 空のスロットの場合
        if (IsEmpty())
        {
            item = itemToAdd;
            quantity = Mathf.Min(amount, itemToAdd.maxStack);
            return amount - quantity;
        }
        
        // 同じアイテムの場合
        if (item == itemToAdd)
        {
            int maxAdd = item.maxStack - quantity;
            int actualAdd = Mathf.Min(maxAdd, amount);
            quantity += actualAdd;
            return amount - actualAdd;
        }
        
        // 違うアイテムの場合は追加できない
        return amount;
    }
    
    // アイテムを削除
    // 戻り値: 実際に削除できた数
    public int RemoveItem(int amount)
    {
        if (IsEmpty()) return 0;
        
        int removed = Mathf.Min(quantity, amount);
        quantity -= removed;
        
        if (quantity <= 0)
        {
            Clear();
        }
        
        return removed;
    }
}