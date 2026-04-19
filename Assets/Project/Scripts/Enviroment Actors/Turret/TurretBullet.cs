using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float lifetime;
    private Transform target;
    [SerializeField] private float damage;

    public void Initialized(float speed, float lifetime, Transform target, float damage)
    {
        this.speed = speed;
        this.lifetime = lifetime;
        this.target = target;
        this.damage = damage;
    }
    void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null && !damageable.IsDead())
        {
            Vector2 knockbackDirection = Vector2.zero;
            damageable.TakeDamage(damage, knockbackDirection);

        }

        if (collision.CompareTag("Player") || collision.CompareTag("BigClone"))
        {
            Destroy(gameObject);
        }
    }
}
