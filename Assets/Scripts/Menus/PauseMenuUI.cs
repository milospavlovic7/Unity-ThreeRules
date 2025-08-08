using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;

    private void Start()
    {
        resumeButton.onClick.AddListener(OnResumeClicked);
        mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    private void OnResumeClicked()
    {
        GameStateController.Instance.ChangeState(GameState.Playing);
    }

    private void OnMainMenuClicked()
    {
        GameStateController.Instance.ChangeState(GameState.MainMenu);
    }
}
