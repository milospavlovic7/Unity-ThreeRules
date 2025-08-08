using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayingUIManager : MonoBehaviour
{
    [Header("Info Panel")]
    public Text levelText;
    public Text phaseText;
    public Text movesText;

    [Header("Inventory Panel")]
    public Image keySlotImage;

    [System.Serializable]
    public class ItemSlotUI
    {
        public Image slotBackground;  // ItemSlot image (za menjanje boje selekcije)
        public Image itemIcon;        // ItemIcon image (sprite)
        public Text itemText;         // ItemText (naziv itema)
    }

    public ItemSlotUI[] itemSlots = new ItemSlotUI[3];
    public Text descriptionText;

    private PlayerInventory inventory;
    private int selectedIndex = -1;

    private Color selectedColor = Color.yellow;
    private Color normalColor = Color.white;

    private void Start()
    {
        if (GameplayManager.Instance == null)
        {
            Debug.LogError("GameplayManager instance not found!");
            return;
        }

        inventory = GameplayManager.Instance.Inventory;
        if (inventory == null)
        {
            Debug.LogError("PlayerInventory not found in GameplayManager!");
            return;
        }

        // Inicijalno postavljanje UI
        UpdateInventoryUI();
        UpdateLevelPhaseUI(GameplayManager.Instance.CurrentPhaseIndex, GameplayManager.Instance.CurrentLevelIndex);
        UpdateMovesUI(GameplayManager.Instance.GetMovesMade());

        ClearDescription();
        HideKeySlot();

        if (inventory.Items.Count > 0)
            SelectItem(0);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateInventoryUI;

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
            GameplayManager.Instance.OnLevelChanged -= UpdateLevelPhaseUI;
            GameplayManager.Instance.OnMovesChanged -= UpdateMovesUI;
        }
    }

    public void UpdateInventoryUI()
    {
        var items = inventory.Items;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                itemSlots[i].itemIcon.sprite = item.sprite;
                itemSlots[i].itemIcon.enabled = true;

                itemSlots[i].itemText.text = item.itemName;

                // Set slot background color based on selection
                itemSlots[i].slotBackground.color = (i == selectedIndex) ? selectedColor : normalColor;
            }
            else
            {
                itemSlots[i].itemIcon.sprite = null;
                itemSlots[i].itemIcon.enabled = false;

                itemSlots[i].itemText.text = "";

                itemSlots[i].slotBackground.color = normalColor;
            }
        }

        UpdateKeySlot();
    }

    private void UpdateKeySlot()
    {
        keySlotImage.enabled = inventory.HasKey;
    }

    private void HideKeySlot()
    {
        if (keySlotImage != null)
            keySlotImage.enabled = false;
    }

    public void UpdateLevelPhaseUI(int phase, int level)
    {
        if (phaseText != null)
            phaseText.text = $"Phase: {phase + 1}";
        if (levelText != null)
            levelText.text = $"Level: {level + 1}";
    }

    public void UpdateMovesUI(int moves)
    {
        if (movesText != null)
            movesText.text = $"Moves: {moves}";
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= inventory.Items.Count)
        {
            ClearDescription();
            selectedIndex = -1;
            UpdateInventoryUI();
            return;
        }

        selectedIndex = index;
        descriptionText.text = inventory.Items[index].description;

        // Update slot background colors to highlight selection
        UpdateInventoryUI();
    }

    public void ClearDescription()
    {
        descriptionText.text = "";
    }

    public void OnItemSlotClicked(int index)
    {
        SelectItem(index);
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += UpdateInventoryUI;

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnInventoryChanged += UpdateInventoryUI;
            GameplayManager.Instance.OnLevelChanged += UpdateLevelPhaseUI;
            GameplayManager.Instance.OnMovesChanged += UpdateMovesUI;
        }
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= UpdateInventoryUI;

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
            GameplayManager.Instance.OnLevelChanged -= UpdateLevelPhaseUI;
            GameplayManager.Instance.OnMovesChanged -= UpdateMovesUI;
        }
    }
}
