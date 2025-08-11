using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Item/Effects/EchoDoll Place")]
public class EchoDollPlaceEffect : ItemEffect
{
    [Tooltip("Prefab of the doll to instantiate (should contain EchoDollController).")]
    public GameObject dollPrefab;

    [Tooltip("The ItemData to add to inventory AFTER placing the doll (activator remote).")]
    public ItemData activatorItemData;

    [Tooltip("Try to snap to ground tilemap cells.")]
    public bool tryUseGroundTilemap = true;

    public bool snapToIntegerGridFallback = true;

    public override void ActivateEffect(GameplayManager gm)
    {
        if (gm == null)
        {
            Debug.LogWarning("[EchoDollPlaceEffect] GameplayManager null.");
            return;
        }

        if (dollPrefab == null)
        {
            Debug.LogWarning("[EchoDollPlaceEffect] dollPrefab not set!");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[EchoDollPlaceEffect] Player not found.");
            return;
        }

        Vector3 spawnWorld = player.transform.position;

        if (tryUseGroundTilemap)
        {
            var tilemaps = GameObject.FindObjectsOfType<Tilemap>();
            Tilemap ground = null;
            foreach (var t in tilemaps)
            {
                if (t.gameObject.name == "GroundTilemap" || t.gameObject.name.ToLower().Contains("ground"))
                {
                    ground = t;
                    break;
                }
            }

            if (ground != null)
            {
                Vector3Int cell = ground.WorldToCell(player.transform.position);
                spawnWorld = ground.GetCellCenterWorld(cell);
            }
            else if (snapToIntegerGridFallback)
            {
                spawnWorld = SnapToIntegerCell(player.transform.position);
            }
        }
        else if (snapToIntegerGridFallback)
        {
            spawnWorld = SnapToIntegerCell(player.transform.position);
        }

        // Parent under Level root if present
        GameObject levelRoot = GameObject.FindGameObjectWithTag("Level");
        var go = (levelRoot != null)
            ? Object.Instantiate(dollPrefab, spawnWorld, Quaternion.identity, levelRoot.transform)
            : Object.Instantiate(dollPrefab, spawnWorld, Quaternion.identity);

        // tag as Player (so enemies treat it like player for collisions)
        go.tag = "Player";

        // Ensure EchoDollManager exists
        if (EchoDollManager.Instance == null)
        {
            var mg = new GameObject("EchoDollManager");
            mg.AddComponent<EchoDollManager>();
        }
        EchoDollManager.Instance.RegisterDoll(go);

        // IMPORTANT: disable doll movement component immediately so it doesn't react to input yet
        var dollCtrl = go.GetComponent<EchoDollController>();
        if (dollCtrl != null)
        {
            // disable the script (this triggers OnDisable in PlayerController and will properly disable its input actions)
            dollCtrl.enabled = false;
        }

        // Add the activator item to inventory so player can press space again to activate the doll
        if (activatorItemData != null)
        {
            int usedIndex = -1;
            if (gm?.Inventory != null)
                usedIndex = gm.Inventory.LastUsedItemIndex;

            // start a coroutine on GameplayManager to wait one frame, then do the replacement/add.
            gm.StartCoroutine(PlaceActivatorNextFrame(gm, usedIndex));
        }



        Debug.Log("[EchoDollPlaceEffect] Doll placed at " + spawnWorld);
    }

    private System.Collections.IEnumerator PlaceActivatorNextFrame(GameplayManager gm, int usedIndex)
    {
        // čekamo jedan frame da UseItemAt završi svoj posao (npr. briše item ako removeAfterUse == true)
        yield return null;

        var ui = PlayingUIManager.Instance;

        if (gm == null || gm.Inventory == null)
        {
            Debug.LogWarning("[EchoDollPlaceEffect] Cannot place activator: GameplayManager or Inventory null on next frame.");
            yield break;
        }

        if (usedIndex >= 0)
        {
            // Zameni u istom slotu (ovo će vratiti activator čak i ako je original igračev item uklonjen)
            gm.Inventory.ReplaceItem(usedIndex, activatorItemData);
            ui?.SelectItemByIndex(usedIndex);
            Debug.Log($"[EchoDollPlaceEffect] (deferred) Activator replaced in same slot {usedIndex}.");
        }
        else
        {
            // fallback: ubaci u prvi slobodan slot
            int idx = gm.Inventory.AddItem(activatorItemData);
            if (idx >= 0)
            {
                ui?.SelectItemByIndex(idx);
                Debug.Log($"[EchoDollPlaceEffect] (deferred) Activator added into free slot {idx}.");
            }
            else
            {
                Debug.LogWarning("[EchoDollPlaceEffect] (deferred) Inventory full; activator not added. Consider opening replace UI.");
                // opcionalno: gm.Inventory.ReplaceItem(0, activatorItemData); ui?.SelectItemByIndex(0);
            }
        }
    }


    private Vector3 SnapToIntegerCell(Vector3 world)
    {
        return new Vector3(Mathf.Round(world.x), Mathf.Round(world.y), world.z);
    }
}
