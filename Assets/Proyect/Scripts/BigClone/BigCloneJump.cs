using UnityEngine;

public class BigCloneJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int maxJumps = 1;

    [Header("Gravity Controls")]
    [SerializeField] private float normalGravityScale = 2.5f;
    [SerializeField] private float fallGravityMultiplier = 2f;
    [SerializeField] private float lowJumpMultiplier = 3f;
    [SerializeField] private float maxFallSpeed = 20f;

    [Header("Coyote Time")]
    [SerializeField] private float coyoteTime;

    [Header("Jump Buffer")]
    [SerializeField] private float jumpBufferTime = 0.2f;

    [Header("Apex Modifiers")]
    [SerializeField] private float apexThreshold = 2f;
    [SerializeField] private float apexHangTime = 0.1f;
    [SerializeField] private float apexGravityMultiplier = 0.5f;

    [Header("Jump Cut")]
    [SerializeField] private float jumpCutMultiplier = 0.5f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem landParticles;
    [SerializeField] private ParticleSystem runParticles;

    [Header("Audio")]
    [SerializeField] SoundManager soundManager;

    private Controller inputActions;

    public bool isJumping { get; private set; }
    public bool isGrounded { get; private set; }
    public int jumpCounter = 0;


    public float coyoteCounter;
    private float jumpBufferCounter;
    private float apexHangCounter;
    private Rigidbody2D rb;
    private bool wasGrounded;
    private bool isAtApex;
    private bool jumpHeld;
    private bool jumpCut;
    private bool wasJumpPressed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

        inputActions = new Controller();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = CheckGround();

        if (isGrounded && !wasGrounded)
        {
            OnLand();
        }


        if (!isGrounded && !isJumping)
        {
            coyoteCounter -= Time.deltaTime;
        }


        bool jumpPressed = inputActions.Gameplay.Jump.triggered;

        if (jumpPressed)
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        jumpHeld = inputActions.Gameplay.Jump.IsPressed();

        bool canControl = perspectiveSwitch == null || perspectiveSwitch.GetControllingPlayer();

        if (canControl)
        {
            if (jumpBufferCounter > 0f && CanJump() && !GameManager.Instance.IsPlayerDead())
            {
                PerformJump();
                jumpBufferCounter = 0f;
            }

            if (rb.linearVelocity.y > 0f && !jumpCut)
            {
                CutJump();
            }
        }

        CheckApex();
        UpdateParticles();

    }

    private void FixedUpdate()
    {
        ApplyGravityModifiers();
        ClampFallSpeed();
    }

    private bool CheckGround()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private bool CanJump()
    {
        if (jumpCounter == 0)
        {
            return isGrounded || coyoteCounter > 0f;
        }
        else
        {
            return jumpCounter < maxJumps;
        }
    }

    private void PerformJump()
    {
        soundManager.PlaySFX(soundManager.jump);
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        jumpCounter++;

        isJumping = true;
        jumpCut = false;

        if (jumpParticles != null && jumpCounter > 1)
        {
            jumpParticles.Play();
        }

    }

    private void CutJump()
    {
        jumpCut = true;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
    }

    private void CheckApex()
    {

        if (Mathf.Abs(rb.linearVelocity.y) < apexThreshold && !isGrounded)
        {
            if (!isAtApex)
            {
                isAtApex = true;
                apexHangCounter = apexHangTime;
            }
        }
        else
        {
            isAtApex = false;
        }

        if (isAtApex && apexHangCounter > 0f)
        {
            apexHangCounter -= Time.deltaTime;
        }
    }

    private void ApplyGravityModifiers()
    {

        if (isAtApex && apexHangCounter > 0f)
        {
            rb.gravityScale = normalGravityScale * apexGravityMultiplier;
        }

        else if (rb.linearVelocity.y < 0f)
        {
            rb.gravityScale = normalGravityScale * fallGravityMultiplier;
        }

        else if (rb.linearVelocity.y > 0f && !jumpHeld)
        {
            rb.gravityScale = normalGravityScale * lowJumpMultiplier;
        }

        else
        {
            rb.gravityScale = normalGravityScale;
        }
    }

    private void ClampFallSpeed()
    {
        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private void OnLand()
    {
        isJumping = false;
        jumpCut = false;
        jumpCounter = 0;
        coyoteCounter = coyoteTime;
        if (landParticles != null)
        {
            landParticles.Play();
        }
    }

    private void UpdateParticles()
    {

        if (runParticles != null && movement != null)
        {
            if (isGrounded && movement.isMoving && !runParticles.isPlaying)
            {
                runParticles.Play();
            }
            else if ((!isGrounded || !movement.isMoving) && runParticles.isPlaying)
            {
                runParticles.Stop();
            }
        }
    }

    public bool IsAtApex()
    {
        return isAtApex;
    }

    public int GetJumpCount()
    {
        return jumpCounter;
    }

    public float GetVerticalVelocity()
    {
        return rb != null ? rb.linearVelocity.y : 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}