using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PinkLavaTrigger : MonoBehaviour
{
    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        var abilities = collision.GetComponent<PlayerAbilities>();
        if (abilities != null && abilities.CanWalkOnPinkLava)
        {
            // Player can walk on lava — do nothing
            Debug.Log("[PinkLavaTrigger] Player walked on pink lava safely (has boots).");
            return;
        }

        // Player does not have boots — die / go to GameOver
        Debug.Log("[PinkLavaTrigger] Player touched pink lava without boots — death!");

        // Trigger a death / gameover. Choose behavior:
        // Option A: send player to GameOver state:
        GameStateController.Instance.ChangeState(GameState.GameOver);


        // Option B: immediate restart stage:
        // StageManager.Instance?.RestartCurrentStage();
        // GameStateController.Instance?.ChangeState(GameState.Playing);
    }
}
