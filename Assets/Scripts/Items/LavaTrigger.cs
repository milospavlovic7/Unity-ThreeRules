using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LavaTrigger : MonoBehaviour
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
        if (abilities != null && abilities.CanWalkOnLava)
        {
            // Player can walk on lava — do nothing
            Debug.Log("[LavaTrigger] Player walked on lava safely (has boots).");
            return;
        }

        // Player does not have boots — die / go to GameOver
        Debug.Log("[LavaTrigger] Player touched lava without boots — death!");

        // Trigger a death / gameover. Choose behavior:
        // Option A: send player to GameOver state:
        GameStateController.Instance.ChangeState(GameState.GameOver);
        AudioManager.Instance.PlaySFX(2);


        // Option B: immediate restart stage:
        // StageManager.Instance?.RestartCurrentStage();
        // GameStateController.Instance?.ChangeState(GameState.Playing);
    }
}
