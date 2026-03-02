using UnityEngine;

public class SmallCloneMovment
{
    private Rigidbody2D rb;
    private SmallCloneStats stat;
    private SpriteRenderer spriteRenderer;


    private float maxSpeed;
    private float acceleration = 105f;
    private float deceleration = 55f;
    private float airAcceleration = 90f;
    private float airDeceleration = 90f;


    private Transform edgeCheckFront;
    private Transform edgeCheckBack;
    private float edgeCheckDistance = 0.3f;
    private float edgeClampSpeed = 2f;


    private Transform groundCheck;
    private float groundCheckRadius;
    private LayerMask groundLayer;

    private SoundManager soundManager;

    private Controller inputActions;

    public float horizontal { get; private set; }
    public bool isMoving { get; private set; }
    public bool isGrounded { get; private set; }
    private bool facingRight = true;
    private float currentSpeed;
    private bool isOnEdge;
    private bool wasMoving;

    public SmallCloneMovment(Rigidbody2D rb, SmallCloneStats stat, SpriteRenderer spriteRenderer,
                             Transform groundCheck, float groundCheckRadius, LayerMask groundLayer,
                             Transform edgeCheckFront, Transform edgeCheckBack)
    {
        this.rb = rb;
        this.stat = stat;
        this.spriteRenderer = spriteRenderer;
        this.groundCheck = groundCheck;
        this.groundCheckRadius = groundCheckRadius;
        this.groundLayer = groundLayer;
        this.edgeCheckFront = edgeCheckFront;
        this.edgeCheckBack = edgeCheckBack;

        this.maxSpeed = 7f * stat.SpeedMultiplier;
        this.currentSpeed = 0f;
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            this.soundManager = audioObject.GetComponent<SoundManager>();
        }

        this.inputActions = new Controller();
        this.inputActions.Gameplay.Enable();
    }

    public void Update()
    {
        isGrounded = CheckGround();
        CheckEdge();
    }

    public void Move(bool canControl)
    {
        if (canControl)
        {
            Vector2 moveInput = inputActions.Gameplay.Move.ReadValue<Vector2>();
            horizontal = moveInput.x;
        }
        else
        {
            horizontal = 0f;
        }

        ApplyMovement();
        bool isCurrentlyMoving = isGrounded && isMoving && Mathf.Abs(currentSpeed) > 0.1f;

        if (isCurrentlyMoving && !wasMoving)
        {
            soundManager.PlayLoop(soundManager.movementSC);
        }
        else if (!isCurrentlyMoving && wasMoving)
        {
            soundManager.StopLoop();
        }


        wasMoving = isCurrentlyMoving;

        if (isOnEdge && isGrounded)
        {
            ClampToEdge();
        }
    }

    private bool CheckGround()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void ApplyMovement()
    {
        float targetSpeed = horizontal * maxSpeed;
        float accel = isGrounded ? acceleration : airAcceleration;
        float decel = isGrounded ? deceleration : airDeceleration;

        if (Mathf.Abs(horizontal) > 0.01f)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accel * Time.fixedDeltaTime);
            isMoving = true;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, decel * Time.fixedDeltaTime);
            isMoving = Mathf.Abs(currentSpeed) > 0.1f;
        }

        rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocity.y);


        if (currentSpeed > 0.1f && !facingRight)
        {
            Flip();
        }
        else if (currentSpeed < -0.1f && facingRight)
        {
            Flip();
        }
    }

    private void CheckEdge()
    {
        if (!isGrounded || edgeCheckFront == null || edgeCheckBack == null)
        {
            isOnEdge = false;
            return;
        }

        Vector2 frontCheck = edgeCheckFront.position;
        Vector2 backCheck = edgeCheckBack.position;
        bool frontHasGround = Physics2D.Raycast(frontCheck, Vector2.down, edgeCheckDistance, groundLayer);
        bool backHasGround = Physics2D.Raycast(backCheck, Vector2.down, edgeCheckDistance, groundLayer);
        isOnEdge = !frontHasGround || !backHasGround;
    }

    private void ClampToEdge()
    {
        if (Mathf.Abs(rb.linearVelocity.x) > edgeClampSpeed)
        {
            float clampedVelocity = Mathf.Sign(rb.linearVelocity.x) * edgeClampSpeed;
            rb.linearVelocity = new Vector2(clampedVelocity, rb.linearVelocity.y);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;
    }

    public bool IsFacingRight()
    {
        return facingRight;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public bool IsOnEdge()
    {
        return isOnEdge;
    }
    public void Dispose()
    {
        if (inputActions != null)
        {
            inputActions.Gameplay.Disable();
            inputActions.Dispose();
        }
    }
}