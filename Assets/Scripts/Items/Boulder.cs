using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Collider2D))]
public class Boulder : GridMover
{
    public const string BOULDER_TAG = "Boulder";

    public bool TryPush(Vector2Int cellDir)
    {
        Vector3Int currentCell = groundTilemap.WorldToCell(transform.position);
        Vector3Int targetCell = currentCell + new Vector3Int(cellDir.x, cellDir.y, 0);

        if (!groundTilemap.HasTile(targetCell))
            return false;
        if (collisionTilemap != null && collisionTilemap.HasTile(targetCell))
            return false;

        Vector3 targetWorld = groundTilemap.GetCellCenterWorld(targetCell);
        Collider2D[] hits = Physics2D.OverlapPointAll(targetWorld);
        foreach (var c in hits)
        {
            if (c == null || c.gameObject == this.gameObject) continue;
            if (c.GetComponent<Boulder>() != null) return false;
            if (c.GetComponent<EnemyController>() != null) return false;
            if (!c.isTrigger) return false;
        }

        var myCollider = GetComponent<Collider2D>();
        bool hadCollider = myCollider != null && myCollider.enabled;
        if (myCollider != null) myCollider.enabled = false;

        bool started = Move(new Vector2(cellDir.x, cellDir.y));
        if (!started)
        {
            if (myCollider != null) myCollider.enabled = hadCollider;
            return false;
        }

        StartCoroutine(ReenableColliderWhenSettled(myCollider, targetWorld));

        return true;
    }

    private System.Collections.IEnumerator ReenableColliderWhenSettled(Collider2D col, Vector3 targetWorld)
    {
        float timeout = 1.0f;
        while (Vector3.Distance(transform.position, targetWorld) > 0.01f && timeout > 0f)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        SnapToGrid();

        if (col != null)
            col.enabled = true;
    }

    protected bool Move(Vector2Int cellDir)
    {
        return Move(new Vector2(cellDir.x, cellDir.y));
    }

    // ** PREDLOG **
    // Dodati vizuelni ili zvučni feedback u TryPush na početku pomeranja, za bolju juiciness.
    // Npr. pozvati event ili emitovati zvuk.
}
