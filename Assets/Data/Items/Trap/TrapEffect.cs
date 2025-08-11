using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Item/Effects/Trap (Kills enemies)")]
public class TrapEffect : ItemEffect
{
    [Tooltip("Prefab koji će biti spawn-ovan (treba da sadrži komponentu Trap).")]
    public GameObject trapPrefab;

    [Tooltip("Ako je true, igra će pokušati da snap-uje trap na Tilemap sa imenom 'GroundTilemap'.")]
    public bool tryUseGroundTilemap = true;

    [Tooltip("Fallback: ako nema tilemap, zamka se snap-uje na najbliži ceo broj (grid).")]
    public bool snapToIntegerGridFallback = true;

    public override void ActivateEffect(GameplayManager gm)
    {
        if (trapPrefab == null)
        {
            Debug.LogWarning("[TrapEffect] trapPrefab nije postavljen!");
            return;
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("[TrapEffect] Ne mogu naći igrača za postavljanje zamke.");
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

        // Pronađi parent objekat nivoa, ako postoji
        GameObject levelRoot = GameObject.FindGameObjectWithTag("Level");

        var go = (levelRoot != null)
            ? Object.Instantiate(trapPrefab, spawnWorld, Quaternion.identity, levelRoot.transform)
            : Object.Instantiate(trapPrefab, spawnWorld, Quaternion.identity);

        Debug.Log("[TrapEffect] Trap postavljen na " + spawnWorld);
    }

    private Vector3 SnapToIntegerCell(Vector3 world)
    {
        float x = Mathf.Round(world.x);
        float y = Mathf.Round(world.y);
        return new Vector3(x, y, world.z);
    }
}
