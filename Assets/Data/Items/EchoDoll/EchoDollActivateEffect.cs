using UnityEngine;

[CreateAssetMenu(menuName = "Item/Effects/EchoDoll Activate")]
public class EchoDollActivateEffect : ItemEffect
{
    public override void ActivateEffect(GameplayManager gm)
    {
        if (EchoDollManager.Instance == null || EchoDollManager.Instance.CurrentDoll == null)
        {
            Debug.LogWarning("[EchoDollActivateEffect] No placed doll to activate.");
            return;
        }

        bool ok = EchoDollManager.Instance.EnableDollMovement();
        if (ok)
            Debug.Log("[EchoDollActivateEffect] Doll movement enabled.");
        else
            Debug.LogWarning("[EchoDollActivateEffect] Failed to enable doll movement.");
    }
}
