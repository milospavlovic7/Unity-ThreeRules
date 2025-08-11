using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public bool CanWalkOnLava { get; set; } = false;
    public bool CanWalkOnPinkLava { get; set; } = false;
    public bool CanPushBoulders { get; set; } = false;
    public bool FollowEnemiesEnabled { get; set; } = false;

    // dodaj druge ability-je po potrebi
}
