using UnityEngine;

public class RideBigClone : MonoBehaviour
{
    private Transform originalParent;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer cloneSpriteRenderer;
    private Transform cloneCanvas;
    private Vector3 canvasOriginalPosition;
    private bool isOnClone = false;
    private Transform cloneTransform;
    private Vector3 lastClonePosition;
    private PlayerMovement playerMovement;

    public bool IsRiding => isOnClone && !GameManager.Instance.GetControlllingPlayer();

    void Start()
    {
        originalParent = this.transform.parent;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        if (!IsRiding) return;
        if (cloneTransform == null) return;

        rb.linearVelocity = Vector2.zero;
        Vector3 cloneDelta = cloneTransform.position - lastClonePosition;
        lastClonePosition = cloneTransform.position;

        if (cloneDelta.sqrMagnitude > 0f)
            rb.MovePosition(rb.position + new Vector2(cloneDelta.x, cloneDelta.y));
    }

    void Update()
    {
        if (!isOnClone) return;

        if (!GameManager.Instance.GetControlllingPlayer())
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f;
            spriteRenderer.sortingOrder = 1;

            if (cloneCanvas != null)
                cloneCanvas.localPosition = canvasOriginalPosition + new Vector3(0, 0.5f, 0);

            if (cloneSpriteRenderer != null && playerMovement != null)
            {
                bool cloneFacingRight = !cloneSpriteRenderer.flipX;
                playerMovement.ForceFlipVisualOnly(cloneFacingRight);
            }
        }
        else
        {
            rb.gravityScale = 1f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            spriteRenderer.sortingOrder = 3;

            if (cloneCanvas != null)
                cloneCanvas.localPosition = canvasOriginalPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("topCollision"))
        {
            isOnClone = true;
            cloneTransform = other.transform.parent;
            lastClonePosition = cloneTransform.position;
            cloneSpriteRenderer = GameManager.Instance.GetBigCloneSpriteRenderer();
            cloneCanvas = GameManager.Instance.GetBigCloneCanvas();
            if (cloneCanvas != null)
                canvasOriginalPosition = cloneCanvas.localPosition;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("topCollision"))
        {
            isOnClone = false;
            cloneTransform = null;
            cloneSpriteRenderer = null;
            spriteRenderer.sortingOrder = 3;
            rb.gravityScale = 1f;
            rb.bodyType = RigidbodyType2D.Dynamic;

            if (cloneCanvas != null)
                cloneCanvas.localPosition = canvasOriginalPosition;
            cloneCanvas = null;
        }
    }
}