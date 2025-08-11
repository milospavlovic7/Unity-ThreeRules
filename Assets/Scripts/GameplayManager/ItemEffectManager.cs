using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(GameplayManager))]
public class ItemEffectManager : MonoBehaviour
{
    private PlayerInventory inventory;
    private GameplayManager gm;

    private List<ItemData> previousItems = new List<ItemData>();

    private void Awake()
    {
        gm = GetComponent<GameplayManager>();
        inventory = gm?.Inventory;
    }

    private void Start()
    {
        if (inventory == null && gm != null)
            inventory = gm.Inventory;

        if (inventory != null)
            SubscribeToInventory();
        else
        {
            Debug.LogWarning("[ItemEffectManager] Inventory not found on Start.");
            StartCoroutine(WaitForInventory());
        }
    }

    private System.Collections.IEnumerator WaitForInventory()
    {
        while (gm != null && gm.Inventory == null)
            yield return null;

        inventory = gm.Inventory;
        if (inventory != null)
            SubscribeToInventory();
    }

    private void SubscribeToInventory()
    {
        previousItems = new List<ItemData>(inventory.Items);
        inventory.OnInventoryChanged += HandleInventoryChanged;
        inventory.OnItemUsed += HandleItemUsed;
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= HandleInventoryChanged;
            inventory.OnItemUsed -= HandleItemUsed;
        }
    }

    private void HandleInventoryChanged()
    {
        var current = new List<ItemData>(inventory.Items);

        var added = current.Except(previousItems).ToList();
        var removed = previousItems.Except(current).ToList();

        foreach (var item in added)
        {
            if (item?.effect != null)
            {
                item.effect.OnPickup(gm);
                Debug.Log($"[ItemEffectManager] OnPickup executed for {item.itemName}");
            }
        }

        foreach (var item in removed)
        {
            if (item?.effect != null)
            {
                item.effect.OnRemove(gm);
                Debug.Log($"[ItemEffectManager] OnRemove executed for {item.itemName}");
            }
        }

        previousItems = current;
    }

    private void HandleItemUsed(ItemData item)
    {
        if (item == null) return;

        if (item.effect != null)
        {
            item.effect.ActivateEffect(gm);
            Debug.Log($"[ItemEffectManager] Activated effect for {item.itemName}");
        }
    }

    public void OnTurnPassed()
    {
        if (inventory == null) return;

        foreach (var item in inventory.Items)
        {
            item.effect?.OnTurnUpdate(gm);
        }
    }
}