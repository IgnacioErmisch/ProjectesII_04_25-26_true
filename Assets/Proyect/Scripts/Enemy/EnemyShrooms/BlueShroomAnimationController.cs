using UnityEngine;
public class BlueShroomAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyGuardBlue guardEnemy;
    private Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        guardEnemy = GetComponent<EnemyGuardBlue>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

        if (guardEnemy.IsDead())
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsWalking", false);
            enabled = false;
            return;
        }


        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsWalking", isMoving);
    }

    public void PlayAttackAnimation()
    {
        if (!guardEnemy.IsDead())
        {
            animator.SetTrigger("Attack");
        }
    }

    public void PlayHitAnimation()
    {
        if (!guardEnemy.IsDead())
        {
            animator.SetTrigger("TakeDamage");
        }
    }
}