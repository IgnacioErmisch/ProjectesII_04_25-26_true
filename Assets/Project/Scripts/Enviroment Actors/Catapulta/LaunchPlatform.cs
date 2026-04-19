using UnityEngine;
using System.Collections;

public class LaunchPlatform : MonoBehaviour
{
    [Header("Launch Settings")]
    [SerializeField] private Vector2 launchDirection = new Vector2(1, 1);
    [SerializeField] private float launchForce = 20f;
    [SerializeField] private float resetDelay = 1f;
    [SerializeField] private float movementDisableDuration = 1.2f;

    [Header("Visual Feedback")]
    [SerializeField] private float launchOffset = 0.5f;
    [SerializeField] private float launchAnimationSpeed = 15f;
    [SerializeField] private ParticleSystem launchEffect;
    [SerializeField] private SoundManager soundManager;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private GameObject pendingPlayer;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;

        if (soundManager == null)
            soundManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<SoundManager>();
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.deltaTime * launchAnimationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            pendingPlayer = collision.gameObject;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            pendingPlayer = collision.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            pendingPlayer = collision.gameObject;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            pendingPlayer = collision.gameObject;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            pendingPlayer = null;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            pendingPlayer = null;
    }

    public bool IsPlayerReady()
    {
        return pendingPlayer != null;
    }

    public void LaunchPlayer()
    {
        if (pendingPlayer == null) return;

        Rigidbody2D playerRb = pendingPlayer.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        PlayerMovement playerMovement = pendingPlayer.GetComponent<PlayerMovement>();
        if (playerMovement != null)
            playerMovement.DisableControlForLaunch(movementDisableDuration);

        StartCoroutine(ForceLaunch(playerRb));

        Vector2 normalizedDirection = launchDirection.normalized;
        Vector3 launchDir3D = new Vector3(normalizedDirection.x, normalizedDirection.y, 0);
        targetPosition = originalPosition + launchDir3D * launchOffset;

        PlayEffects();
        Invoke(nameof(ResetPlatform), resetDelay);
        pendingPlayer = null;
    }

    private IEnumerator ForceLaunch(Rigidbody2D rb)
    {
        if (rb == null) yield break;

        Vector2 normalizedDirection = launchDirection.normalized;
        Vector2 targetVelocity = normalizedDirection * launchForce;

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        float savedGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        for (int i = 0; i < 5; i++)
        {
            rb.linearVelocity = targetVelocity;
            yield return new WaitForFixedUpdate();
        }

        rb.gravityScale = savedGravity;
    }

    private void ResetPlatform()
    {
        targetPosition = originalPosition;
    }

    private void PlayEffects()
    {
        if (launchEffect != null)
            Instantiate(launchEffect, transform.position, Quaternion.identity);

        if (soundManager != null)
            soundManager.PlaySFX(soundManager.catapult);
    }

    public void ForceReset()
    {
        CancelInvoke();
        StopAllCoroutines();
        targetPosition = originalPosition;
        transform.localPosition = originalPosition;
        pendingPlayer = null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? transform.parent.position + originalPosition : transform.position;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos, transform.localScale);

        Vector2 normalizedDir = launchDirection.normalized;
        Vector3 launchDir3D = new Vector3(normalizedDir.x, normalizedDir.y, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(pos, launchDir3D * 3f);
        Gizmos.DrawWireSphere(pos + launchDir3D * 3f, 0.3f);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 3f);
    }
}