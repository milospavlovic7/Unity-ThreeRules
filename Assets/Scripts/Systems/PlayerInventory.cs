using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    public bool HasKey { get; private set; } = false;

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

    public void GainKey()
    {
        HasKey = true;
        Debug.Log("Key added to inventory!");
    }

    public void RemoveKey()
    {
        HasKey = false;
    }
}
