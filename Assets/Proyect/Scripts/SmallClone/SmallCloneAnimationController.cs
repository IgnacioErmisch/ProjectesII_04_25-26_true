using UnityEngine;

public class SmallCloneAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SmallCloneController smallCloneController;

    private SmallCloneMovment movement;
    private SmallCloneDoubleJump doubleJump;

    private void Awake()
    {
    }
    void Start()
    {
        animator = GetComponent<Animator>();

        if (smallCloneController == null)
        {
            smallCloneController = GetComponent<SmallCloneController>();
        }

        if (smallCloneController != null)
        {
            movement = smallCloneController.Movement;
            doubleJump = smallCloneController.DoubleJump;
        }
        GameManager.Instance.SetSmallController(this);
    }

    void Update()
    {
        PlayWalkAnimation();
        PlayJumpAnimation();
    }

    private void PlayWalkAnimation()
    {

        if (movement != null && movement.isMoving)
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
     
        if (doubleJump != null && doubleJump.isJumping)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

    }
    public void ResetAnimations()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isJumping", false);
    }
}
