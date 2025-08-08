using UnityEngine;

public class KeyTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered)
            return;

        if (collision.CompareTag("Player"))
        {
            isTriggered = true;
            GameplayManager.Instance.Inventory.GainKey();
            Debug.Log("Key gained!");

            Destroy(gameObject);
        }
    }
}
