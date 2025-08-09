using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Configuration")]
    [SerializeField] private StageData[] stages;

    private int currentStageIndex;

    private ProgressManager progressManager;
    private StageLoader stageLoader;

    public static StageManager Instance { get; private set; }

    public int CurrentStageIndex => currentStageIndex;

    // snapshots per stage index (in-memory)
    private Dictionary<int, LevelSnapshot> levelSnapshots = new Dictionary<int, LevelSnapshot>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        progressManager = new ProgressManager();
        stageLoader = new StageLoader();
    }

    private void OnEnable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if (GameStateController.Instance != null)
            GameStateController.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState newState)
    {
        if (newState == GameState.Playing)
        {
            LoadCurrentStage();
        }
        else if (newState == GameState.MainMenu || newState == GameState.GameOver)
        {
            UnloadCurrentStage();
            GameplayManagerHandler.Instance?.DestroyGameplayManager();
        }
    }

    private void LoadCurrentStage()
    {
        if (stages == null || stages.Length == 0)
        {
            Debug.LogError("StageManager: No stages configured.");
            return;
        }

        if (currentStageIndex >= stages.Length)
        {
            Debug.LogWarning("No more stages to load.");
            GameStateController.Instance.ChangeState(GameState.GameOver);
            currentStageIndex = stages.Length - 1;
            return;
        }

        // LoadStage returns true if we actually instantiated a new stage (vs already loaded)
        bool loadedNew = stageLoader.LoadStage(stages[currentStageIndex]);

        var gm = GameplayManagerHandler.Instance?.EnsureGameplayManager();

        if (loadedNew)
        {
            // If there's a saved snapshot for this stage, restore it;
            // otherwise capture a snapshot of the current state (so retry will restore to this initial state).
            gm?.StartLevel(currentStageIndex);

            if (levelSnapshots.TryGetValue(currentStageIndex, out var snap))
            {
                gm?.RestoreSnapshot(snap);
            }
            else
            {
                // capture current state (so retry/continue will get this)
                var newSnap = gm?.CaptureSnapshot();
                if (newSnap != null)
                    levelSnapshots[currentStageIndex] = newSnap;
            }
        }
        else
        {
            // Stage already loaded — do nothing (this prevents StartLevel being called on resume)
        }
    }

    private void UnloadCurrentStage()
    {
        stageLoader.UnloadStage();
    }

    public void AdvanceStage()
    {
        UnloadCurrentStage();
        currentStageIndex++;
        progressManager.SaveProgress(currentStageIndex);
        LoadCurrentStage();
    }

    public void StartNewGame()
    {
        UnloadCurrentStage();
        currentStageIndex = 0;
        progressManager.SaveProgress(currentStageIndex);

        // clear any previous snapshots when starting new game (fresh stage progression)
        levelSnapshots.Clear();

        LoadCurrentStage();
    }

    public void ContinueGame()
    {
        UnloadCurrentStage();
        LoadProgress();
        LoadCurrentStage();
    }

    private void LoadProgress()
    {
        currentStageIndex = progressManager.LoadProgress();
    }

    public void RestartCurrentStage()
    {
        UnloadCurrentStage();
        // keep snapshot (so we can restore to initial state of this run)
        LoadCurrentStage();
    }

    // Optional: API to explicitly set snapshot for current stage from elsewhere
    public void SaveSnapshotForCurrentStage(LevelSnapshot snapshot)
    {
        if (snapshot == null) return;
        levelSnapshots[currentStageIndex] = snapshot;
    }
}


public class StageLoader
{
    private GameObject currentStageInstance;

    /// <summary>
    /// If stage was actually instantiated returns true; if already loaded returns false.
    /// </summary>
    public bool LoadStage(StageData stageData)
    {
        if (currentStageInstance != null)
            return false;

        currentStageInstance = Object.Instantiate(stageData.stagePrefab);
        return true;
    }

    public void UnloadStage()
    {
        if (currentStageInstance != null)
        {
            Object.Destroy(currentStageInstance);
            currentStageInstance = null;
        }
    }
}



public class ProgressManager
{
    private const string LastStageKey = "LastStageIndex";

    public int LoadProgress()
    {
        return PlayerPrefs.GetInt(LastStageKey, 0);
    }

    public void SaveProgress(int stageIndex)
    {
        PlayerPrefs.SetInt(LastStageKey, stageIndex);
        PlayerPrefs.Save();
    }
}
