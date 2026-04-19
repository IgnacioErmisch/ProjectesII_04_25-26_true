using UnityEngine;
using System.Collections;

public interface IDamageable
{
    void TakeDamage(float damage, Vector2 knockbackDirection);
    bool IsDead();
    float GetCurrentHealth();
}

public interface IDamageableBlue
{
    void TakeDamage(float damage, Vector2 knockbackDirection);
    bool IsDead();
    float GetCurrentHealth();
}

public interface IDamageableRed
{
    void TakeDamage(float damage, Vector2 knockbackDirection);
    bool IsDead();
    float GetCurrentHealth();
}

public interface IAttacker
{
    void Attack();
    float GetAttackDamage();
    float GetAttackRange();
}

public interface IDetectionSystem
{
    bool DetectTarget();
    Transform GetTarget();
}

public interface IMovementSystem
{
    void Move(Vector2 direction);
    void Stop();
}

public interface IEnemyState
{
    void Enter();
    void Update();
    void Exit();
}


public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead;

    public event System.Action<float> OnHealthChanged;
    public event System.Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void IsDeadTrue()
    {
        isDead = !isDead;
    }
    public bool IsDead()
    {
        return isDead;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public float ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        return currentHealth;
    }
}


public class KnockbackSystem : MonoBehaviour
{
    [SerializeField] private float knockbackForce = 5f;
    [SerializeField] private float knockbackDuration = 0.3f;

    private Rigidbody2D rb;
    private float knockbackTimer;
    private bool isKnockedBack;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetKnockbackForce(float force)
    {
        knockbackForce = force;
    }

    public void SetKnockbackDuration(float duration)
    {
        knockbackDuration = duration;
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (rb == null) return;

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        rb.linearVelocity = direction.normalized * knockbackForce;
    }

    public void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}


public class RadiusDetectionSystem : MonoBehaviour, IDetectionSystem
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private LayerMask targetLayer;

    private Transform owner;
    private Transform currentTarget;

    private void Awake()
    {
        owner = transform;
    }

    public void SetDetectionRadius(float radius)
    {
        detectionRadius = radius;
    }

    public void SetTargetLayer(LayerMask layer)
    {
        targetLayer = layer;
    }

    public bool DetectTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(owner.position, detectionRadius, targetLayer);

        if (hits.Length > 0)
        {
            currentTarget = hits[0].transform;
            return true;
        }

        currentTarget = null;
        return false;
    }

    public Transform GetTarget() => currentTarget;

    public void DrawGizmos()
    {
        if (owner != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(owner.position, detectionRadius);
        }
    }
}


public class PlayerMeleeAttack : MonoBehaviour, IAttacker
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float knockbackForce = 10f;

    private float lastAttackTime;

    public event System.Action OnAttackPerformed;

    private void Awake()
    {
        lastAttackTime = -attackCooldown;
    }

    public void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;
        OnAttackPerformed?.Invoke();

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null && !damageable.IsDead())
            {
                Vector2 knockbackDirection = (enemy.transform.position - attackPoint.position).normalized;
                knockbackDirection.y = 0.3f;

                damageable.TakeDamage(attackDamage, knockbackDirection * knockbackForce);
            }
        }
    }

    public float GetAttackDamage() => attackDamage;
    public float GetAttackRange() => attackRange;

    public void DrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}

public class EnemyStateContext
{
    public Transform transform;
    public IDetectionSystem detectionSystem;
    public IMovementSystem movementSystem;
    public IAttacker attackSystem;
    public float chaseSpeed;
    public float attackRange;
    public float attackCooldown;
    public float lastAttackTime;
}


public class PatrolState : MonoBehaviour, IEnemyState
{
    public EnemyStateContext context;
    public IMovementSystem patrolMovement;

    public void Enter()
    {

    }

    public void Update()
    {

    }

    public void Exit()
    {
        patrolMovement?.Stop();
    }
}


public class ChaseState : MonoBehaviour, IEnemyState
{
    public EnemyStateContext context;

    public void Enter()
    {

    }

    public void Update()
    {
        Transform target = context.detectionSystem.GetTarget();
        if (target != null)
        {
            Vector2 direction = (target.position - context.transform.position).normalized;
            context.movementSystem.Move(direction);
        }
    }

    public void Exit()
    {
        context.movementSystem.Stop();
    }
}


public class AttackState : MonoBehaviour, IEnemyState
{
    public EnemyStateContext context;

    public void Enter()
    {
        context.movementSystem.Stop();
    }

    public void Update()
    {
        if (Time.time - context.lastAttackTime >= context.attackCooldown)
        {
            context.attackSystem?.Attack();
            context.lastAttackTime = Time.time;
        }
    }

    public void Exit()
    {

    }
}