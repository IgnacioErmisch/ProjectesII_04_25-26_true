using UnityEngine;
using System.Collections.Generic;

public class AntiCloneZone : MonoBehaviour
{
   
    [SerializeField] private bool visualizeZone = true;
    [SerializeField] private Color zoneColor = new Color(1f, 0f, 0f, 0.3f);
    private static HashSet<AntiCloneZone> allZones = new HashSet<AntiCloneZone>();
    private HashSet<GameObject> clonesInZone = new HashSet<GameObject>();
    [SerializeField] private SoundManager soundManager;
    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BigClone") || collision.CompareTag("SmallClone"))

        {
            GameObject clone = collision.gameObject;
            clonesInZone.Add(clone);

            DespawnClone(clone);
            clonesInZone.Remove(collision.gameObject);
        }
    }

    private void DespawnClone(GameObject clone)
    {
      
        CloneSpawner[] spawners = FindObjectsByType<CloneSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var spawner in spawners)
        {
            if (spawner.GetCurrentClone() == clone)
            {
                spawner.TryDespawnClone();
                break;
            }
        }

        soundManager.PlaySFX(soundManager.antiCloneZone);
    }

    public bool IsPositionInAntiCloneZone(Vector3 position)
    {
        foreach (var zone in allZones)
        {
            Collider zoneCollider = zone.GetComponent<Collider>();
            if (zoneCollider != null && zoneCollider.ClosestPoint(position) == position)
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!visualizeZone) return;

        Gizmos.color = zoneColor;

        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
            Gizmos.DrawWireCube(box.offset, box.size);
        }
    }
}
