using System;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event Action OnLevelStarted;
    public event Action OnLevelCompleted;
    public event Action OnLevelFailed;

    [SerializeField] private int levelIndex = 0;

    private bool levelActive = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        levelActive = true;
        Debug.Log($"Level {levelIndex} started.");

        // Resetuj ili inicijalizuj sve potrebne stvari ovde (npr pozicioniraj playera, spawn iteme...)

        OnLevelStarted?.Invoke();
    }

    public void CompleteLevel()
    {
        if (!levelActive) return;

        levelActive = false;
        Debug.Log($"Level {levelIndex} completed!");
        OnLevelCompleted?.Invoke();
    }

    public void FailLevel()
    {
        if (!levelActive) return;

        levelActive = false;
        Debug.Log($"Level {levelIndex} failed!");
        OnLevelFailed?.Invoke();
    }
}
