using System.Collections.Generic;
using UnityEngine;

public class SmallCloneSpikes : MonoBehaviour
{
    private HashSet<GameObject> clonesInZone = new HashSet<GameObject>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SmallClone"))

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
    }

}
