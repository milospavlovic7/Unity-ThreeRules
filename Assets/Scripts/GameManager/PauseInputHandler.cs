using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    private GameInputActions inputActions;

    private void Awake()
    {
        inputActions = new GameInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.UI.Pause.performed += OnPausePerformed;
    }

    private void OnDisable()
    {
        inputActions.UI.Pause.performed -= OnPausePerformed;
        inputActions.Disable();
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        var gameStateController = GameStateController.Instance;

        if (gameStateController.CurrentState == GameState.Playing)
        {
            gameStateController.ChangeState(GameState.Paused);
        }
        else if (gameStateController.CurrentState == GameState.Paused)
        {
            gameStateController.ChangeState(GameState.Playing);
        }
        else if (gameStateController.CurrentState == GameState.GameOver)
        {
            gameStateController.ChangeState(GameState.MainMenu);
        }
    }

}
