using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickupTrigger : MonoBehaviour
{
    [Tooltip("ScriptableObject that describes this item")]
    public ItemData itemData;

    [Tooltip("Destroy pickup after collected (true)")]
    public bool destroyOnPickup = true;

    private bool collected = false;

    // state for replace-mode
    private bool awaitingReplace = false;
    private GameObject playerInTrigger = null;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;
        if (!collision.CompareTag("Player")) return;

        if (GameplayManager.Instance == null || GameplayManager.Instance.Inventory == null)
        {
            Debug.LogWarning("No GameplayManager/Inventory found - can't pick up item.");
            return;
        }

        int index = GameplayManager.Instance.Inventory.AddItem(itemData);
        if (index >= 0)
        {
            OnItemCollected(index);
        }
        else
        {
            StartReplaceMode(collision.gameObject);
        }
    }

    private void OnItemCollected(int index)
    {
        collected = true;
        Debug.Log($"Picked up {itemData.itemName} into slot {index}.");

        var ui = PlayingUIManager.Instance;

        if (ui != null && ui.SelectedIndex == -1)
            ui.SelectItemByIndex(index);

        if (destroyOnPickup)
            Destroy(gameObject);
        else
            gameObject.SetActive(false);
    }

    private void StartReplaceMode(GameObject player)
    {
        var ui = PlayingUIManager.Instance;
        if (ui == null)
        {
            Debug.Log("Inventory full - no UI to choose replacement.");
            return;
        }

        Debug.Log("Inventory full - entering replace mode. You may press 1/2/3 only while standing on this pickup.");

        awaitingReplace = true;
        playerInTrigger = player;

        ui.BeginReplaceMode(itemData,
            onSelected: (slotIndex) =>
            {
                if (!awaitingReplace || playerInTrigger == null)
                {
                    Debug.Log("Replace attempted but player left the pickup area.");
                    EndReplaceMode(ui);
                    return;
                }

                if (slotIndex >= 0)
                {
                    GameplayManager.Instance.Inventory.ReplaceItem(slotIndex, itemData);
                    Debug.Log($"Replaced slot {slotIndex} with {itemData.itemName}.");
                    EndReplaceMode(ui);
                    collected = true;

                    if (destroyOnPickup) Destroy(gameObject); else gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("Replace cancelled or invalid slot.");
                    EndReplaceMode(ui);
                }
            });
    }

    private void EndReplaceMode(PlayingUIManager ui)
    {
        awaitingReplace = false;
        playerInTrigger = null;
        ui?.EndReplaceMode();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (awaitingReplace && collision.gameObject == playerInTrigger)
        {
            var ui = PlayingUIManager.Instance;
            Debug.Log("Player left pickup area - replace cancelled.");
            EndReplaceMode(ui);
        }
    }
}
