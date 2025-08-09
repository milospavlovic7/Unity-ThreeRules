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
    }

    private void ActivateSelectedItem()
    {
        var ui = PlayingUIManager.Instance;
        var gm = GameplayManager.Instance;
        if (ui == null || gm == null) return;

        int idx = ui.SelectedIndex;

        // dozvoli selektovati prazne slotove; ako je prazan, ne radi ništa
        if (idx < 0 || idx >= ui.ItemSlotsCount) // ItemSlotsCount ćemo dodati u PlayingUIManager
            return;

        // ako postoji item na tom indexu
        var inv = gm.Inventory;
        if (inv == null) return;

        if (idx >= inv.Items.Count)
        {
            // prazno mesto — ništa se ne dešava
            Debug.Log($"Activate: slot {idx + 1} is empty — nothing happens.");
            return;
        }

        var item = inv.UseItemAt(idx, removeAfterUse: true);
        if (item != null)
        {
            Debug.Log($"Activated item (slot #{idx + 1}): {item.itemName}");
            // Nakon UseItemAt(remove = true), inventory će emitovati OnInventoryChanged i UI će se osvežiti.
            // Zadržimo selekciju na tom slotu (sada praznom) — PlayingUIManager već to podržava.
        }
    }
}
