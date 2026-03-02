using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerJump playerJump;
    [SerializeField] private PlayerCombatController playerCombatController;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayWalkAnimation();
        PlayJumpAnimation();
        PlayDeathAnimation();
    }

    private void PlayWalkAnimation()
    {
        if (playerMovement.isMoving)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void PlayJumpAnimation()
    {
        if (playerJump.isJumping)
        {
            animator.SetBool("isJumping", true);

        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    public void PlayDeathAnimation()
    {
        if (playerCombatController.IsDead())
        {
            animator.SetTrigger("isDeath");


        }
    }

    public void ResetAnimations()
    {
        animator.SetBool("isDeath", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isJumping", false);
    }
}
