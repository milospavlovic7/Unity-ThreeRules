using UnityEngine;

public class PlayerController : GridMover
{
    private PlayerMovement controls;
    private Vector2 movementInput;
    private bool canMove = true;

    protected override void Start()
    {
        base.Start();
        controls = new PlayerMovement();
        controls.Enable();
    }

    private void Update()
    {
        if (GameStateController.Instance.CurrentState != GameState.Playing)
            return;

        if (!canMove)
            return;

        movementInput = controls.Main.Movement.ReadValue<Vector2>();

        if (movementInput != Vector2.zero)
        {
            Vector2 dir = NormalizeInput(movementInput);
            Vector2Int cellDir = Vector2Int.RoundToInt(dir);

            Vector3Int playerCell = groundTilemap.WorldToCell(transform.position);
            Vector3Int targetCell = playerCell + new Vector3Int(cellDir.x, cellDir.y, 0);
            Vector3 targetWorldCenter = groundTilemap.GetCellCenterWorld(targetCell);

            Collider2D[] hits = Physics2D.OverlapPointAll(targetWorldCenter);

            Boulder foundBoulder = null;
            foreach (var h in hits)
            {
                if (h == null || h.isTrigger) continue;
                var b = h.GetComponent<Boulder>();
                if (b != null)
                {
                    foundBoulder = b;
                    break;
                }
            }

            var abilities = GetComponent<PlayerAbilities>();

            if (foundBoulder != null)
            {
                if (abilities != null && abilities.CanPushBoulders)
                {
                    bool pushed = foundBoulder.TryPush(cellDir);
                    if (pushed)
                    {
                        if (Move(cellDir))
                        {
                            GameplayManager.Instance?.NotifyPlayerMoved(cellDir);
                            AudioManager.Instance.PlaySFX(1);
                        }
                        StartCoroutine(MoveCooldown());
                        return;
                    }
                    else
                    {
                        // Predlog: Dodaj feedback za neuspešan push
                        Debug.Log("Push failed - blocked.");
                        StartCoroutine(MoveCooldown());
                        AudioManager.Instance.PlaySFX(13);
                        return;
                    }
                }
                else
                {
                    // Predlog: Dodaj feedback za nemogućnost guranja
                    Debug.Log("Cannot push boulder - ability missing.");
                    StartCoroutine(MoveCooldown());
                    return;
                }
            }

            if (Move(cellDir))
            {
                GameplayManager.Instance?.NotifyPlayerMoved(cellDir);
                AudioManager.Instance.PlaySFX(1);
            }
            StartCoroutine(MoveCooldown());
        }
    }

    private Vector2 NormalizeInput(Vector2 input)
    {
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            return new Vector2(Mathf.Sign(input.x), 0);
        else
            return new Vector2(0, Mathf.Sign(input.y));
    }

    private System.Collections.IEnumerator MoveCooldown()
    {
        canMove = false;
        yield return new WaitForSeconds(0.1f);
        canMove = true;
    }

    private void OnEnable()
    {
        if (controls == null) controls = new PlayerMovement();
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
