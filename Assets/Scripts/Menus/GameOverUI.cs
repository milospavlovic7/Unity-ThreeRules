using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        retryButton.onClick.AddListener(OnRetryClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnRetryClicked()
    {
        // prvo eksplicitno restartuj nivo (reload scene / prefab)
        StageManager.Instance?.RestartCurrentStage();

        // zatim promeni state na Playing (to će instancirati UI i gameplay)
        GameStateController.Instance.ChangeState(GameState.Playing);
    }

    private void OnMainMenuClicked()
    {
        GameStateController.Instance.ChangeState(GameState.MainMenu);
    }
}
