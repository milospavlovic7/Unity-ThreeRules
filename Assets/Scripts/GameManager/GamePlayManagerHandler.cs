using UnityEngine;

public class GameplayManagerHandler : MonoBehaviour
{
    public static GameplayManagerHandler Instance { get; private set; }

    [Tooltip("Prefab za GameplayManager ako ga nema u sceni.")]
    public GameObject gameplayManagerPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public GameplayManager EnsureGameplayManager()
    {
        if (GameplayManager.Instance == null)
        {
            if (gameplayManagerPrefab == null)
            {
                Debug.LogError("[GameplayManagerHandler] GameplayManager prefab nije postavljen!");
                return null;
            }
            Instantiate(gameplayManagerPrefab);
            Debug.Log("[GameplayManagerHandler] GameplayManager instanciran.");
        }
        return GameplayManager.Instance;
    }

    public void DestroyGameplayManager()
    {
        if (GameplayManager.Instance != null)
        {
            Destroy(GameplayManager.Instance.gameObject);
            Debug.Log("[GameplayManagerHandler] GameplayManager uništen.");
        }
    }
}
