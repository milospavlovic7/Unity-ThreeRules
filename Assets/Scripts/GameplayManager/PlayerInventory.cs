using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int maxItems = 3;

    private List<Item> items = new List<Item>();

    public IReadOnlyList<Item> Items => items.AsReadOnly();

    public event Action OnInventoryChanged;

    private bool hasKey = false;

    public bool HasKey => hasKey;

    public void GainKey()
    {
        hasKey = true;
        OnInventoryChanged?.Invoke();
        Debug.Log("Key gained!");
    }

    public void RemoveKey()
    {
        hasKey = false;
        OnInventoryChanged?.Invoke();
        Debug.Log("Key removed!");
    }

    public bool AddItem(Item item)
    {
        if (items.Count >= maxItems)
            return false;

        items.Add(item);
        OnInventoryChanged?.Invoke();
        Debug.Log($"Item added: {item.itemName}");
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (items.Remove(item))
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"Item removed: {item.itemName}");
        }
    }

    public void ReplaceItem(int index, Item newItem)
    {
        if (index < 0 || index >= items.Count)
            return;

        items[index] = newItem;
        OnInventoryChanged?.Invoke();
        Debug.Log($"Item replaced at {index} with {newItem.itemName}");
    }

    public void ClearInventory()
    {
        items.Clear();
        hasKey = false; 
        OnInventoryChanged?.Invoke();
        Debug.Log("Inventory cleared");
    }
}
