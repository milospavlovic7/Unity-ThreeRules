using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// EchoDollController
/// - nasleđuje PlayerController (koristi istu movement & push logiku)
/// - automatski pronalazi Ground i Collision tilemap iz scene pre Start()
/// - osigurava da prefab ima non-trigger Collider2D i Kinematic Rigidbody2D
/// - omogućava EnableMovement() (manager poziva kada se aktivator upotrebi)
/// - uklanja referencu iz EchoDollManager prilikom uništenja
/// </summary>
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EchoDollController : PlayerController
{
    [Tooltip("Ako je true, automatski pokušava da pronađe Ground i Collision Tilemap iz scene pre Start().")]
    public bool autoFindTilemaps = true;

    [Tooltip("Ako true, pokušava da uskladi layer sa igračem (kad postoji).")]
    public bool matchPlayerLayer = true;

    private void Awake()
    {
        // 1) Auto-find Ground i Collision tilemap pre nego što GridMover.Start() pozove SnapToGrid()
        if (autoFindTilemaps)
        {
            try
            {
                var allTilemaps = GameObject.FindObjectsOfType<Tilemap>();
                foreach (var t in allTilemaps)
                {
                    if (t == null) continue;
                    string lowerName = t.gameObject.name.ToLowerInvariant();
                    if (groundTilemap == null && lowerName == "ground")
                        groundTilemap = t;
                    else if (collisionTilemap == null && lowerName == "collision")
                        collisionTilemap = t;
                }
            }
            catch { /* ignore errors */ }
        }

        // 2) Ensure collider is non-trigger
        var col = GetComponent<Collider2D>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider2D>();
        if (col.isTrigger)
            col.isTrigger = false;

        // 3) Ensure Rigidbody2D exists and is Kinematic
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        if (rb.bodyType != RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Kinematic;

        // 4) Tag & layer setup
        gameObject.tag = "Player";
        if (matchPlayerLayer)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && player != this.gameObject)
                gameObject.layer = player.layer;
        }
    }

    /// <summary>
    /// Called by EchoDollManager/EchoDollActivateEffect to enable movement input on doll.
    /// </summary>
    public void EnableMovement()
    {
        this.enabled = true;
    }

    private void OnDestroy()
    {
        if (EchoDollManager.Instance != null && EchoDollManager.Instance.CurrentDoll == this.gameObject)
            EchoDollManager.Instance.UnregisterDoll();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // GridMover.CanMove već blokira ulazak u zauzete ćelije
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        if (other.GetComponent<EnemyController>() != null || other.CompareTag("Enemy"))
        {
            Debug.Log("[EchoDollController] Doll hit by enemy -> GAME OVER");
            if (GameStateController.Instance != null)
                GameStateController.Instance.ChangeState(GameState.GameOver);
        }
    }
}
