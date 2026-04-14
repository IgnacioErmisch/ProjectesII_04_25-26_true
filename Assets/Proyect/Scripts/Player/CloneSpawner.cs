using Unity.VisualScripting;
using UnityEngine;

public class CloneSpawner : MonoBehaviour
{
    [SerializeField] private EnergyController energyController;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private CinemachineSingleton cinemachineSingleton;
    [SerializeField] private SwitchInterface switchInterface;
    [SerializeField] private GameObject cloneSmallPrefab;
    [SerializeField] private GameObject cloneBigPrefab;
    [SerializeField] private bool isSmallClone = true;
    [SerializeField] private CloneSpawner[] spawners;
    [SerializeField] private Transform cloneSpawnPointPrincipal;
    [SerializeField] private Transform cloneSpawnPointPrincipalUp;
    [SerializeField] private Transform cloneSpawnPointSecondary;
    
    SoundManager soundManager;
    public Camera playerCamera;
    private GameObject currentClone;
    public bool cloneActive = false;
    private bool canSpawnBigClone = true;
    public LayerMask groundLayer;
    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    public bool TrySpawnClone()
    {
       if(Time.timeScale == 0) return false;

        foreach (var spawner in spawners)
        {
            if (spawner.cloneActive)
                return false;
        }

        if (cloneActive)
            return false;

        Vector3 spawnPosition = cloneSpawnPointPrincipal.position;
          
        if (!energyController.TryConsumeInitialCost(isSmallClone))
            return false;

        Vector3 direction = spawnPosition - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, groundLayer);
        if(!Physics2D.Raycast(transform.position, direction, distance, groundLayer))
        {
            if(switchInterface.IsBigCloneSelected)
            {
                if(CheckColisionSpawn()) return false;
                if(!canSpawnBigClone) return false;

                currentClone = Instantiate(cloneBigPrefab, spawnPosition + Vector3.up, Quaternion.identity);
                energyController.RegisterClone(currentClone, isSmallClone);
            }
            else if (!switchInterface.IsBigCloneSelected)
            { 

                currentClone = Instantiate(cloneSmallPrefab, spawnPosition , Quaternion.identity);
                energyController.RegisterClone(currentClone, isSmallClone);
            }
            cloneActive = true;
            playerCamera.transform.SetParent(currentClone.transform);
            playerCamera.transform.localPosition = new Vector3(2, 1, -5);
            perspectiveSwitch.SwitchToClone();
            soundManager.PlaySFX(soundManager.spawnClon);

            return true;
        }
        else
        {
            Vector3 spawnPositionSecondary = cloneSpawnPointSecondary.position;
            
            if (switchInterface.IsBigCloneSelected)
            { 
                currentClone = Instantiate(cloneBigPrefab, spawnPositionSecondary + Vector3.up, Quaternion.identity);
                energyController.RegisterClone(currentClone, isSmallClone);
            }
            else if(!switchInterface.IsBigCloneSelected)
            { 
                currentClone = Instantiate(cloneSmallPrefab, spawnPositionSecondary, Quaternion.identity);
                energyController.RegisterClone(currentClone, isSmallClone);
            }
            cloneActive = true;
            playerCamera.transform.SetParent(currentClone.transform);
            playerCamera.transform.localPosition = new Vector3(2, 1, -5);
            perspectiveSwitch.SwitchToClone();
            
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 spawnPosition = cloneSpawnPointPrincipal.position;
        Gizmos.DrawLine(transform.position, spawnPosition);
        Vector3 direction = spawnPosition - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, groundLayer);
        if (hit.rigidbody != null)
        {
            Gizmos.DrawSphere((Vector3)hit.point, 0.2f);
        }
        else
        {
            Gizmos.DrawSphere(spawnPosition, 0.2f);
        }
        Vector3 spawnPosition2 = cloneSpawnPointPrincipalUp.position;
        Gizmos.DrawLine(transform.position, spawnPosition2);
        Vector3 direction2 = spawnPosition2 - transform.position;
        float distance2 = direction2.magnitude;
        direction2.Normalize();
        RaycastHit2D hit2 = Physics2D.Raycast(transform.position, direction2, distance2, groundLayer);
        if (hit2.rigidbody != null)
        {
            Gizmos.DrawSphere((Vector3)hit2.point, 0.2f);
        }
        else
        {
            Gizmos.DrawSphere(spawnPosition2, 0.2f);
        }
        
    }

    bool CheckColisionSpawn()
    {
        Vector3 spawnPosition = cloneSpawnPointPrincipalUp.position;
        Vector3 direction = spawnPosition - transform.position;
        float distance = direction.magnitude;
        direction.Normalize();
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, groundLayer);
        if (hit.rigidbody != null)
            return true;
        else
            return false;
    }
    public bool TryDespawnClone()
    {
        if(Time.timeScale == 0) return false;

        if (!cloneActive)
            return false;

        if (currentClone != null)
        {        
            playerCamera.transform.SetParent(gameObject.transform);
            playerCamera.transform.localPosition = new Vector3(2, 2, -5);
            perspectiveSwitch.SwitchToPlayer();
            energyController.UnregisterClone(currentClone);
            Destroy(currentClone);
            currentClone = null;
            cloneActive = false;
            return true;
        }

        return false;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {         
            canSpawnBigClone = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        canSpawnBigClone = true;
    }

    public GameObject GetCurrentClone()
    {
        return currentClone;
    }
    public bool GetActiveClone()
    {
        return cloneActive;
    }
    public void RegisterExternalClone(GameObject clone, bool isSmall)
    {
        currentClone = clone;
        cloneActive = true;      
        energyController.RegisterClone(clone, isSmall);
        playerCamera.transform.SetParent(clone.transform);
        playerCamera.transform.localPosition = new Vector3(2, 1, -5);   
        perspectiveSwitch.SwitchToClone();
    }
}