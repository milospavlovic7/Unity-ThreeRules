using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InventoryClearTrigger : MonoBehaviour
{
    private bool triggered = false;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (!collision.CompareTag("Player")) return;

        var inv = GameplayManager.Instance?.Inventory;
        if (inv == null) return;

        inv.ClearInventory();
        PlayingUIManager.Instance?.UpdateInventoryUI();
        triggered = true;

        Debug.Log("[InventoryClearTrigger] Inventory cleared by stepping on clearing tile.");

        // optionally destroy the trigger so it only works once:
        Destroy(gameObject);
    }
}
