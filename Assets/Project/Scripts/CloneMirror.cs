using UnityEngine;

public class CloneMirror : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CloneSpawner smallCloneSpawner;
    [SerializeField] private CloneSpawner bigCloneSpawner;
    [SerializeField] private SwitchInterface switchInterface;

    [Header("Prefabs")]
    [SerializeField] private GameObject smallClonePrefab;
    [SerializeField] private GameObject bigClonePrefab;

    [Header("Exit Point")]
    [SerializeField] private Transform mirrorExitPoint;

    private bool isOnCooldown = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOnCooldown) return;

        if (other.GetComponent<SmallCloneController>() != null)
            HandleMirror(isSmallEntering: true);
        else if (other.GetComponent<BigCloneController>() != null)
            HandleMirror(isSmallEntering: false);
    }

    private void HandleMirror(bool isSmallEntering)
    {
        isOnCooldown = true;

        CloneSpawner spawnerToKill = isSmallEntering ? smallCloneSpawner : bigCloneSpawner;
        CloneSpawner spawnerToActivate = isSmallEntering ? bigCloneSpawner : smallCloneSpawner;

        spawnerToKill.TryDespawnClone();

        if (isSmallEntering)
            switchInterface.SelectBigClone();
        else
            switchInterface.SelectSmallClone();

        GameObject prefabToSpawn = isSmallEntering ? bigClonePrefab : smallClonePrefab;
        Vector3 spawnPos = mirrorExitPoint.position;
        if (isSmallEntering) spawnPos += Vector3.up; 

        GameObject newClone = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);

     
        spawnerToActivate.RegisterExternalClone(newClone, !isSmallEntering);

    
        Invoke(nameof(ResetCooldown), 0.5f);
    }

    private void ResetCooldown() => isOnCooldown = false;
}
