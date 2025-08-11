using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Tooltip("Max number of item slots (fixed).")]
    public int maxItems = 3;

    private List<ItemData> items = new List<ItemData>();

    public IReadOnlyList<ItemData> Items => items.AsReadOnly();

    public event Action OnInventoryChanged;
    public event Action<ItemData> OnItemUsed;

    private bool hasKey = false;
    public bool HasKey => hasKey;

    public int LastUsedItemIndex { get; private set; } = -1;

    private void Awake()
    {
        EnsureListSize();
    }

    private void OnValidate()
    {
        EnsureListSize();
    }

    private void EnsureListSize()
    {
        if (items == null)
            items = new List<ItemData>();

        while (items.Count < maxItems)
            items.Add(null);

        if (items.Count > maxItems)
            items.RemoveRange(maxItems, items.Count - maxItems);
    }

    public void GainKey()
    {
        hasKey = true;
        OnInventoryChanged?.Invoke();
        Debug.Log("Key gained!");

        AudioManager.Instance.PlaySFX(5);
    }

    public void RemoveKey()
    {
        hasKey = false;
        OnInventoryChanged?.Invoke();
        Debug.Log("Key removed!");
    }

    public int AddItem(ItemData item)
    {
        EnsureListSize();

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                OnInventoryChanged?.Invoke();
                AudioManager.Instance.PlaySFX(4);
                Debug.Log($"Item added: {item.itemName} into slot {i}.");

                return i;
            }
        }
        return -1;
    }

    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item)
            {
                items[i] = null;
                OnInventoryChanged?.Invoke();
                Debug.Log($"Item removed from slot {i}: {item.itemName}");
                return;
            }
        }
    }

    public void ReplaceItem(int index, ItemData newItem)
    {
        EnsureListSize();

        if (index < 0 || index >= items.Count)
            return;

        items[index] = newItem;
        OnInventoryChanged?.Invoke();
        AudioManager.Instance.PlaySFX(4);
        Debug.Log($"Item replaced at {index} with {newItem?.itemName ?? "null"}");
    }

    public void ClearInventory()
    {
        EnsureListSize();
        for (int i = 0; i < items.Count; i++)
            items[i] = null;
        OnInventoryChanged?.Invoke();
        Debug.Log("Inventory cleared");
        AudioManager.Instance.PlaySFX(8);
    }

    public ItemData UseItemAt(int index, bool removeAfterUse = false)
    {
        EnsureListSize();

        if (index < 0 || index >= items.Count)
            return null;

        ItemData item = items[index];
        if (item == null)
            return null;

        LastUsedItemIndex = index;

        OnItemUsed?.Invoke(item);
        AudioManager.Instance.PlaySFX(9);

        if (removeAfterUse)
        {
            items[index] = null;
            OnInventoryChanged?.Invoke();
            Debug.Log($"Item used and removed from slot {index}: {item.itemName}");
        }
        else
        {
            Debug.Log($"Item used (kept in slot {index}): {item.itemName}");
        }

        LastUsedItemIndex = -1;

        return item;
        
    }

    public void SetState(IEnumerable<ItemData> newItems, bool newHasKey)
    {
        items = new List<ItemData>(newItems ?? new List<ItemData>());
        EnsureListSize();
        hasKey = newHasKey;
        OnInventoryChanged?.Invoke();
        Debug.Log("Inventory state restored from snapshot.");
    }
}
