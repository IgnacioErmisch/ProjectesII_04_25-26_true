using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class CatapultPlatform : MonoBehaviour
{
    [Header("Activation Methods")]
    [SerializeField] private bool useImpactForce = true;
    [SerializeField] private bool useDashDetection = true;
    [SerializeField] private float requiredImpactForce = 2f;
    [SerializeField] private float resetDelay = 0.5f;

    [Header("Visual Feedback")]
    [SerializeField] private float pressedOffset = 0.3f;
    [SerializeField] private float pressAnimationSpeed = 10f;
    [SerializeField] private ParticleSystem impactEffect;
    [SerializeField] private AudioClip impactSound;

    [Header("References")]
    [SerializeField] private CatapultSystem catapultSystem;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool isPressed = false;
    private AudioSource audioSource;
    private float lastImpactForce = 0f; 

    private void Awake()
    {
        originalPosition = transform.position;
        targetPosition = originalPosition;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && impactSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (catapultSystem == null)
        {
            catapultSystem = FindFirstObjectByType<CatapultSystem>();
        }
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * pressAnimationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BigClone"))
        {
        
            bool shouldActivate = false;
            float impactForce = 0f;

            if (useDashDetection)
            {
                BigCloneAttack bigCloneAttack = collision.gameObject.GetComponent<BigCloneAttack>();

                if (bigCloneAttack != null && bigCloneAttack.isDashing)
                {
                    shouldActivate = true;
                    impactForce = collision.relativeVelocity.magnitude * 1.5f;
                }
            }

            if (useImpactForce && !shouldActivate)
            {
                impactForce = collision.relativeVelocity.magnitude;

                if (impactForce >= requiredImpactForce)
                {
                    shouldActivate = true;
                }
            }

            if (shouldActivate)
            {
                ActivateCatapult(impactForce);
            } 
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BigClone") && !isPressed)
        {
            if (useDashDetection)
            {
                BigCloneAttack bigCloneAttack = collision.gameObject.GetComponent<BigCloneAttack>();

                if (bigCloneAttack != null && bigCloneAttack.isDashing)
                {
                    float impactForce = collision.relativeVelocity.magnitude * 1.5f;
                    ActivateCatapult(impactForce);
                }
            }
        }
    }

    private void ActivateCatapult(float impactForce)
    {
       
        isPressed = true;
        lastImpactForce = impactForce; 
        targetPosition = originalPosition - Vector3.up * pressedOffset;
        PlayEffects();

        if (catapultSystem != null)
        {
            catapultSystem.OnCatapultActivated(lastImpactForce);
        }   
        Invoke(nameof(ResetPlatform), resetDelay);
    }

    private void ResetPlatform()
    {
        targetPosition = originalPosition;
        isPressed = false;
        lastImpactForce = 0f;
    }

    private void PlayEffects()
    {
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        if (audioSource != null && impactSound != null)
        {
            audioSource.PlayOneShot(impactSound);
        }
    }

    public void ForceReset()
    {
        CancelInvoke(nameof(ResetPlatform));
        targetPosition = originalPosition;
        isPressed = false;
        lastImpactForce = 0f;
        transform.position = originalPosition;
    }
    public float GetLastImpactForce()
    {
        return lastImpactForce;
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