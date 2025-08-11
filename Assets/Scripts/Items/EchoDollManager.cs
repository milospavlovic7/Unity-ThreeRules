using UnityEngine;

/// <summary>
/// Simple manager that tracks the currently placed echo doll in the level.
/// </summary>
public class EchoDollManager : MonoBehaviour
{
    public static EchoDollManager Instance { get; private set; }

    public GameObject CurrentDoll { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterDoll(GameObject doll)
    {
        if (doll == null) return;

        if (CurrentDoll != null)
            Destroy(CurrentDoll);

        CurrentDoll = doll;
    }

    public void UnregisterDoll()
    {
        if (CurrentDoll != null)
        {
            Destroy(CurrentDoll);
            CurrentDoll = null;
        }
    }

    /// <summary>
    /// Enables movement on the doll (so it starts accepting input).
    /// </summary>
    public bool EnableDollMovement()
    {
        if (CurrentDoll == null) return false;
        var ctrl = CurrentDoll.GetComponent<EchoDollController>();
        if (ctrl == null) return false;

        ctrl.EnableMovement();
        return true;
    }
}
