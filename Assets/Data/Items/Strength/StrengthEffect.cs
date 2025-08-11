using UnityEngine;

[CreateAssetMenu(menuName = "Item/Effects/Strength (Push Boulders)")]
public class StrengthEffect : ItemEffect
{
    public override void OnPickup(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var abilities = player.GetComponent<PlayerAbilities>();
        if (abilities != null)
        {
            abilities.CanPushBoulders = true;
            Debug.Log("[StrengthEffect] Player can now push boulders.");
        }
    }

    public override void OnRemove(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var abilities = player.GetComponent<PlayerAbilities>();
        if (abilities != null)
        {
            abilities.CanPushBoulders = false;
            Debug.Log("[StrengthEffect] Player can no longer push boulders.");
        }
    }
}
