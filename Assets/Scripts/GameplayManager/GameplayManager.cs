using System;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    [Header("Phase & Level Info")]
    [SerializeField] private int currentPhaseIndex = 0;
    [SerializeField] private int currentLevelIndex = 0;

    public int CurrentPhaseIndex => currentPhaseIndex;
    public int CurrentLevelIndex => currentLevelIndex;

    public PlayerInventory Inventory { get; private set; }

    public event Action<int> OnMovesChanged;
    public event Action<int, int> OnLevelChanged; // phase, level
    public event Action OnInventoryChanged;

    private int movesMade = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Inventory = GetComponent<PlayerInventory>();
        if (Inventory == null)
        {
            Debug.LogError("PlayerInventory component missing from GameplayManager!");
        }
        else
        {
            Inventory.OnInventoryChanged += () => OnInventoryChanged?.Invoke();
        }

        RaiseLevelChanged();
        RaiseMovesChanged();
        RaiseInventoryChanged();
    }

    public void StartPhase(int phaseIndex)
    {
        currentPhaseIndex = phaseIndex;
        currentLevelIndex = 0;
        movesMade = 0;
        Inventory.ClearInventory();

        RaiseLevelChanged();
        RaiseMovesChanged();
        RaiseInventoryChanged();
    }

    public void StartLevel(int levelIndex)
    {
        currentLevelIndex = levelIndex;
        movesMade = 0;
        RaiseLevelChanged();
        RaiseMovesChanged();
    }

    public void RegisterMove()
    {
        movesMade++;
        OnMovesChanged?.Invoke(movesMade);
    }

    public int GetMovesMade()
    {
        return movesMade;
    }

    private void RaiseLevelChanged()
    {
        OnLevelChanged?.Invoke(currentPhaseIndex, currentLevelIndex);
    }

    private void RaiseMovesChanged()
    {
        OnMovesChanged?.Invoke(movesMade);
    }

    private void RaiseInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}
