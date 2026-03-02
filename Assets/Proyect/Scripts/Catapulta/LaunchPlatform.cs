using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaunchPlatform : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private Vector2 launchDirection = new Vector2(1, 1);
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float movementDisableDuration = 0.5f;

    [Header("Dynamic Force Scaling")]
    [SerializeField] private bool useScalableForce = true;
    [SerializeField] private float minForceMultiplier = 1f;
    [SerializeField] private float maxForceMultiplier = 3f;
    [SerializeField] private float baseImpactForce = 5f;
    [SerializeField] private float maxImpactForce = 20f;

    [Header("Visual Feedback")]
    [SerializeField] private float launchOffset = 0.5f;
    [SerializeField] private float launchAnimationSpeed = 15f;
    [SerializeField] private ParticleSystem launchEffect;
    [SerializeField] private AudioClip launchSound;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private AudioSource audioSource;
    private List<GameObject> playersNearby = new List<GameObject>();

    private void Awake()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && launchSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * launchAnimationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playersNearby.Contains(collision.gameObject))
            {
                playersNearby.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!playersNearby.Contains(collision.gameObject))
            {
                playersNearby.Add(collision.gameObject);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playersNearby.Contains(collision.gameObject))
            {
                playersNearby.Add(collision.gameObject);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!playersNearby.Contains(collision.gameObject))
            {
                playersNearby.Add(collision.gameObject);
            }
        }
    }

    public void LaunchPlayer(float impactForce = 0f)
    {
        float forceMultiplier = CalculateForceMultiplier(impactForce);

        GameObject playerToLaunch = FindClosestPlayer();

        if (playerToLaunch == null) return;

        Rigidbody2D playerRb = playerToLaunch.GetComponent<Rigidbody2D>();

        if (playerRb == null) return;

        PlayerMovement playerMovement = playerToLaunch.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.DisableControlForLaunch(movementDisableDuration);
        }

        if (playerRb.bodyType == RigidbodyType2D.Kinematic)
        {
            playerRb.bodyType = RigidbodyType2D.Dynamic;
        }

        RigidbodyConstraints2D originalConstraints = playerRb.constraints;
        playerRb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Vector2 normalizedDirection = launchDirection.normalized;
        float finalForce = launchForce * forceMultiplier;

        playerRb.linearVelocity = normalizedDirection * finalForce;

        Vector3 launchDir3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0);
        float scaledOffset = launchOffset * Mathf.Clamp(forceMultiplier, 0.5f, 2f);
        targetPosition = originalPosition + launchDir3D * scaledOffset;

        PlayEffects();

        Invoke(nameof(ClearPlayersList), 0.5f);
        Invoke(nameof(ResetPlatform), resetDelay);
    }

    private float CalculateForceMultiplier(float impactForce)
    {
        if (!useScalableForce || impactForce <= 0f)
        {
            return minForceMultiplier;
        }

        float normalizedImpact = Mathf.InverseLerp(baseImpactForce, maxImpactForce, impactForce);
        float multiplier = Mathf.Lerp(minForceMultiplier, maxForceMultiplier, normalizedImpact);

        return Mathf.Clamp(multiplier, minForceMultiplier, maxForceMultiplier);
    }

    private GameObject FindClosestPlayer()
    {
        GameObject closest = null;
        float closestDistance = float.MaxValue;

        playersNearby.RemoveAll(player => player == null);

        foreach (GameObject player in playersNearby)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);

            if (distance < 3f && distance < closestDistance)
            {
                closest = player;
                closestDistance = distance;
            }
        }

        return closest;
    }

    private void ClearPlayersList()
    {
        playersNearby.Clear();
    }

    private void ResetPlatform()
    {
        targetPosition = originalPosition;
    }

    private void PlayEffects()
    {
        if (launchEffect != null)
        {
            Instantiate(launchEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && launchSound != null)
        {
            audioSource.PlayOneShot(launchSound);
        }
    }

    public void ForceReset()
    {
        CancelInvoke();
        targetPosition = originalPosition;
        transform.position = originalPosition;
        playersNearby.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? originalPosition : transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos, transform.localScale);

        Vector2 normalizedDir = launchDirection.normalized;
        Vector3 launchDir3D = new Vector3(normalizedDir.x, normalizedDir.y, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(pos, launchDir3D * 3f);
        Gizmos.DrawWireSphere(pos + launchDir3D * 3f, 0.3f);

        if (useScalableForce)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(pos, launchDir3D * 3f * maxForceMultiplier);
            Gizmos.DrawWireSphere(pos + launchDir3D * 3f * maxForceMultiplier, 0.3f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 3f);
    }
}