using UnityEngine;

public class GatewayTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered)
            return;

        if (collision.CompareTag("Player"))
        {
            if (GameplayManager.Instance.Inventory.HasKey)
            {
                isTriggered = true;
                Debug.Log("Player advances!");
                AudioManager.Instance.PlaySFX(6);
                GameplayManager.Instance.Inventory.RemoveKey();
                StageManager.Instance.AdvanceStage();
            }
            else
            {
                Debug.Log("You need a key to pass!");
            }
        }
    }
}
