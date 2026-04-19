using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Attack Settings")]
    [SerializeField] private GameObject turretBullet;
    [SerializeField] private Transform spawnBullets;
    [SerializeField] private float shootCooldown = 1f;

    private RadiusDetectionSystem detectionSystem;
    private Transform playerTarget;
    private float lastShootTime;

    private void Awake()
    {
        if (detectionPoint == null)
        {
            detectionPoint = transform;
        }

        detectionSystem = gameObject.AddComponent<RadiusDetectionSystem>();
        detectionSystem.SetDetectionRadius(detectionRadius);
        detectionSystem.SetTargetLayer(playerLayer);
    }

    void Update()
    {
        if (detectionSystem.DetectTarget())
        {
            playerTarget = detectionSystem.GetTarget();
            if (Time.time >= lastShootTime + shootCooldown)
            {
                Attack();
                lastShootTime = Time.time;
            }
        }
    }

    private void Attack()
    {
        Vector3 spawnPos = spawnBullets.transform.position;
        GameObject bullet = Instantiate(turretBullet, spawnPos, Quaternion.identity);
        TurretBullet bulletScript = bullet.GetComponent<TurretBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetTarget(playerTarget);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (detectionPoint != null)
        {
            Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}