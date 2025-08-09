using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickupTrigger : MonoBehaviour
{
    [Tooltip("ScriptableObject that describes this item")]
    public ItemData itemData;

    [Tooltip("Destroy pickup after collected (true)")]
    public bool destroyOnPickup = true;

    private bool collected = false;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true; // preporucceno
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
            collected = true;
            Debug.Log($"Picked up {itemData.itemName} into slot {index}.");

            // Animiraj UI dodavanje ako PlayingUIManager postoji
            var ui = PlayingUIManager.Instance;
            ui?.AnimateAddItemAt(index);

            // Ako nemamo selektovan nijedan slot, automatski izaberi novododati item
            if (ui != null && ui.SelectedIndex == -1)
            {
                ui.SelectItemByIndex(index);
            }

            if (destroyOnPickup)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Inventory full - can't pick up item.");
            // mozes dodati feedback igracu ovde (npr. zvuk)
        }
    }
}
