using UnityEngine;

public class GameTimeController : MonoBehaviour
{
    private void OnEnable()
    {
        GameStateController.Instance.OnGameStateChanged += HandleTimeForState;
    }

    private void OnDisable()
    {
        GameStateController.Instance.OnGameStateChanged -= HandleTimeForState;
    }

    private void HandleTimeForState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
            case GameState.MainMenu:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                break;
        }
    }
}
