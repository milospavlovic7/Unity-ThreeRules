using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manager koji sinhronizuje item efekte sa inventarom.
/// Treba da bude prisutan tokom gameplaya; preporučeno je dodati ga kao komponentu na GameplayManager prefab.
/// </summary>
[RequireComponent(typeof(GameplayManager))]
public class ItemEffectManager : MonoBehaviour
{
    private PlayerInventory inventory;
    private GameplayManager gm;

    // Copy of last known inventory items (to detect additions/removals)
    private List<ItemData> previousItems = new List<ItemData>();

    private void Awake()
    {
        gm = GetComponent<GameplayManager>();
        // Inventory komponenta je na istom prefab-u kao GameplayManager u tvom setupu
        inventory = gm?.Inventory;

        // If inventory is not available yet, wait until Start (or initialize externally)
    }

    private void Start()
    {
        if (inventory == null && gm != null)
        {
            inventory = gm.Inventory;
        }

        if (inventory != null)
        {
            // Initialize previousItems with current items
            previousItems = new List<ItemData>(inventory.Items);

            inventory.OnInventoryChanged += HandleInventoryChanged;
            inventory.OnItemUsed += HandleItemUsed;
        }
        else
        {
            Debug.LogWarning("[ItemEffectManager] Inventory not found on Start.");
            // Try to subscribe later when GameplayManager initialized
            StartCoroutine(WaitForInventory());
        }
    }

    private System.Collections.IEnumerator WaitForInventory()
    {
        while (gm != null && (gm.Inventory == null))
            yield return null;

        inventory = gm.Inventory;
        if (inventory != null)
        {
            previousItems = new List<ItemData>(inventory.Items);
            inventory.OnInventoryChanged += HandleInventoryChanged;
            inventory.OnItemUsed += HandleItemUsed;
        }
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
        // Compare lists by reference (ItemData ScriptableObject)
        var current = new List<ItemData>(inventory.Items);

        // Added = current - previous
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

        // Update previous
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

        // Note: If UseItemAt was called with removeAfterUse == true, Inventory.OnInventoryChanged will be triggered later,
        // and HandleInventoryChanged will call OnRemove automatically.
    }

    /// <summary>
    /// Optional: call this each turn if you want per-turn updates for passive items.
    /// Call from GameplayManager when moves are registered (e.g. in RegisterMove).
    /// </summary>
    public void OnTurnPassed()
    {
        if (inventory == null) return;

        foreach (var item in inventory.Items)
        {
            item.effect?.OnTurnUpdate(gm);
        }
    }
}
