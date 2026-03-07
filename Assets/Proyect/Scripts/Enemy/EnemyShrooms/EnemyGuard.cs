using UnityEngine;

public class BasicGuardEnemy : MonoBehaviour, IDamageableRed
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 50f;

    [Header("Patrol Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private GameObject leftLimit;
    [SerializeField] private GameObject rightLimit;

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float chaseDuration = 2f;

    [Header("Attack Settings")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private Transform attackPoint;
    private egAnimationController animController;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private HealthSystem healthSystem;
    private RadiusDetectionSystem detectionSystem;
    private KnockbackSystem knockbackSystem;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private int jumpsHitDamage;
    

    private enum State { Patrol, Chase, Attack, Dead }
    private State currentState = State.Patrol;
    private bool movingRight = true;
    private float chaseTimer;
    private Transform playerTarget;
    private float lastAttackTime;
    private Vector3 originalAttackPointLocalPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animController = GetComponent<egAnimationController>();

        healthSystem = gameObject.AddComponent<HealthSystem>();
        healthSystem.SetMaxHealth(maxHealth);
        healthSystem.OnDeath += HandleDeath;

        detectionSystem = gameObject.AddComponent<RadiusDetectionSystem>();
        detectionSystem.SetDetectionRadius(detectionRadius);
        detectionSystem.SetTargetLayer(playerLayer);

        knockbackSystem = gameObject.AddComponent<KnockbackSystem>();
        knockbackSystem.SetKnockbackForce(knockbackForce);
        knockbackSystem.SetKnockbackDuration(knockbackDuration);

        if (rb != null)
        {
            rb.gravityScale = 3f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (attackPoint == null)
        {
            attackPoint = transform;
        }
        else
        {
            originalAttackPointLocalPosition = attackPoint.localPosition;
        }

        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }

    private void Update()
    {
        if (healthSystem.IsDead()) return;

        knockbackSystem.Update();

        if (knockbackSystem.IsKnockedBack()) return;

        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Chase:
                UpdateChase();
                break;
            case State.Attack:
                UpdateAttack();
                break;
        }
    }

    private void UpdatePatrol()
    {
        Patrol();

        if (detectionSystem.DetectTarget())
        {
            playerTarget = detectionSystem.GetTarget();
            TransitionToChase();
        }
    }

    private void Patrol()
    {
        if (leftLimit == null || rightLimit == null) return;

        Vector2 velocity = rb.linearVelocity;

        if (movingRight)
        {
            velocity.x = patrolSpeed;
            FlipSprite(false);

            if (transform.position.x >= rightLimit.transform.position.x)
            {
                movingRight = false;
            }
        }
        else
        {
            velocity.x = -patrolSpeed;
            FlipSprite(true);

            if (transform.position.x <= leftLimit.transform.position.x)
            {
                movingRight = true;
            }
        }

        rb.linearVelocity = velocity;
    }

    private void TransitionToChase()
    {
        currentState = State.Chase;
        chaseTimer = chaseDuration;
    }

    private void UpdateChase()
    {
        chaseTimer -= Time.deltaTime;

        if (playerTarget == null || chaseTimer <= 0)
        {
            currentState = State.Patrol;
            playerTarget = null;
            return;
        }

        if (IsPlayerInAttackRange())
        {
            TransitionToAttack();
            return;
        }

        ChasePlayer();

        if (detectionSystem.DetectTarget())
        {
            chaseTimer = chaseDuration;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction.x * chaseSpeed;
        rb.linearVelocity = velocity;
        FlipSprite(direction.x < 0);
    }

    private void TransitionToAttack()
    {
        currentState = State.Attack;
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    private void UpdateAttack()
    {
        if (playerTarget == null)
        {
            currentState = State.Patrol;
            return;
        }

        if (!IsPlayerInAttackRange())
        {
            TransitionToChase();
            return;
        }

        Vector2 directionToPlayer = (playerTarget.position - transform.position).normalized;
        FlipSprite(directionToPlayer.x < 0);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            PerformAttack();
        }
    }

    private bool IsPlayerInAttackRange()
    {
        if (playerTarget == null) return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            if (hit.transform == playerTarget)
            {
                return true;
            }
        }

        return false;
    }

    private void PerformAttack()
    {
        lastAttackTime = Time.time;

        
        if (animController != null)
        {
            animController.PlayAttackAnimation();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        soundManager.PlaySFX(soundManager.redAttack);

        foreach (Collider2D hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead())
            {
                Vector2 knockbackDirection = (hit.transform.position - transform.position).normalized;
                damageable.TakeDamage(attackDamage, knockbackDirection);
            }
        }
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if (healthSystem.IsDead()) return;

        healthSystem.TakeDamage(damage, knockbackDirection);
        knockbackSystem.ApplyKnockback(knockbackDirection);
  
        if (animController != null)
        {
            animController.PlayHitAnimation();
        }
    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
    }

    public float GetCurrentHealth()
    {
        return healthSystem.GetCurrentHealth();
    }

    private void HandleDeath()
    {
        currentState = State.Dead;
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 2f);
    }

    private void FlipSprite(bool flipLeft)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flipLeft;
        }

        if (attackPoint != null && attackPoint != transform)
        {
            Vector3 newPosition = originalAttackPointLocalPosition;

            if (flipLeft)
            {
                newPosition.x = -Mathf.Abs(originalAttackPointLocalPosition.x);
            }
            else
            {
                newPosition.x = Mathf.Abs(originalAttackPointLocalPosition.x);
            }

            attackPoint.localPosition = newPosition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (leftLimit != null && rightLimit != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(leftLimit.transform.position, rightLimit.transform.position);
        }
    }
}