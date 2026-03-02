using UnityEngine;

public class BigCloneAttack : MonoBehaviour
{

    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashDamage;
    [SerializeField] private float dashKnockbackForce;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private SpriteRenderer sr;

    private Controller inputActions;

    public bool isDashing = false;
    private bool canDash = true;
    private float dashTimer = 0f;
    private float cooldownTimer = 0f;

    private void Awake()
    {
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (inputActions.Gameplay.Dash.triggered && canDash && !isDashing && !GameManager.Instance.GetControlllingPlayer())
        {
            StartDash();
        }

        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0)
            {
                StopDash();
            }
        }

        if (!canDash)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canDash = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            if (!sr.flipX)
            {
                rb.linearVelocity = new Vector2(dashSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(-dashSpeed, rb.linearVelocity.y);

            }

        }
    }

    void StartDash()
    {
        soundManager.PlaySFX(soundManager.dash);
        isDashing = true;
        canDash = false;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
    }

    void StopDash()
    {
        isDashing = false;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDashing)
        {

            IDamageableRed damageable = collision.gameObject.GetComponent<IDamageableRed>();

            if (damageable != null && !damageable.IsDead())
            {

                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                damageable.TakeDamage(dashDamage, knockbackDirection * dashKnockbackForce);


            }
        }
    }


}