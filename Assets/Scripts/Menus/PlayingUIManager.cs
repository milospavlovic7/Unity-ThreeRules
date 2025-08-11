using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayingUIManager : MonoBehaviour
{
    public static PlayingUIManager Instance { get; private set; }

    private Color selectedColor = Color.yellow;
    private Color normalColor = Color.white;
    private Color replaceColor = new Color(0.35f, 0.6f, 1f); // pleasant blue

    [Header("Info Panel")]
    public Text movesText;

    [Header("Inventory Panel")]
    public Image keySlotImage;

    [System.Serializable]
    public class ItemSlotUI
    {
        public Image slotBackground;
        public Image itemIcon;
        public Text itemText;
    }

    public ItemSlotUI[] itemSlots = new ItemSlotUI[3];
    public Text descriptionText;

    private PlayerInventory inventory;
    private int selectedIndex = -1;
    private bool isInReplaceMode = false;
    private ItemData pendingReplaceNewItem;
    private System.Action<int> replaceCallback;

    public int ItemSlotsCount => itemSlots?.Length ?? 0;
    public int SelectedIndex => selectedIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DisableUI();
    }

    private void OnEnable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged += OnGameStateChanged;
        TryInitializeInventory();
    }

    private void OnDisable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged -= OnGameStateChanged;

        UnsubscribeFromInventoryEvents();
    }

    private void OnDestroy() => Instance = null;

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
            TryInitializeInventory();
        else
            UnsubscribeFromInventoryEvents();
    }

    private void TryInitializeInventory()
    {
        if (inventory != null)
        {
            UpdateInventoryUI();
            return;
        }

        if (GameplayManager.Instance?.Inventory != null)
        {
            inventory = GameplayManager.Instance.Inventory;
            SubscribeToInventoryEvents();
            UpdateInventoryUI();
        }
    }

    private void SubscribeToInventoryEvents()
    {
        inventory.OnInventoryChanged += UpdateInventoryUI;
        inventory.OnItemUsed += OnItemUsed;
    }

    private void UnsubscribeFromInventoryEvents()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryUI;
            inventory.OnItemUsed -= OnItemUsed;
        }
    }

    // This method is now responsible for updating the inventory UI and clearing description when necessary
    public void UpdateInventoryUI()
    {
        if (inventory == null) return;

        bool descriptionCleared = true; // We will check if we need to clear the description

        for (int i = 0; i < itemSlots.Length; i++)
        {
            var slotUI = itemSlots[i];
            if (slotUI == null) continue;

            ItemData item = i < inventory.Items.Count ? inventory.Items[i] : null;

            if (item != null)
            {
                slotUI.itemIcon.sprite = item.sprite;
                slotUI.itemIcon.enabled = true;
                slotUI.itemText.text = item.itemName;

                // Set the description of the selected item or clear if empty
                if (descriptionText != null && i == selectedIndex)
                {
                    descriptionText.text = item.description;
                }
                descriptionCleared = false;
            }
            else
            {
                slotUI.itemIcon.enabled = false;
                slotUI.itemText.text = "";
            }

            // Highlight selected item and handle replace mode color
            if (isInReplaceMode)
                slotUI.slotBackground.color = replaceColor;
            else
                slotUI.slotBackground.color = (i == selectedIndex) ? selectedColor : normalColor;
        }

        if (keySlotImage != null)
            keySlotImage.enabled = inventory.HasKey;

        // If no item is selected and description should be cleared, clear it
        if (descriptionCleared)
        {
            ClearDescription();
        }
    }

    private void ClearDescription() => descriptionText.text = "";

    private void SelectItem(int index)
    {
        if (index < 0 || index >= ItemSlotsCount) return;

        selectedIndex = index;
        ItemData selectedItem = inventory.Items[selectedIndex];
        ClearDescription();

        // Update description if the item is valid
        if (selectedItem != null)
        {
            descriptionText.text = selectedItem.description;
        }

        UpdateInventoryUI();
    }

    public void BeginReplaceMode(ItemData newItem, System.Action<int> onSelected)
    {
        if (isInReplaceMode) return;

        isInReplaceMode = true;
        pendingReplaceNewItem = newItem;
        replaceCallback = onSelected;

        // Inform player about replacement mode
        descriptionText.text = $"Inventory full — press 1/2/3 to replace with {newItem.itemName}. Move away to cancel.";
        UpdateInventoryUI();
    }

    public void EndReplaceMode()
    {
        isInReplaceMode = false;
        pendingReplaceNewItem = null;
        replaceCallback = null;

        ClearDescription();
        UpdateInventoryUI();
    }

    public void SelectItemByIndex(int index)
    {
        if (index < 0 || index >= itemSlots.Length) return;

        if (isInReplaceMode)
        {
            replaceCallback?.Invoke(index);
        }
        else
        {
            SelectItem(index);
        }
    }

    private void OnItemUsed(ItemData usedItem)
    {
        // Clear the description after using an item
        ClearDescription();

        if (usedItem == null)
        {
            SelectItem(-1); // Deselect item if used item is null
        }
        else
        {
            // Check if item is still in the selected slot
            if (inventory.Items[selectedIndex] == null)
            {
                // If no item is in the selected slot, clear description
                ClearDescription();
            }
            else
            {
                // Keep item selected
                SelectItem(selectedIndex);
            }
        }
    }

    public void DisableUI()
    {
        movesText.gameObject.SetActive(false);
        keySlotImage.gameObject.SetActive(false);
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.slotBackground.gameObject.SetActive(false);
        }
    }

    public void EnableUI()
    {
        movesText.gameObject.SetActive(true);
        keySlotImage.gameObject.SetActive(true);
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.slotBackground.gameObject.SetActive(true);
        }
    }
}
