using UnityEngine;

/// <summary>
/// Base class for item effects.
/// Implement concrete effects by deriving from this.
/// </summary>
public abstract class ItemEffect : ScriptableObject
{
    /// <summary>
    /// Called when the item is picked up (added to inventory).
    /// </summary>
    public virtual void OnPickup(GameplayManager gm) { }

    /// <summary>
    /// Called when the item is removed from inventory.
    /// </summary>
    public virtual void OnRemove(GameplayManager gm) { }

    /// <summary>
    /// Called when the item is used/activated (e.g. player presses Space).
    /// </summary>
    public virtual void ActivateEffect(GameplayManager gm) { }

    /// <summary>
    /// Called each turn (if you want per-turn effects). GameplayManager should call this each move/turn.
    /// </summary>
    public virtual void OnTurnUpdate(GameplayManager gm) { }
}
