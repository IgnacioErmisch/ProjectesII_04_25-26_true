using UnityEngine;

public class aeAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AerialSentinelEnemy aerial;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        aerial = GetComponent<AerialSentinelEnemy>();
    }

    private void Update()
    {
        if (aerial.IsDead())
        {
            animator.SetTrigger("Die");
            animator.SetBool("IsWalking", false);
            enabled = false;
            return;
        }

        // Siempre está caminando mientras está vivo
        animator.SetBool("IsWalking", true);
    }

    public void PlayAttackAnimation()
    {
        if (!aerial.IsDead())
        {
            animator.SetTrigger("Attack");
        }
    }

    public void PlayHitAnimation()
    {
        if (!aerial.IsDead())
        {
            animator.SetTrigger("TakeDamage");
        }
    }
}