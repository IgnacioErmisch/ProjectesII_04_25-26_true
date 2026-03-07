using UnityEngine;

public class BigCloneController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float maxSpeed = 7f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 60f;
    [SerializeField] private float airAcceleration = 80f;
    [SerializeField] private float airDeceleration = 80f;

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
    [SerializeField] private Transform attackPoint;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private BigCloneAttack bigCloneAttack;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private BigCloneStats stats;
    private Transform transformBigClone;
    public GameObject energyImage;
    public BigCloneMovement movement { get; private set; }
    private BigCloneWallDestroyer wallDestroyer;
    private WallContactDetector wallContactDetector;
    private WallDestructor wallDestructor;
    [SerializeField] private Transform canvas;

    [SerializeField] private RideBigClone rideBigClone;
    [SerializeField] private SoundManager soundManager;


    private void Awake()
    {
        InitializeComponents();
        ApplySizeModifier();
        CinemachineSingleton.Instance.SetBigClone(transformBigClone);
        GameManager.Instance.SetBigCloneEnergy(energyImage);
        GameManager.Instance.SetBigCloneCanvas(canvas);
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }

    private void OnDestroy()
    {
        rideBigClone.DetachPlayer();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        transformBigClone = GetComponent<Transform>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        stats = new BigCloneStats();

       
        movement = new BigCloneMovement(
            rb,
            stats,
            spriteRenderer,
            attackPoint,
            groundCheck,
            edgeCheckFront,
            edgeCheckBack,
            groundLayer
        );

        wallContactDetector = new WallContactDetector();
        wallDestructor = new WallDestructor();
        wallDestroyer = new BigCloneWallDestroyer(wallContactDetector, wallDestructor, bigCloneAttack);

        if (perspectiveSwitch == null)
        {
            perspectiveSwitch = FindFirstObjectByType<PerspectiveSwitch>();
        }
    }

    private void ApplySizeModifier()
    {
        transform.localScale = Vector3.one * stats.SizeMultiplier;
    }

    private void Update()
    {
        if (movement != null && !perspectiveSwitch.GetControllingPlayer())
        {
            movement.UpdateMovement();
        }

        if (!wallDestroyer.isAttacking)
        {
            bool wasTouchingWall = wallContactDetector.IsTouchingWall();

            wallDestroyer.CheckAndDestroyWall();

            if (wasTouchingWall && !wallContactDetector.IsTouchingWall())
            {
                soundManager.PlaySFX(soundManager.wallBreak); 
            }

            if (wallContactDetector.IsTouchingWall() && bigCloneAttack.isDashing)
            {
                wallDestroyer.StartAttack(this);
            }
        }
    }

    private void FixedUpdate()
    {
        if (movement != null)
        {
            movement.Move(!perspectiveSwitch.GetControllingPlayer());
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            wallContactDetector.SetWallContact(collision.gameObject, true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (movement != null)
        {
            movement.DrawGizmos(groundCheck, edgeCheckFront, edgeCheckBack);
        }
        else if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

            if (edgeCheckFront != null && edgeCheckBack != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(edgeCheckFront.position, edgeCheckFront.position + Vector3.down * edgeCheckDistance);
                Gizmos.DrawLine(edgeCheckBack.position, edgeCheckBack.position + Vector3.down * edgeCheckDistance);
            }
        }
    }
}