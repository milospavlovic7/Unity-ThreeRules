using UnityEngine;

[CreateAssetMenu(menuName = "Item/Effects/Enable Follow Enemies")]
public class EnableFollowEnemiesEffect : ItemEffect
{
    // Ovo je čisto toggle effect: OnPickup enable-uje FollowEnemiesEnabled na PlayerAbilities,
    // OnRemove ga disable-uje.

    public override void OnPickup(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[EnableFollowEnemiesEffect] Player not found when picking up item.");
            return;
        }

        var abilities = player.GetComponent<PlayerAbilities>();
        if (abilities == null)
        {
            Debug.LogWarning("[EnableFollowEnemiesEffect] PlayerAbilities component missing on Player.");
            return;
        }

        abilities.FollowEnemiesEnabled = true;
        Debug.Log("[EnableFollowEnemiesEffect] FollowEnemiesEnabled = true");
    }

    public override void OnRemove(GameplayManager gm)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[EnableFollowEnemiesEffect] Player not found when removing item.");
            return;
        }

        var abilities = player.GetComponent<PlayerAbilities>();
        if (abilities == null)
        {
            Debug.LogWarning("[EnableFollowEnemiesEffect] PlayerAbilities component missing on Player.");
            return;
        }

        abilities.FollowEnemiesEnabled = false;
        Debug.Log("[EnableFollowEnemiesEffect] FollowEnemiesEnabled = false");
    }
}
