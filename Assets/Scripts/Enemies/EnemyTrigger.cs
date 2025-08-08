using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player killed!");
            GameStateController.Instance.ChangeState(GameState.GameOver);
        }
    }
}
