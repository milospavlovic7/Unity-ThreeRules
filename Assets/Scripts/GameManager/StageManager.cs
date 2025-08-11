using System.Collections;
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

    [SerializeField] private bool debugMode = false; // Debug flag for logs

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
        // Validate stages array and current stage index
        if (stages == null || stages.Length == 0)
        {
            Debug.LogError("StageManager: No stages configured.");
            return;
        }

        if (currentStageIndex >= stages.Length || stages[currentStageIndex] == null)
        {
            currentStageIndex = 0;
            progressManager.SaveProgress(currentStageIndex);
            levelSnapshots.Clear();
            GameStateController.Instance?.ChangeState(GameState.MainMenu);
            return;
        }

        bool loadedNew = stageLoader.LoadStage(stages[currentStageIndex]);

        var gm = GameplayManagerHandler.Instance?.EnsureGameplayManager();

        if (loadedNew)
        {
            gm?.StartLevel(currentStageIndex);

            if (levelSnapshots.TryGetValue(currentStageIndex, out var snap))
            {
                gm?.RestoreSnapshot(snap);
                if (debugMode) Debug.Log($"[StageManager] Restored snapshot for stage {currentStageIndex}");
            }
            else
            {
                var newSnap = gm?.CaptureSnapshot();
                if (newSnap != null)
                {
                    levelSnapshots[currentStageIndex] = newSnap;
                    if (debugMode) Debug.Log($"[StageManager] Captured new snapshot for stage {currentStageIndex}");
                }
            }
        }
        else
        {
            if (debugMode) Debug.Log($"[StageManager] Stage {currentStageIndex} already loaded, skipping start level.");
        }

        // Check if PlayingUIManager is already instantiated
        if (PlayingUIManager.Instance != null)
        {
            // If it is, directly update UI
            UpdateUIForStage();
        }
        else
        {
            // Otherwise, wait until PlayingUIManager is instantiated
            StartCoroutine(WaitForUIManagerAndUpdate());
        }
    }

    private void UpdateUIForStage()
    {
        if (stages[currentStageIndex].isStoryStage)
        {
            PlayingUIManager.Instance.DisableUI();
        }
        else
        {
            PlayingUIManager.Instance.EnableUI();
        }
    }

    private IEnumerator WaitForUIManagerAndUpdate()
    {
        // Wait until PlayingUIManager is instantiated
        while (PlayingUIManager.Instance == null)
        {
            yield return null; // Wait one frame
        }

        // Now that PlayingUIManager is available, update the UI based on the stage type
        UpdateUIForStage();
    }

    private void UnloadCurrentStage()
    {
        stageLoader.UnloadStage();
        if (debugMode) Debug.Log($"[StageManager] Unloaded current stage.");
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
        if (debugMode) Debug.Log($"[StageManager] Loaded progress, current stage index: {currentStageIndex}");
    }

    public void RestartCurrentStage()
    {
        UnloadCurrentStage();
        LoadCurrentStage();
        if (debugMode) Debug.Log($"[StageManager] Restarted current stage.");
    }

    public void SaveSnapshotForCurrentStage(LevelSnapshot snapshot)
    {
        if (snapshot == null) return;
        levelSnapshots[currentStageIndex] = snapshot;
        if (debugMode) Debug.Log($"[StageManager] Saved snapshot for stage {currentStageIndex}");
    }
}



public class StageLoader
{
    private GameObject currentStageInstance;

    public bool LoadStage(StageData stageData)
    {
        if (currentStageInstance != null)
            return false;

        if (stageData == null || stageData.stagePrefab == null)
        {
            Debug.LogError("[StageLoader] Invalid stage data or prefab.");
            return false;
        }

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
