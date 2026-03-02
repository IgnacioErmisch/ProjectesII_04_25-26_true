using UnityEngine;

public class BigCloneMovement
{
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Transform attackPoint;
    private Transform groundCheck;
    private Transform edgeCheckFront;
    private Transform edgeCheckBack;
    public BigCloneStats stat;
    private float maxSpeed = 7f;
    private float acceleration = 100f;
    private float deceleration = 60f;
    private float airAcceleration = 90f;
    private float airDeceleration = 90f;
    private float groundCheckRadius = 0.2f;
    private float edgeCheckDistance = 0.3f;
    private float edgeClampSpeed = 4f;
    private LayerMask groundLayer;

    private Controller inputActions;

    public float horizontal { get; private set; }
    public bool isMoving { get; private set; }
    public bool isGrounded { get; private set; }

    private bool facingRight = true;
    private float currentSpeed;
    private bool wasGrounded;
    private bool isOnEdge;
    private SoundManager soundManager;
    private bool wasMoving;

    public BigCloneMovement(
        Rigidbody2D rb,
        BigCloneStats stat,
        SpriteRenderer spriteRenderer,
        Transform attackPoint = null,
        Transform groundCheck = null,
        Transform edgeCheckFront = null,
        Transform edgeCheckBack = null,
        LayerMask groundLayer = default)
    {
        this.rb = rb;
        this.stat = stat;
        this.spriteRenderer = spriteRenderer;
        this.attackPoint = attackPoint;
        this.groundCheck = groundCheck;
        this.edgeCheckFront = edgeCheckFront;
        this.edgeCheckBack = edgeCheckBack;
        this.groundLayer = groundLayer;
        GameObject audioObject = GameObject.FindGameObjectWithTag("Audio");
        if (audioObject != null)
        {
            this.soundManager = audioObject.GetComponent<SoundManager>();
        }

        this.inputActions = new Controller();
        this.inputActions.Gameplay.Enable();
    }


    public void UpdateMovement()
    {
        wasGrounded = isGrounded;
        isGrounded = CheckGround();
        CheckEdge();
    }

    public void Move(bool canMove)
    {
        if (canMove)
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
            soundManager.PlayLoop(soundManager.movementBC);
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

    private bool CheckGround()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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

        if (attackPoint != null)
        {
            Vector3 attackScale = attackPoint.localScale;
            attackScale.x *= -1;
            attackPoint.localScale = attackScale;
        }
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

    public bool GetMove()
    {
        return isMoving;
    }

    public void Dispose()
    {
        if (inputActions != null)
        {
            inputActions.Gameplay.Disable();
            inputActions.Dispose();
        }
    }

    public void DrawGizmos(Transform groundCheck, Transform edgeCheckFront, Transform edgeCheckBack)
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (edgeCheckFront != null && edgeCheckBack != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(edgeCheckFront.position, edgeCheckFront.position + Vector3.down * edgeCheckDistance);
            Gizmos.DrawLine(edgeCheckBack.position, edgeCheckBack.position + Vector3.down * edgeCheckDistance);
        }
    }
}