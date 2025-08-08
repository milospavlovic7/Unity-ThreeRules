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

        stageLoader.LoadStage(stages[currentStageIndex]);

        var gm = GameplayManagerHandler.Instance?.EnsureGameplayManager();
        gm?.StartLevel(currentStageIndex);
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
        progressManager.SaveProgress(currentStageIndex);
        LoadCurrentStage();
    }
}


public class StageLoader
{
    private GameObject currentStageInstance;

    public void LoadStage(StageData stageData)
    {
        if (currentStageInstance != null)
            return;

        currentStageInstance = Object.Instantiate(stageData.stagePrefab);
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
