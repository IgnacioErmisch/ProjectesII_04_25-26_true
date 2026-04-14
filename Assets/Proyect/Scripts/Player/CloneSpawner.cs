using UnityEngine;

public class CloneSpawner : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private EnergyController energyController;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private CinemachineSingleton cinemachineSingleton;
    [SerializeField] private SwitchInterface switchInterface;

    [Header("Prefabs")]
    [SerializeField] private GameObject cloneSmallPrefab;
    [SerializeField] private GameObject cloneBigPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform cloneSpawnPointPrincipal;
    [SerializeField] private Transform cloneSpawnPointPrincipalUp;
    [SerializeField] private Transform cloneSpawnPointSecondary;

    [Header("Config")]
    [SerializeField] private bool isSmallClone = true;
    [SerializeField] private CloneSpawner[] otherSpawners;
    [SerializeField] private LayerMask groundLayer;

    [Header("Camera")]
    public Camera playerCamera;

    private Vector2 SmallCloneSize = new Vector2(0.5f, 0.5f);
    private Vector2 BigCloneSize = new Vector2(0.8f, 1.6f);

    private Vector3 CameraLocalPosition = new Vector3(2f, 1f, -5f);
    private Vector3 CameraLocalPositionPlayer = new Vector3(2f, 2f, -5f);

    private GameObject currentClone;
    private SoundManager soundManager;
   

    public bool CloneActive { get; private set; } = false;

   
    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }
   

    private void OnDrawGizmos()
    {
        DrawSpawnGizmo(cloneSpawnPointPrincipal);
        DrawSpawnGizmo(cloneSpawnPointPrincipalUp);
    }

 

    public bool TrySpawnClone()
    {
        if (!CanSpawn()) return false;
        if (!energyController.TryConsumeInitialCost(isSmallClone)) return false;

        bool isBig = switchInterface.IsBigCloneSelected;

        if (isBig && (CheckColisionSpawn()))
            return false;

        Vector3? spawnPosition = ResolveSpawnPosition(isBig);
        if (spawnPosition == null) return false;

        SpawnClone(isBig, spawnPosition.Value);
        return true;
    }

    public bool TryDespawnClone()
    {
        if (!CloneActive || currentClone == null) return false;

        ReturnCameraToPlayer();
        energyController.UnregisterClone(currentClone);
        Destroy(currentClone);
        currentClone = null;
        CloneActive = false;
        return true;
    }

    public void RegisterExternalClone(GameObject clone, bool isSmall)
    {
        currentClone = clone;
        CloneActive = true;
        energyController.RegisterClone(clone, isSmall);
        AttachCameraToClone(clone);
        perspectiveSwitch.SwitchToClone();
    }

    public GameObject GetCurrentClone() => currentClone;
    public bool GetActiveClone() => CloneActive;

  
    private bool CanSpawn()
    {
        if (CloneActive) return false;

        foreach (var spawner in otherSpawners)
            if (spawner.CloneActive) return false;

        return true;
    }

   
    private Vector3? ResolveSpawnPosition(bool isBig)
    {
        Vector3 principal = cloneSpawnPointPrincipal.position;
        Vector3 secondary = cloneSpawnPointSecondary.position;

        Vector2 cloneSize = isBig ? BigCloneSize : SmallCloneSize;
        float heightOffset = isBig ? 1f : 0f;

        if (IsSpawnPositionValid(principal, cloneSize, heightOffset))
            return principal + Vector3.up * heightOffset;

        if (IsSpawnPositionValid(secondary, cloneSize, heightOffset))
            return secondary + Vector3.up * heightOffset;

        return null;
    }

   
    private bool IsSpawnPositionValid(Vector3 targetPosition, Vector2 size, float heightOffset)
    {
        if (IsPathBlocked(transform.position, targetPosition))
            return false;

        Vector3 finalPos = targetPosition + Vector3.up * heightOffset;
        return !Physics2D.OverlapBox(finalPos, size, 0f, groundLayer);
    }

    private void SpawnClone(bool isBig, Vector3 position)
    {
        GameObject prefab = isBig ? cloneBigPrefab : cloneSmallPrefab;
        currentClone = Instantiate(prefab, position, Quaternion.identity);
        energyController.RegisterClone(currentClone, isSmallClone);

        CloneActive = true;
        AttachCameraToClone(currentClone);
        perspectiveSwitch.SwitchToClone();
        soundManager.PlaySFX(soundManager.spawnClon);
    }

   
    private bool CheckColisionSpawn()
    {
        return IsPathBlocked(transform.position, cloneSpawnPointPrincipalUp.position);
    }

    private bool IsPathBlocked(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        RaycastHit2D hit = Physics2D.Raycast(from, direction.normalized, distance, groundLayer);
        return hit.collider != null;
    }

    private bool IsGroundLayer(int layer) => ((1 << layer) & groundLayer) != 0;

    private void AttachCameraToClone(GameObject clone)
    {
        playerCamera.transform.SetParent(clone.transform);
        playerCamera.transform.localPosition = CameraLocalPosition;
    }

    private void ReturnCameraToPlayer()
    {
        playerCamera.transform.SetParent(transform);
        playerCamera.transform.localPosition = CameraLocalPositionPlayer;
        perspectiveSwitch.SwitchToPlayer();
    }

    private void DrawSpawnGizmo(Transform spawnPoint)
    {
        if (spawnPoint == null) return;

        Vector3 target = spawnPoint.position;
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        Gizmos.DrawLine(transform.position, target);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction.normalized, distance, groundLayer);
        Gizmos.DrawSphere(hit.collider != null ? (Vector3)hit.point : target, 0.2f);
    }
}