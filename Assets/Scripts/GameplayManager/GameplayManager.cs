using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelSnapshot
{
    public List<ItemData> items = new List<ItemData>();
    public int moves = 0;
    public bool hasKey = false;
}

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
    public event Action<int, int> OnLevelChanged;
    public event Action OnInventoryChanged;
    public event Action<Vector2Int> OnPlayerMoved;

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
        Inventory?.ClearInventory();

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

    public int GetMovesMade() => movesMade;

    private void RaiseLevelChanged() => OnLevelChanged?.Invoke(currentPhaseIndex, currentLevelIndex);
    private void RaiseMovesChanged() => OnMovesChanged?.Invoke(movesMade);
    private void RaiseInventoryChanged() => OnInventoryChanged?.Invoke();

    public void NotifyPlayerMoved(Vector2Int playerMoveDirection)
    {
        RegisterMove();
        OnPlayerMoved?.Invoke(playerMoveDirection);
    }

    public LevelSnapshot CaptureSnapshot()
    {
        var snap = new LevelSnapshot
        {
            items = Inventory != null ? new List<ItemData>(Inventory.Items) : new List<ItemData>(),
            moves = movesMade,
            hasKey = Inventory != null && Inventory.HasKey
        };
        return snap;
    }

    public void RestoreSnapshot(LevelSnapshot snapshot)
    {
        if (snapshot == null) return;

        Inventory?.SetState(snapshot.items, snapshot.hasKey);
        movesMade = snapshot.moves;

        RaiseMovesChanged();
        RaiseInventoryChanged();

        Debug.Log("GameplayManager: snapshot restored.");
    }
}

