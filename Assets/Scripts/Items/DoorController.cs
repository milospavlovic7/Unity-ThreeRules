using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DoorController : MonoBehaviour
{
    [Tooltip("Sprite when door is closed (default).")]
    public Sprite closedSprite;

    [Tooltip("Sprite when door is open (optional). If null, sprite won't change.")]
    public Sprite openSprite;

    [Tooltip("If true, doors start opened.")]
    public bool startOpened = false;

    private Collider2D doorCollider;
    private SpriteRenderer spriteRenderer;
    private int openRequests = 0;

    // Dodato: UnityEvents za otvaranje i zatvaranje vrata
    public UnityEngine.Events.UnityEvent OnDoorOpened;
    public UnityEngine.Events.UnityEvent OnDoorClosed;

    private void Awake()
    {
        doorCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (doorCollider == null)
            Debug.LogWarning($"[DoorController] No Collider2D found on door '{name}'. Door won't block movement.");

        if (spriteRenderer != null && closedSprite == null)
            closedSprite = spriteRenderer.sprite;

        if (startOpened)
            SetOpenState(true);
        else
            SetOpenState(false);
    }

    public void RequestOpen()
    {
        openRequests = Mathf.Max(0, openRequests) + 1;
        if (openRequests == 1)
            SetOpenState(true);
    }

    public void RequestClose()
    {
        openRequests = Mathf.Max(0, openRequests - 1);
        if (openRequests == 0)
            SetOpenState(false);
    }

    private void SetOpenState(bool open)
    {
        if (doorCollider != null)
            doorCollider.enabled = !open;

        if (spriteRenderer != null)
        {
            if (open && openSprite != null)
                spriteRenderer.sprite = openSprite;
            else if (!open && closedSprite != null)
                spriteRenderer.sprite = closedSprite;
        }

        // Pozivanje UnityEvents za bolje povezivanje sa UI/audio sistemom
        if (open)
        {
            OnDoorOpened?.Invoke();
            AudioManager.Instance.PlaySFX(3);
        }
        else
        {
            OnDoorClosed?.Invoke();
            AudioManager.Instance.PlaySFX(3);
        }
    }

    public void ForceClose()
    {
        openRequests = 0;
        SetOpenState(false);
    }

    public void ForceOpen()
    {
        openRequests = Mathf.Max(0, openRequests) + 1;
        SetOpenState(true);
    }
}
