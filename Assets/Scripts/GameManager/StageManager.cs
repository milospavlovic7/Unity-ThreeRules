using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Configuration")]
    [SerializeField] private StageData[] stages;

    private int currentStageIndex = 0;
    private GameObject currentStageInstance;

    public static StageManager Instance { get; private set; }

    public int CurrentStageIndex => currentStageIndex;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        GameStateController.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
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
        }
    }

    private void LoadCurrentStage()
    {
        if (currentStageIndex >= stages.Length)
        {
            Debug.LogWarning("No more stages to load.");
            GameStateController.Instance.ChangeState(GameState.GameOver);
            currentStageIndex = stages.Length - 1;
            return;
        }

        if (currentStageInstance != null)
            return;

        var stagePrefab = stages[currentStageIndex].stagePrefab;
        currentStageInstance = Instantiate(stagePrefab);
    }

    private void UnloadCurrentStage()
    {
        if (currentStageInstance != null)
        {
            Destroy(currentStageInstance);
            currentStageInstance = null;
        }
    }

    public void AdvanceStage()
    {
        UnloadCurrentStage();
        currentStageIndex++;
        SaveProgress();
        LoadCurrentStage();
    }

    public void StartNewGame()
    {
        UnloadCurrentStage();
        currentStageIndex = 0;
        SaveProgress();
        LoadCurrentStage();
    }

    public void ContinueGame()
    {
        UnloadCurrentStage();
        LoadCurrentStage();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("LastStageIndex", currentStageIndex);
        PlayerPrefs.Save();
    }

    public void LoadProgress()
    {
        currentStageIndex = PlayerPrefs.GetInt("LastStageIndex", 0);
    }

    public void RestartCurrentStage()
    {
        UnloadCurrentStage();   
        SaveProgress();         
        LoadCurrentStage();     
    }
}
