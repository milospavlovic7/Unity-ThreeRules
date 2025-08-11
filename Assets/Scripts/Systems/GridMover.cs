using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public abstract class GridMover : MonoBehaviour
{
    [SerializeField] protected Tilemap groundTilemap;
    [SerializeField] protected Tilemap collisionTilemap;
    [SerializeField] private float moveDuration = 0.12f; // vreme animacije pomeranja

    private Coroutine moveCoroutine;
    private Vector3 currentTargetPos;

    protected virtual void Start()
    {
        SnapToGrid();
    }

    protected void SnapToGrid()
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(cellPos);
    }

    /// <summary>
    /// Proverava da li je moguće pomeriti se za dati world direction (koji bi trebao biti identičan koraku mreže, npr (1,0) ili (0,1)).
    /// Sada uzima u obzir statičke tile-ove (ground / collision) i dinamičke non-trigger kolidere (boulderi, neprijatelji, zidovi).
    /// </summary>
    protected bool CanMove(Vector2 direction)
    {
        Vector3 targetPos = transform.position + (Vector3)direction;
        Vector3Int gridPosition = groundTilemap.WorldToCell(targetPos);

        // mora postojati ground tile
        if (!groundTilemap.HasTile(gridPosition))
            return false;

        // i ne sme postojati statička kolizija (npr. zidovi tilemap)
        if (collisionTilemap != null && collisionTilemap.HasTile(gridPosition))
            return false;

        // proveri dinamičke kolizije u centru ćelije
        Vector3 cellCenter = groundTilemap.GetCellCenterWorld(gridPosition);
        Collider2D[] hits = Physics2D.OverlapPointAll(cellCenter);

        foreach (var c in hits)
        {
            if (c == null) continue;
            if (c.gameObject == this.gameObject) continue; // ignoriši sebe
            if (c.isTrigger) continue; // triggeri (pickupi, zone) ne blokiraju

            // bilo koji non-trigger blokira (boulder, neprijatelj, zid sa colliderom)
            return false;
        }

        return true;
    }

    /// <summary>
    /// Pomeranje sa glatkom interpolacijom. Move koristi CanMove koja već proverava i dinamičke kolizije.
    /// </summary>
    protected virtual bool Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                transform.position = currentTargetPos;
            }

            moveCoroutine = StartCoroutine(SmoothMove(direction));
            return true;
        }
        return false;
    }

    private IEnumerator SmoothMove(Vector2 direction)
    {
        Vector3 startPos = transform.position;
        currentTargetPos = startPos + (Vector3)direction;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            transform.position = Vector3.Lerp(startPos, currentTargetPos, t);
            yield return null;
        }

        transform.position = currentTargetPos;
        moveCoroutine = null;
    }
}
