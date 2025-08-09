using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayingUIManager : MonoBehaviour
{
    public static PlayingUIManager Instance { get; private set; }

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

    private Coroutine waitForGameplayCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        TryInitializeOrWait();
    }

    private void OnEnable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged += OnGameStateChanged;

        TryInitializeOrWait();
    }

    private void OnDisable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged -= OnGameStateChanged;

        CancelWaitCoroutine();
        UnsubscribeFromInventoryAndGameplay();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Playing)
            TryInitializeOrWait();
        else
            UnsubscribeFromInventoryAndGameplay();
    }

    private void TryInitializeOrWait()
    {
        if (inventory != null && GameplayManager.Instance != null)
        {
            UpdateInventoryUI();
            UpdateLevelPhaseUI(GameplayManager.Instance.CurrentPhaseIndex, GameplayManager.Instance.CurrentLevelIndex);
            UpdateMovesUI(GameplayManager.Instance.GetMovesMade());
            return;
        }

        if (GameplayManager.Instance != null && GameplayManager.Instance.Inventory != null)
        {
            InitializeWithGameplayManager();
            return;
        }

        CancelWaitCoroutine();
        waitForGameplayCoroutine = StartCoroutine(WaitForGameplayManagerAndInit());
    }

    private IEnumerator WaitForGameplayManagerAndInit()
    {
        while (GameplayManager.Instance == null || GameplayManager.Instance.Inventory == null)
            yield return null;

        yield return null; // wait one frame
        InitializeWithGameplayManager();
        waitForGameplayCoroutine = null;
    }

    private void CancelWaitCoroutine()
    {
        if (waitForGameplayCoroutine != null)
        {
            StopCoroutine(waitForGameplayCoroutine);
            waitForGameplayCoroutine = null;
        }
    }

    private void InitializeWithGameplayManager()
    {
        UnsubscribeFromInventoryAndGameplay();

        inventory = GameplayManager.Instance.Inventory;
        if (inventory == null)
        {
            Debug.LogError("PlayingUIManager: Inventory still null during initialization.");
            return;
        }

        inventory.OnInventoryChanged += UpdateInventoryUI;
        inventory.OnItemUsed += OnItemUsed;

        GameplayManager.Instance.OnInventoryChanged += UpdateInventoryUI;
        GameplayManager.Instance.OnLevelChanged += UpdateLevelPhaseUI;
        GameplayManager.Instance.OnMovesChanged += UpdateMovesUI;

        UpdateInventoryUI();
        UpdateLevelPhaseUI(GameplayManager.Instance.CurrentPhaseIndex, GameplayManager.Instance.CurrentLevelIndex);
        UpdateMovesUI(GameplayManager.Instance.GetMovesMade());

        ClearDescription();
        HideKeySlot();

        if (inventory.Items.Count > 0 && selectedIndex == -1)
            SelectItem(0);
    }

    private void UnsubscribeFromInventoryAndGameplay()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= UpdateInventoryUI;
            inventory.OnItemUsed -= OnItemUsed;
        }

        if (GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnInventoryChanged -= UpdateInventoryUI;
            GameplayManager.Instance.OnLevelChanged -= UpdateLevelPhaseUI;
            GameplayManager.Instance.OnMovesChanged -= UpdateMovesUI;
        }

        inventory = null;
    }

    public void UpdateInventoryUI()
    {
        if (inventory == null) return;

        var items = inventory.Items;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < items.Count)
            {
                var item = items[i];
                itemSlots[i].itemIcon.sprite = item.sprite;
                itemSlots[i].itemIcon.enabled = true;
                itemSlots[i].itemText.text = item.itemName;
            }
            else
            {
                itemSlots[i].itemIcon.sprite = null;
                itemSlots[i].itemIcon.enabled = false;
                itemSlots[i].itemText.text = "";
            }

            // highlight slot even if it's empty if selectedIndex points to it
            itemSlots[i].slotBackground.color = (i == selectedIndex) ? selectedColor : normalColor;
        }

        UpdateKeySlot();
    }

    private void UpdateKeySlot()
    {
        if (keySlotImage == null || inventory == null) return;
        keySlotImage.enabled = inventory.HasKey;
    }

    private void HideKeySlot()
    {
        if (keySlotImage != null)
            keySlotImage.enabled = false;
    }

    public void UpdateLevelPhaseUI(int phase, int level)
    {
        if (phaseText != null) phaseText.text = $"Phase: {phase + 1}";
        if (levelText != null) levelText.text = $"Level: {level + 1}";
    }

    public void UpdateMovesUI(int moves)
    {
        if (movesText != null) movesText.text = $"Moves: {moves}";
    }

    // Modified: allow selecting empty slots (0 .. itemSlots.Length-1)
    public void SelectItem(int index)
    {
        if (index < 0 || index >= itemSlots.Length)
        {
            // invalid index -> clear
            ClearDescription();
            selectedIndex = -1;
            UpdateInventoryUI();
            return;
        }

        selectedIndex = index;

        // if slot has item, show description, otherwise clear
        if (inventory != null && index < inventory.Items.Count)
        {
            if (descriptionText != null)
                descriptionText.text = inventory.Items[index].description;
        }
        else
        {
            ClearDescription();
        }

        UpdateInventoryUI();
    }

    // wrapper for input
    public void SelectItemByIndex(int index) => SelectItem(index);

    public int SelectedIndex => selectedIndex;
    public int ItemSlotsCount => itemSlots?.Length ?? 0; // used by input controller

    public void ClearDescription()
    {
        if (descriptionText != null) descriptionText.text = "";
    }

    public void OnItemSlotClicked(int index) => SelectItem(index);

    private void OnItemUsed(ItemData item)
    {
        if (descriptionText != null)
            descriptionText.text = $"Activated: {item.itemName}";

        StartCoroutine(ClearDescriptionAfterDelay(1.2f));
    }

    private IEnumerator ClearDescriptionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ClearDescription();
    }

    public void AnimateAddItemAt(int index)
    {
        if (index < 0 || index >= itemSlots.Length) return;
        StartCoroutine(AnimateAddCoroutine(itemSlots[index]));
    }

    private IEnumerator AnimateAddCoroutine(ItemSlotUI slot)
    {
        if (slot == null) yield break;

        var rt = slot.itemIcon.rectTransform;
        Vector3 initialScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;
        float duration = 0.18f;
        float t = 0f;

        slot.itemIcon.enabled = true;
        rt.localScale = initialScale;

        while (t < duration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(initialScale, targetScale, t / duration);
            yield return null;
        }
        rt.localScale = targetScale;

        Color orig = slot.slotBackground.color;
        Color flash = Color.Lerp(orig, Color.white, 0.7f);
        float flashDur = 0.18f;
        t = 0f;
        while (t < flashDur)
        {
            t += Time.deltaTime;
            slot.slotBackground.color = Color.Lerp(flash, orig, t / flashDur);
            yield return null;
        }
        slot.slotBackground.color = orig;
    }
}
