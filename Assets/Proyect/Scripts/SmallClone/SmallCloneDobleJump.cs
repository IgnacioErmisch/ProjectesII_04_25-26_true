using UnityEngine;

public class SmallCloneDoubleJump
{
    private Rigidbody2D rb;
    private Transform groundCheck;
    private float groundCheckRadius;
    private LayerMask groundLayer;
    private float jumpForce;
    private float secondJumpForce;
    private float jumpMultiplier;
    private int maxJumps;
    private float normalGravityScale = 2.5f;
    private float fallGravityMultiplier = 2.5f;
    private float lowJumpMultiplier = 3f;
    private float maxFallSpeed = 20f;
    private float apexThreshold = 2f;
    private float apexHangTime = 0.1f;
    private float apexGravityMultiplier = 1.5f;
    private float jumpCutMultiplier = 0.5f;
    private float coyoteTime;
    private float coyoteCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;
    private bool isAtApex;
    private float apexHangCounter;
    private bool jumpHeld;
    private bool jumpCut;
    private bool wasGrounded;
    private int jumpCounter = 0;

    private bool bounceOverride = false;
    private float bounceOverrideTimer = 0f;
    private const float bounceOverrideDuration = 0.15f;

    public bool isJumping { get; private set; }
    public bool isGrounded { get; private set; }

    [SerializeField] private SoundManager soundManager;

    private Controller inputActions;
    private CloneGravity cloneGravity;

    public SmallCloneDoubleJump(Rigidbody2D rb, Transform groundCheck, float groundCheckRadius,
                                LayerMask groundLayer, float jumpForce, float secondJumpForce, float jumpMultiplier,
                                float coyoteTime, int maxJumps = 2, int jumpCounter = 0)
    {
        this.rb = rb;
        this.groundCheck = groundCheck;
        this.groundCheckRadius = groundCheckRadius;
        this.groundLayer = groundLayer;
        this.jumpForce = jumpForce;
        this.secondJumpForce = secondJumpForce;
        this.jumpMultiplier = jumpMultiplier;
        this.coyoteTime = coyoteTime;
        this.maxJumps = maxJumps;
        this.jumpBufferCounter = 0f;
        this.coyoteCounter = 0f;
        this.jumpCounter = 0;

        this.cloneGravity = rb.GetComponent<CloneGravity>();

        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
            this.soundManager = audioObject.GetComponent<SoundManager>();

        this.inputActions = new Controller();
        this.inputActions.Gameplay.Enable();
    }

    public void Update(bool canControl, ParticleSystem particleLand, ParticleSystem particleJump)
    {
        wasGrounded = isGrounded;
        isGrounded = CheckGround();

        if (isGrounded && !wasGrounded)
            OnLand(particleLand);
        else if (isGrounded && isJumping && IsMovingTowardGravity())
            OnLand(particleLand);

        if (!isGrounded && !isJumping)
            coyoteCounter -= Time.deltaTime;

        if (inputActions.Gameplay.Jump.triggered)
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        jumpHeld = inputActions.Gameplay.Jump.IsPressed();

        
        if (bounceOverride)
        {
            bounceOverrideTimer -= Time.deltaTime;
            if (bounceOverrideTimer <= 0f)
                bounceOverride = false;
        }

        if (canControl && !bounceOverride)
        {
            if (jumpBufferCounter > 0f && CanJump())
            {
                PerformJump(particleJump);
                jumpBufferCounter = 0f;
            }

            if (!jumpHeld && IsMovingAwayFromGravity() && !jumpCut)
                CutJump();
        }

        CheckApex();
    }

    public void FixedUpdate()
    {
        ApplyGravityModifiers();
        ClampFallSpeed();
    }

   
    public void ApplyBounce(float force)
    {
        float actualForce = (cloneGravity != null && cloneGravity.IsInverted()) ? -force : force;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, actualForce);

        bounceOverride = true;
        bounceOverrideTimer = 0.4f; 
        jumpBufferCounter = 0f;
        jumpCut = false;
        isJumping = true;
    }

    private bool CheckGround()
    {
        if (groundCheck == null) return false;

        Vector2 checkPosition = groundCheck.position;

        if (cloneGravity != null && cloneGravity.IsInverted())
            checkPosition += Vector2.up * (groundCheckRadius * 2);

        return Physics2D.OverlapCircle(checkPosition, groundCheckRadius, groundLayer);
    }

    public bool CanJump()
    {
        return jumpCounter < maxJumps && (isGrounded || coyoteCounter > 0f);
    }

    public void PerformJump(ParticleSystem particleJump)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        soundManager.PlaySFX(soundManager.jump);

        float actualJumpForce = jumpCounter > 0
            ? secondJumpForce * jumpMultiplier
            : jumpForce * jumpMultiplier;

        if (cloneGravity != null && cloneGravity.IsInverted())
            actualJumpForce = -actualJumpForce;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, actualJumpForce);
        jumpCounter++;
        isJumping = true;
        jumpCut = false;

        if (particleJump != null && jumpCounter > 1)
            particleJump.Play();
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
            apexHangCounter -= Time.deltaTime;
    }

    private void ApplyGravityModifiers()
    {
        float gravityMultiplier = cloneGravity != null ? cloneGravity.GetGravityMultiplier() : 1f;
        float baseGravity = normalGravityScale * gravityMultiplier;

        if (bounceOverride)
        {
           
            rb.gravityScale = baseGravity * fallGravityMultiplier;
            return;
        }

        if (isAtApex && apexHangCounter > 0f)
            rb.gravityScale = baseGravity * apexGravityMultiplier;
        else if (IsMovingTowardGravity())
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        else if (IsMovingAwayFromGravity() && !jumpHeld)
            rb.gravityScale = baseGravity * lowJumpMultiplier;
        else
            rb.gravityScale = baseGravity;
    }

    private void ClampFallSpeed()
    {
        if (cloneGravity != null && cloneGravity.IsInverted())
        {
            if (rb.linearVelocity.y > maxFallSpeed)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
        else
        {
            if (rb.linearVelocity.y < -maxFallSpeed)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private bool IsMovingTowardGravity()
    {
        if (cloneGravity != null && cloneGravity.IsInverted())
            return rb.linearVelocity.y > 0f;
        return rb.linearVelocity.y < 0f;
    }

    private bool IsMovingAwayFromGravity()
    {
        if (cloneGravity != null && cloneGravity.IsInverted())
            return rb.linearVelocity.y < 0f;
        return rb.linearVelocity.y > 0f;
    }

    public void OnLand(ParticleSystem particleLand)
    {
        isJumping = false;
        jumpCut = false;
        jumpCounter = 0;
        coyoteCounter = coyoteTime;
        bounceOverride = false;

        if (particleLand != null)
            particleLand.Play();
    }

    public bool Landed() => wasGrounded == false && isGrounded == true;
    public bool IsJumping() => isJumping;
    public bool IsAtApex() => isAtApex;

    public void Dispose()
    {
        if (inputActions != null)
        {
            inputActions.Gameplay.Disable();
            inputActions.Dispose();
        }
    }
}