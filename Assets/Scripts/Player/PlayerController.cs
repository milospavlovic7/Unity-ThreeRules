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
            Move(NormalizeInput(movementInput));
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
