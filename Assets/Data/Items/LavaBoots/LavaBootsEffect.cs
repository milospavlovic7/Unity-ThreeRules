using UnityEngine;

[CreateAssetMenu(menuName = "Item/Effects/LavaBoots (WalkOnLava)")]
public class LavaBootsEffect : ItemEffect
{
    // When picked up, grant walk-on-lava; when removed, revoke.
    public override void OnPickup(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var abilities = player.GetComponent<PlayerAbilities>();
            if (abilities != null)
            {
                abilities.CanWalkOnLava = true;
                Debug.Log("[LavaBootsEffect] Player can now walk on lava.");
            }
        }
    }

    public override void OnRemove(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var abilities = player.GetComponent<PlayerAbilities>();
            if (abilities != null)
            {
                abilities.CanWalkOnLava = false;
                Debug.Log("[LavaBootsEffect] Player lost lava-walking ability.");
            }
        }
    }
}
