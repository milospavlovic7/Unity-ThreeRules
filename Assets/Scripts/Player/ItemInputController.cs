using UnityEngine;

public class ItemInputController : MonoBehaviour
{
    private UIActionsInputActions controls;

    private void Awake()
    {
        controls = new UIActionsInputActions();

        // Selektovanje itema
        controls.UIActions.SelectItem1.performed += _ => SelectItem(0);
        controls.UIActions.SelectItem2.performed += _ => SelectItem(1);
        controls.UIActions.SelectItem3.performed += _ => SelectItem(2);

        // Aktivacija
        controls.UIActions.ActivateItem.performed += _ => ActivateSelectedItem();

        // Osiguraj da su akcije enable-ovane odmah (ako je ovaj objekt persistent, input radi uvek)
        controls.Enable();
    }

    private void OnEnable()
    {
        // Za svaki slučaj (u slučaju da si disable/enable objekat) uveri se da je map uključen
        controls.UIActions.Enable();
    }

    private void OnDisable()
    {
        // Isključi mapu kad se objekt onemogući
        controls.UIActions.Disable();
    }

    private void SelectItem(int index)
    {
        PlayingUIManager.Instance?.SelectItemByIndex(index);
        AudioManager.Instance.PlaySFX(4);
    }

    private void ActivateSelectedItem()
    {
        var ui = PlayingUIManager.Instance;
        var gm = GameplayManager.Instance;
        if (ui == null || gm == null) return;

        int idx = ui.SelectedIndex;

        if (idx < 0 || idx >= ui.ItemSlotsCount)
            return;

        var inv = gm.Inventory;
        if (inv == null) return;

        // New: check slot content directly (inventory uses fixed slots, null = empty)
        if (idx >= inv.Items.Count || inv.Items[idx] == null)
        {
            Debug.Log($"Activate: slot {idx + 1} is empty — nothing happens.");
            return;
        }

        var item = inv.Items[idx];

        // ONLY allow activation (and removal) for ACTIVE items
        if (item.itemType != ItemType.Active)
        {
            Debug.Log($"Item {item.itemName} is passive — cannot activate with Space.");
            return;
        }

        // Use the item and remove it (active items consumed on use)
        var used = inv.UseItemAt(idx, removeAfterUse: true);
        if (used != null)
        {
            Debug.Log($"Activated (and consumed) item (slot #{idx + 1}): {used.itemName}");
            // Selection remains at same index (slot is now empty) — UI will reflect this.
        }
    }
}
