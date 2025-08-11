using UnityEngine;
using UnityEngine.InputSystem;

public class StoryController : MonoBehaviour
{
    private bool spacePressed = false;


    private void Update()
    {
        // Proveri da li je igrač pritisnuo taster Space
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spacePressed = true;
        }

        // Ako je space pritisnut, uništi prefab i prelazi na sledeći stage
        if (spacePressed)
        {
            StageManager.Instance.AdvanceStage();
            AudioManager.Instance.PlaySFX(7);
        }
    }
}
