using UnityEngine;
using System;

public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver
}

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        CurrentState = newState;
        Debug.Log($"[GameManager] Game state changed to: {newState}");
        OnGameStateChanged?.Invoke(CurrentState);
    }
}
