using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pressure plate: trigger collider. When Player / Enemy / Boulder stands on it,
/// it notifies linked doors to open. When all occupants leave, it notifies doors to close.
/// 
/// Usage:
/// - Add a 2D Collider (Box/Circle) to the plate prefab and set isTrigger = true.
/// - Attach this script and assign doors (array) in inspector OR set doorTag to auto-find.
/// - Optionally toggle which tags/components are accepted (Player, Enemy, BoulderComponent).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PressurePlate : MonoBehaviour
{
    [Tooltip("Doors directly linked in inspector.")]
    public DoorController[] linkedDoors;

    [Tooltip("Optional: if no doors in linkedDoors, find doors by tag.")]
    public string doorsTag = ""; // e.g. "Door" if you want to use tags

    [Tooltip("Accept Player-tagged objects?")]
    public bool acceptPlayer = true;

    [Tooltip("Accept Enemy-tagged objects?")]
    public bool acceptEnemy = true;

    [Tooltip("Accept objects that have Boulder component?")]
    public bool acceptBoulderComponent = true;

    // set of colliders currently pressing the plate (to avoid duplicates)
    private HashSet<Collider2D> occupants = new HashSet<Collider2D>();

    private void Start()
    {
        // if linkedDoors empty and doorsTag provided, auto-find doors with tag
        if ((linkedDoors == null || linkedDoors.Length == 0) && !string.IsNullOrEmpty(doorsTag))
        {
            var found = GameObject.FindGameObjectsWithTag(doorsTag);
            var list = new List<DoorController>();
            foreach (var go in found)
            {
                var d = go.GetComponent<DoorController>();
                if (d != null) list.Add(d);
            }
            linkedDoors = list.ToArray();
        }

        // Initially check overlaps (in case something starts on the plate)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // sample colliders at center and small radius
            var hits = Physics2D.OverlapPointAll(transform.position);
            foreach (var h in hits)
            {
                if (IsValidOccupant(h))
                    AddOccupant(h);
            }
        }
    }

    private bool IsValidOccupant(Collider2D other)
    {
        if (other == null) return false;

        if (acceptPlayer && other.CompareTag("Player")) return true;
        if (acceptEnemy && other.CompareTag("Enemy")) return true;
        if (acceptBoulderComponent && other.GetComponent<Boulder>() != null) return true;

        // optionally accept specific tags in future
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidOccupant(other)) return;
        AddOccupant(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidOccupant(other)) return;
        RemoveOccupant(other);
    }

    private void AddOccupant(Collider2D c)
    {
        if (occupants.Add(c))
        {
            // first occupant? notify doors if previously empty
            if (occupants.Count == 1)
                NotifyDoorsOpen();
        }
    }

    private void RemoveOccupant(Collider2D c)
    {
        if (occupants.Remove(c))
        {
            if (occupants.Count == 0)
                NotifyDoorsClose();
        }
    }

    private void NotifyDoorsOpen()
    {
        if (linkedDoors == null || linkedDoors.Length == 0) return;
        foreach (var d in linkedDoors)
        {
            if (d != null) d.RequestOpen();
        }
    }

    private void NotifyDoorsClose()
    {
        if (linkedDoors == null || linkedDoors.Length == 0) return;
        foreach (var d in linkedDoors)
        {
            if (d != null) d.RequestClose();
        }
    }

    // public helper to add a door at runtime
    public void AddLinkedDoor(DoorController door)
    {
        var list = new List<DoorController>(linkedDoors ?? new DoorController[0]);
        if (!list.Contains(door))
        {
            list.Add(door);
            linkedDoors = list.ToArray();
        }
    }
}
