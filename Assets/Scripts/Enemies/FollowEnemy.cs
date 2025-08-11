using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FollowAxis { X, Y, Both }

[RequireComponent(typeof(Collider2D))]
public class FollowEnemy : GridMover
{
    [Header("Follow settings")]
    [SerializeField] private FollowAxis followAxis = FollowAxis.Both;

    [Tooltip("Ako je true -> neprijatelj prati igrača i bez aktivne abiliti. Ako je false -> zahteva abiliti na igraču.")]
    [SerializeField] private bool ignoreAbilityRequirement = false;

    [SerializeField] private string groundTilemapNameFallback = "GroundTilemap";

    [SerializeField] private bool debugMode = false;  // Dodat flag za uključivanje/isključivanje logova

    private bool subscribed = false;

    private void OnEnable()
    {
        TrySubscribe();
    }

    private void OnDisable()
    {
        if (subscribed && GameplayManager.Instance != null)
        {
            GameplayManager.Instance.OnPlayerMoved -= OnPlayerMoved;
            subscribed = false;
            if (debugMode) Debug.Log($"[FollowEnemy] Unsubscribed from OnPlayerMoved ({gameObject.name}).");
        }
    }

    private void TrySubscribe()
    {
        if (GameplayManager.Instance == null)
        {
            StartCoroutine(WaitAndSubscribe());
            return;
        }

        if (!subscribed)
        {
            GameplayManager.Instance.OnPlayerMoved += OnPlayerMoved;
            subscribed = true;
            if (debugMode) Debug.Log($"[FollowEnemy] Subscribed to OnPlayerMoved ({gameObject.name}).");
        }
    }

    private IEnumerator WaitAndSubscribe()
    {
        float timeout = 2f;
        float t = 0f;
        while (GameplayManager.Instance == null && t < timeout)
        {
            t += Time.deltaTime;
            yield return null;
        }

        if (GameplayManager.Instance != null)
        {
            TrySubscribe();
        }
        else
        {
            Debug.LogWarning($"[FollowEnemy] GameplayManager.Instance not found within {timeout}s. ({gameObject.name})");
        }
    }

    protected override void Start()
    {
        base.Start();

        if (groundTilemap == null)
        {
            var maps = GameObject.FindObjectsOfType<Tilemap>();
            foreach (var m in maps)
            {
                if (m.gameObject.name == groundTilemapNameFallback || m.gameObject.name.ToLower().Contains("ground"))
                {
                    groundTilemap = m;
                    if (debugMode) Debug.Log($"[FollowEnemy] Auto-assigned groundTilemap for {gameObject.name} -> {m.gameObject.name}");
                    break;
                }
            }
        }
    }

    private void OnPlayerMoved(Vector2Int playerMoveDir)
    {
        if (GameStateController.Instance != null && GameStateController.Instance.CurrentState != GameState.Playing)
            return;

        if (!ignoreAbilityRequirement)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning($"[FollowEnemy] Player not found ({gameObject.name}).");
                return;
            }

            var abilities = player.GetComponent<PlayerAbilities>();
            if (abilities == null || !abilities.FollowEnemiesEnabled)
            {
                if (debugMode) Debug.Log($"[FollowEnemy] Player doesn't have FollowEnemiesEnabled; skipping. ({gameObject.name})");
                return;
            }
        }

        Vector2Int move = Vector2Int.zero;

        switch (followAxis)
        {
            case FollowAxis.X:
                if (playerMoveDir.x != 0)
                    move = new Vector2Int(Mathf.Clamp(playerMoveDir.x, -1, 1), 0);
                break;
            case FollowAxis.Y:
                if (playerMoveDir.y != 0)
                    move = new Vector2Int(0, Mathf.Clamp(playerMoveDir.y, -1, 1));
                break;
            case FollowAxis.Both:
                if (playerMoveDir != Vector2Int.zero)
                    move = new Vector2Int(Mathf.Clamp(playerMoveDir.x, -1, 1), Mathf.Clamp(playerMoveDir.y, -1, 1));
                break;
        }

        if (move == Vector2Int.zero)
        {
            if (debugMode) Debug.Log($"[FollowEnemy] No relevant move for axis ({followAxis}). ({gameObject.name})");
            return;
        }

        if (!CanMove((Vector2)move))
        {
            Vector3 targetPos = transform.position + new Vector3(move.x, move.y, 0);
            Vector3Int gridPos = groundTilemap != null ? groundTilemap.WorldToCell(targetPos) : Vector3Int.zero;

            if (groundTilemap == null)
            {
                Debug.LogWarning($"[FollowEnemy] groundTilemap is null on {gameObject.name} — assign it in Inspector.");
            }
            else if (!groundTilemap.HasTile(gridPos))
            {
                if (debugMode) Debug.Log($"[FollowEnemy] Can't move: no ground tile at {gridPos} for {gameObject.name}.");
            }
            else if (collisionTilemap != null && collisionTilemap.HasTile(gridPos))
            {
                if (debugMode) Debug.Log($"[FollowEnemy] Can't move: collision tile at {gridPos} for {gameObject.name}.");
            }
            else
            {
                Vector3 cellCenter = groundTilemap.GetCellCenterWorld(gridPos);
                Collider2D[] hits = Physics2D.OverlapPointAll(cellCenter);
                foreach (var c in hits)
                {
                    if (c == null) continue;
                    if (c.gameObject == this.gameObject) continue;
                    if (c.isTrigger) continue;
                    if (debugMode) Debug.Log($"[FollowEnemy] Can't move: blocked by non-trigger collider '{c.gameObject.name}' on {gameObject.name}.");
                }
            }
            return;
        }

        bool moved = Move((Vector2)move);
        if (debugMode) Debug.Log($"[FollowEnemy] Move attempt for {gameObject.name} -> {move} result: {moved}");
    }
}
