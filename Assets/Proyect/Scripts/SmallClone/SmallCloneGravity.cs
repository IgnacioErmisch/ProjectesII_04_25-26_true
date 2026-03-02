using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CloneGravity : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float invertedYOffset = -0.2f;
    [SerializeField] private float groundCheckInvertedOffset;

    private Rigidbody2D rb;
    private bool isInverted = false;
    private Transform spriteTransform;
    private Vector3 originalSpritePosition;
    private Vector3 originalGroundCheckPosition; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            spriteTransform = spriteRenderer.transform;
            originalSpritePosition = spriteTransform.localPosition;
        }

        if (groundCheck != null)
        {
            originalGroundCheckPosition = groundCheck.localPosition;
        }
        else
        {
            Transform foundCheck = transform.Find("groundCheck");
            if (foundCheck != null)
            {
                groundCheck = foundCheck;
                originalGroundCheckPosition = groundCheck.localPosition;
            }
           
        }
    }

    void LateUpdate()
    {
        if (isInverted)
        {
            if (rb.gravityScale > 0)
            {
                rb.gravityScale = -rb.gravityScale;
            }
        }
        else
        {
            if (rb.gravityScale < 0)
            {
                rb.gravityScale = -rb.gravityScale;
            }
        }
    }

    public void InvertGravity()
    {
        isInverted = !isInverted;

        rb.gravityScale = -rb.gravityScale;

   
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, -rb.linearVelocity.y);

  
        if (Mathf.Abs(rb.linearVelocity.y) < 0.5f)
        {
            float impulseDirection = rb.gravityScale < 0 ? 1f : -1f;
            rb.AddForce(Vector2.up * impulseDirection * 3f, ForceMode2D.Impulse);
        }

        if (spriteTransform != null)
        {
            if (isInverted)
            {
                spriteTransform.localEulerAngles = new Vector3(180f, 0f, 0f);
                spriteTransform.localPosition = new Vector3(   originalSpritePosition.x,  -originalSpritePosition.y + invertedYOffset,  originalSpritePosition.z);
            }
            else
            {
                spriteTransform.localEulerAngles = new Vector3(0f, 0f, 0f);
                spriteTransform.localPosition = originalSpritePosition;
            }
        }

  
        if (groundCheck != null)
        {
            if (isInverted)
            {
                groundCheck.localPosition = new Vector3(originalGroundCheckPosition.x, 
                                                        groundCheckInvertedOffset, 
                                                        originalGroundCheckPosition.z);
            }
            else
            {
                groundCheck.localPosition = originalGroundCheckPosition;
            }
        }
    }

    public bool IsInverted()
    {
        return isInverted;
    }

    public float GetGravityMultiplier()
    {
        return isInverted ? -1f : 1f;
    }
}