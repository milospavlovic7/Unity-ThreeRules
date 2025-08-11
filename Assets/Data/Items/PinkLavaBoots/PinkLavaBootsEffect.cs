using UnityEngine;

[CreateAssetMenu(menuName = "Item/Effects/PinkLavaBoots (WalkOnPinkLava)")]
public class PinkLavaBootsEffect : ItemEffect
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
                abilities.CanWalkOnPinkLava = true;
                Debug.Log("[PinkLavaBootsEffect] Player can now walk on Pink lava.");
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
                abilities.CanWalkOnPinkLava = false;
                Debug.Log("[PinkLavaBootsEffect] Player lost Pink lava-walking ability.");
            }
        }
    }
}
