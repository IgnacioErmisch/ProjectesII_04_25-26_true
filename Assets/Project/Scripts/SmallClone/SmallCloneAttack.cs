using UnityEngine;

public class SmallCloneAttack : MonoBehaviour
{
    [SerializeField] private int jumpDamage;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashKnockbackForce = 0f;

    private Rigidbody2D rb;
    private SoundManager soundManager;
    private SmallCloneDoubleJump doubleJumpSystem;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SmallCloneController controller = GetComponent<SmallCloneController>();
        if (controller != null)
            doubleJumpSystem = controller.DoubleJump;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("HitCollider")) return;

        soundManager.PlaySFX(soundManager.jumpOnEnemies);

        IDamageableBlue damageable = collision.gameObject.GetComponentInParent<IDamageableBlue>();
        if (damageable != null && !damageable.IsDead())
        {
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            damageable.TakeDamage(jumpDamage, knockbackDirection * dashKnockbackForce);
        }

        
        if (transform.position.y > collision.bounds.max.y)
        {
            if (doubleJumpSystem != null)
                doubleJumpSystem.ApplyBounce(jumpForce);
            else
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce); 
        }
    }
}