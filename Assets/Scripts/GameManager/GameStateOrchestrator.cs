using UnityEngine;

public class GameStateOrchestrator : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject mainMenuPrefab;
    public GameObject pausedMenuPrefab;
    public GameObject gameOverPrefab;
    public GameObject playingUIPrefab;

    private GameObject currentUI;

    private void Start()
    {
        HandleGameStateChanged(GameStateController.Instance.CurrentState);
    }

    private void OnEnable()
    {
        GameStateController.Instance.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        GameStateController.Instance.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void HandleGameStateChanged(GameState state)
    {
        if (currentUI != null)
            Destroy(currentUI);

        switch (state)
        {
            case GameState.MainMenu:
                currentUI = Instantiate(mainMenuPrefab);
                break;
            case GameState.Paused:
                currentUI = Instantiate(pausedMenuPrefab);
                break;
            case GameState.GameOver:
                currentUI = Instantiate(gameOverPrefab);
                break;
            case GameState.Playing:
                currentUI = Instantiate(playingUIPrefab);
                break;
        }
    }
}
