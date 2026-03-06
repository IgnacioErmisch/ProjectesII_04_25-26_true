using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float airAcceleration;
    [SerializeField] private float airDeceleration;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Edge Detection")]
    [SerializeField] private Transform edgeCheckFront;
    [SerializeField] private Transform edgeCheckBack;
    [SerializeField] private float edgeCheckDistance = 0.3f;
    [SerializeField] private float edgeClampSpeed = 2f;

    [Header("References")]
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform cloneSpawnerPoint;
    [SerializeField] private Transform cloneSpawnerPointUp;
    [SerializeField] private Transform cloneSpawnerPointSecond;
    [SerializeField] private PlayerCombatController playerCombatController;

    private Controller inputActions;
    private SoundManager soundManager;
    private RideBigClone rideBigClone;

    public float horizontal { get; private set; }
    public bool isMoving { get; private set; }
    public bool isGrounded { get; private set; }

    private Rigidbody2D rb2D;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private float currentSpeed;
    private bool wasGrounded;
    private bool isOnEdge;
    private bool wasMoving;
    private bool isBeingLaunched = false;
    private float launchControlDisableTime = 0f;
    private Vector2 externalVelocity = Vector2.zero;
    private bool isInAirCurrent = false;
    private Vector2 airCurrentVelocity = Vector2.zero;
    private Vector2 airCurrentExtraVelocity = Vector2.zero;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        inputActions = new Controller();
    }

    private void OnEnable() => inputActions.Gameplay.Enable();
    private void OnDisable() => inputActions.Gameplay.Disable();

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rideBigClone = GetComponent<RideBigClone>();
    }

    private void Update()
    {
        wasGrounded = isGrounded;
        isGrounded = CheckGround();

        if (perspectiveSwitch.GetControllingPlayer() && !playerCombatController.IsDead())
        {
            Vector2 moveInput = inputActions.Gameplay.Move.ReadValue<Vector2>();
            horizontal = moveInput.x;
        }
        else
        {
            horizontal = 0f;
            isMoving = false;
        }

        bool isCurrentlyMoving = isGrounded && isMoving && Mathf.Abs(currentSpeed) > 0.1f;

        if (isCurrentlyMoving && !wasMoving)
            soundManager.PlayLoop(soundManager.movementP);
        else if (!isCurrentlyMoving && wasMoving)
            soundManager.StopLoop();

        wasMoving = isCurrentlyMoving;

        CheckEdge();
    }

    private void FixedUpdate()
    {
        if (isBeingLaunched)
        {
            if (Time.time < launchControlDisableTime)
            {
                externalVelocity = Vector2.zero;
                airCurrentExtraVelocity = Vector2.zero;
                return;
            }
            isBeingLaunched = false;
        }

        if (perspectiveSwitch.GetControllingPlayer() && !playerCombatController.IsDead())
        {
            ApplyMovement();
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.fixedDeltaTime);
            float finalX = currentSpeed + externalVelocity.x;
            float finalY = rb2D.linearVelocity.y + externalVelocity.y;
            rb2D.linearVelocity = new Vector2(finalX, finalY);
            isMoving = false;
        }

        externalVelocity = Vector2.zero;
        airCurrentExtraVelocity = Vector2.zero;

        if (isOnEdge && isGrounded && !isInAirCurrent)
            ClampToEdge();

        isInAirCurrent = false;
        airCurrentVelocity = Vector2.zero;
    }

    private void ApplyMovement()
    {
        if (isInAirCurrent)
        {
            Vector2 currentDir = airCurrentVelocity.normalized;
            Vector2 perp = new Vector2(-currentDir.y, currentDir.x);

            float playerPerpComponent = horizontal * maxSpeed * perp.x;

            float finalX = airCurrentVelocity.x + externalVelocity.x + airCurrentExtraVelocity.x
                           + perp.x * playerPerpComponent;
            float finalY = airCurrentVelocity.y + externalVelocity.y + airCurrentExtraVelocity.y
                           + perp.y * playerPerpComponent;

            rb2D.linearVelocity = new Vector2(finalX, finalY);
            currentSpeed = rb2D.linearVelocity.x;

            if (horizontal > 0.1f && !facingRight) Flip();
            else if (horizontal < -0.1f && facingRight) Flip();

            return;
        }

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

        float finalXNormal = currentSpeed + externalVelocity.x;
        float finalYNormal = rb2D.linearVelocity.y + externalVelocity.y;
        rb2D.linearVelocity = new Vector2(finalXNormal, finalYNormal);

        if (currentSpeed > 0.1f && !facingRight) Flip();
        else if (currentSpeed < -0.1f && facingRight) Flip();
        
    }

    public void SetExternalVelocity(Vector2 velocity) => externalVelocity += velocity;
    public void SetAirCurrentVelocity(Vector2 velocity) { isInAirCurrent = true; airCurrentVelocity = velocity; }
    public void AddAirCurrentExtra(Vector2 extra) => airCurrentExtraVelocity += extra;
    public void ClearAirCurrent() { isInAirCurrent = false; airCurrentVelocity = Vector2.zero; airCurrentExtraVelocity = Vector2.zero; }
    public void DisableControlForLaunch(float duration) { isBeingLaunched = true; launchControlDisableTime = Time.time + duration; }

    public bool IsFacingRight() => facingRight;
    public float GetCurrentSpeed() => currentSpeed;
    public bool IsOnEdge() => isOnEdge;

   
    public void ForceFlipVisualOnly(bool shouldFaceRight)
    {
        if (facingRight == shouldFaceRight) return;
        facingRight = shouldFaceRight;
        spriteRenderer.flipX = !facingRight;
        cloneSpawnerPoint.localPosition = new Vector3(-cloneSpawnerPoint.localPosition.x, cloneSpawnerPoint.localPosition.y, cloneSpawnerPoint.localPosition.z);
        cloneSpawnerPointUp.localPosition = new Vector3(-cloneSpawnerPointUp.localPosition.x, cloneSpawnerPointUp.localPosition.y, cloneSpawnerPointUp.localPosition.z);
        cloneSpawnerPointSecond.localPosition = new Vector3(-cloneSpawnerPointSecond.localPosition.x, cloneSpawnerPointSecond.localPosition.y, cloneSpawnerPointSecond.localPosition.z);
    }

    private bool CheckGround() => Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    private void CheckEdge()
    {
        if (!isGrounded) { isOnEdge = false; return; }
        bool frontHasGround = Physics2D.Raycast(edgeCheckFront.position, Vector2.down, edgeCheckDistance, groundLayer);
        bool backHasGround = Physics2D.Raycast(edgeCheckBack.position, Vector2.down, edgeCheckDistance, groundLayer);
        isOnEdge = !frontHasGround || !backHasGround;
    }

    private void ClampToEdge()
    {
        if (Mathf.Abs(rb2D.linearVelocity.x) > edgeClampSpeed)
        {
            float clamped = Mathf.Sign(rb2D.linearVelocity.x) * edgeClampSpeed;
            rb2D.linearVelocity = new Vector2(clamped, rb2D.linearVelocity.y);
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        spriteRenderer.flipX = !facingRight;

        Vector3 attackScale = attackPoint.localScale;
        attackScale.x *= -1;
        attackPoint.localScale = attackScale;

        cloneSpawnerPoint.localPosition = new Vector3(-cloneSpawnerPoint.localPosition.x, cloneSpawnerPoint.localPosition.y, cloneSpawnerPoint.localPosition.z);
        cloneSpawnerPointUp.localPosition = new Vector3(-cloneSpawnerPointUp.localPosition.x, cloneSpawnerPointUp.localPosition.y, cloneSpawnerPointUp.localPosition.z);
        cloneSpawnerPointSecond.localPosition = new Vector3(-cloneSpawnerPointSecond.localPosition.x, cloneSpawnerPointSecond.localPosition.y, cloneSpawnerPointSecond.localPosition.z);
    }

    private void OnDrawGizmosSelected()
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