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
            // Ako Move vraca bool, koristi ovo da proveris da li je pomeranje uspelo
            bool moved = Move(NormalizeInput(movementInput));

            if (moved)
            {
                // Registruj potez u GameplayManageru
                if (GameplayManager.Instance != null)
                    GameplayManager.Instance.RegisterMove();
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
        yield return new WaitForSeconds(0.1f); // prilagodi po potrebi
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
