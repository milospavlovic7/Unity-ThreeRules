using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class GridMover : MonoBehaviour
{
    [SerializeField] protected Tilemap groundTilemap;
    [SerializeField] protected Tilemap collisionTilemap;

    protected virtual void Start()
    {
        SnapToGrid();
    }

    protected void SnapToGrid()
    {
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);
        transform.position = groundTilemap.GetCellCenterWorld(cellPos);
    }

    protected bool CanMove(Vector2 direction)
    {
        Vector3 targetPos = transform.position + (Vector3)direction;
        Vector3Int gridPosition = groundTilemap.WorldToCell(targetPos);

        return groundTilemap.HasTile(gridPosition) && !collisionTilemap.HasTile(gridPosition);
    }

    protected virtual bool Move(Vector2 direction)
    {
        if (CanMove(direction))
        {
            transform.position += (Vector3)direction;
            return true;
        }
        return false;
    }
}
