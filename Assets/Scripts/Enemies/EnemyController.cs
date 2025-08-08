using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemyController : GridMover
{
    [SerializeField] private List<Vector3Int> patrolPoints; 
    [SerializeField] private float moveSpeed = 1f; 

    private int targetIndex = 0;

    protected override void Start()
    {
        base.Start();

        // Postavi na prvu patrolnu tacku
        if (patrolPoints.Count > 0)
        {
            transform.position = groundTilemap.GetCellCenterWorld(patrolPoints[0]);
        }

        // Startuj patrolu
        if (moveSpeed > 0f)
        {
            float interval = 1f / moveSpeed;
            InvokeRepeating(nameof(Patrol), interval, interval);
        }
    }


    private void Patrol()
    {
        if (patrolPoints.Count == 0) return;

        Vector3 targetWorldPos = groundTilemap.GetCellCenterWorld(patrolPoints[targetIndex]);
        Vector2Int moveDirection = GetMoveDirection(targetWorldPos);

        if (moveDirection != Vector2Int.zero && CanMove(moveDirection))
        {
            Move(moveDirection);
        }

        // Kada stigne na cilj
        if (Vector3.Distance(transform.position, targetWorldPos) < 0.1f)
        {
            targetIndex = (targetIndex + 1) % patrolPoints.Count;
        }
    }

    private Vector2Int GetMoveDirection(Vector3 targetWorldPos)
    {
        Vector3 current = transform.position;
        Vector3 diff = targetWorldPos - current;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return new Vector2Int((int)Mathf.Sign(diff.x), 0);
        }
        else if (Mathf.Abs(diff.y) > 0.01f)
        {
            return new Vector2Int(0, (int)Mathf.Sign(diff.y));
        }
        else
        {
            return Vector2Int.zero;
        }
    }
}
