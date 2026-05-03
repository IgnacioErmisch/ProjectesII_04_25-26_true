using UnityEngine;

public class CatapultPlatform : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float resetDelay = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private float pressedOffset = 0.3f;
    [SerializeField] private float pressAnimationSpeed = 10f;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private AudioClip impactSound;

    [Header("References")]
    [SerializeField] private CatapultSystem catapultSystem;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isPressed = false;
    private bool bigCloneOnPlatform = false; 
    private AudioSource audioSource;

    private void Awake()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && impactSound != null)
            audioSource = gameObject.AddComponent<AudioSource>();
        if (catapultSystem == null)
            catapultSystem = FindFirstObjectByType<CatapultSystem>();
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * pressAnimationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("BigClone")) return;

        bigCloneOnPlatform = true;

        if (isPressed) return; 

        ActivateCatapult();
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("BigClone")) return;

        bigCloneOnPlatform = false;
        isPressed = false; 
    }

    private void ActivateCatapult()
    {
        isPressed = true;
        targetPosition = originalPosition - Vector3.up * pressedOffset;
        PlayEffects();

        if (catapultSystem != null)
            catapultSystem.OnCatapultActivated(0f);

        Invoke(nameof(ResetVisual), resetDelay); 
    }

    private void ResetVisual()
    {
        targetPosition = originalPosition;
    }

    private void PlayEffects()
    {
        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        if (audioSource != null && impactSound != null)
            audioSource.PlayOneShot(impactSound);
    }

    public void ForceReset()
    {
        CancelInvoke(nameof(ResetVisual));
        targetPosition = originalPosition;
        transform.position = originalPosition;
        isPressed = false;
        bigCloneOnPlatform = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? originalPosition : transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(pos, transform.localScale);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos - Vector3.up * pressedOffset, transform.localScale);
    }
}