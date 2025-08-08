using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button exitButton;

    private void Start()
    {
        newGameButton.onClick.AddListener(OnNewGameClicked);
        continueButton.onClick.AddListener(OnContinueClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        // Prikazi Continue samo ako ima snimljen progres
        if (PlayerPrefs.HasKey("LastStageIndex") && PlayerPrefs.GetInt("LastStageIndex") > 0)
            continueButton.gameObject.SetActive(true);
        else
            continueButton.gameObject.SetActive(false);
    }

    private void OnNewGameClicked()
    {
        StageManager.Instance.StartNewGame();
        GameStateController.Instance.ChangeState(GameState.Playing);
    }

    private void OnContinueClicked()
    {
        StageManager.Instance.RestartCurrentStage();
        GameStateController.Instance.ChangeState(GameState.Playing);
    }


    private void OnExitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
