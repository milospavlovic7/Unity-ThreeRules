using UnityEngine;
using UnityEngine.SceneManagement;

public class Trap : MonoBehaviour
{
    [Tooltip("Ako je true - trap se uništava nakon što ubije prvog neprijatelja.")]
    public bool destroyAfterKill = true;

    private void OnEnable()
    {
        // Slušamo kada se učita nova scena (nivo)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Kad se učita novi nivo, brišemo trap
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        
        // Pokušaj da nađe EnemyController skriptu
        var enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            Debug.Log("[Trap] Enemy killed by trap (EnemyController).");
            Destroy(enemy.gameObject);
            AudioManager.Instance.PlaySFX(12);

            if (destroyAfterKill)
                Destroy(gameObject);
            return;
        }

        // Ako nema skripte, proveri po tagu
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("[Trap] Enemy killed by trap (tag check).");
            Destroy(collision.gameObject);
            AudioManager.Instance.PlaySFX(12);



            if (destroyAfterKill)
                Destroy(gameObject);
        }
    }
}
