using UnityEngine;

public class RideBigClone : MonoBehaviour
{
    [SerializeField] private FixedJoint2D joint;
    [SerializeField] private SpriteRenderer bigCloneSpriteRenderer; 

    private Rigidbody2D player;
    private SpriteRenderer playerSpriteRenderer;
    private Transform cloneCanvas;
    private Vector3 canvasOriginalPosition;

    private void Update()
    {
        if (!GameManager.Instance.GetControlllingPlayer())
        {
            if (bigCloneSpriteRenderer != null && player != null)
            {
                bool cloneFacingRight = !bigCloneSpriteRenderer.flipX;
                PlayerMovement pm = player.GetComponent<PlayerMovement>();
                if (pm != null)
                    pm.ForceFlipVisualOnly(cloneFacingRight);
            }
        }
         
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.attachedRigidbody;
            playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            playerSpriteRenderer = null;
        }
    }

    public void AttachPlayer()
    {
        if (player == null) return;
        joint.enabled = true;
        joint.connectedBody = player;

        if (cloneCanvas == null)
        {
            cloneCanvas = GameManager.Instance.GetBigCloneCanvas();
            if (cloneCanvas != null)
                canvasOriginalPosition = cloneCanvas.localPosition;
        }

        if (playerSpriteRenderer != null)
            playerSpriteRenderer.sortingOrder = 1;
        if (cloneCanvas != null)
            cloneCanvas.localPosition = canvasOriginalPosition + new Vector3(0, 0.5f, 0);
    }

    public void DetachPlayer()
    {
        joint.enabled = false;
        if (playerSpriteRenderer != null)
            playerSpriteRenderer.sortingOrder = 3;
        if (cloneCanvas != null)
            cloneCanvas.localPosition = canvasOriginalPosition;
    }

    public bool IsAtached() => player != null;
}