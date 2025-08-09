using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int maxItems = 3;

    private List<ItemData> items = new List<ItemData>();
    public IReadOnlyList<ItemData> Items => items.AsReadOnly();

    public event Action OnInventoryChanged;
    public event Action<ItemData> OnItemUsed; // fired when player "activates" an item

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

    /// <summary>
    /// Adds item to the first free slot. Returns index where added, or -1 if inventory full.
    /// </summary>
    public int AddItem(ItemData item)
    {
        if (items.Count >= maxItems)
            return -1;

        items.Add(item);
        OnInventoryChanged?.Invoke();
        Debug.Log($"Item added: {item.itemName} at index {items.Count - 1}");
        return items.Count - 1;
    }

    public void RemoveItem(ItemData item)
    {
        if (items.Remove(item))
        {
            OnInventoryChanged?.Invoke();
            Debug.Log($"Item removed: {item.itemName}");
        }
    }

    public void ReplaceItem(int index, ItemData newItem)
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

    /// <summary>
    /// Use (activate) item at index. If removeAfterUse is true, item will be removed from inventory.
    /// Returns the used item or null if invalid index.
    /// </summary>
    public ItemData UseItemAt(int index, bool removeAfterUse = false)
    {
        if (index < 0 || index >= items.Count)
            return null;

        ItemData item = items[index];
        OnItemUsed?.Invoke(item);

        if (removeAfterUse)
        {
            items.RemoveAt(index);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Item used and removed: {item.itemName}");
        }
        else
        {
            Debug.Log($"Item used: {item.itemName}");
        }

        return item;
    }

    /// <summary>
    /// Replace internal items list and hasKey in one shot (fires a single OnInventoryChanged).
    /// Use this for restoring snapshots to avoid multiple events.
    /// </summary>
    public void SetState(IEnumerable<ItemData> newItems, bool newHasKey)
    {
        items = new List<ItemData>(newItems ?? new List<ItemData>());
        hasKey = newHasKey;
        OnInventoryChanged?.Invoke();
        Debug.Log("Inventory state restored from snapshot.");
    }
}
