using UnityEngine;

public class RideBigClone : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bigCloneSpriteRenderer;
    [SerializeField] private Transform playerAnchorPoint;
    private Rigidbody2D player;
    private SpriteRenderer playerSpriteRenderer;
    private Transform cloneCanvas;
    private Vector3 canvasOriginalPosition;
    private Transform playerTransform;
    private float originalGravity;
    private bool isAttached = false;
    private float originalMass = 1000f;
    private bool playerInsideTrigger = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isAttached || player == null || playerAnchorPoint == null) return;
        player.MovePosition(playerAnchorPoint.position);
        player.MoveRotation(rb.rotation);
        player.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (!isAttached) return;
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
        if (playerTransform != null)
            playerTransform.rotation = transform.rotation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = true;
            player = collision.attachedRigidbody;
            playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
            playerTransform = player.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTrigger = false;
            if (isAttached)
                DetachPlayer();
            else
            {
                player = null;
                playerSpriteRenderer = null;
                playerTransform = null;
            }
        }
    }

    public void AttachPlayer()
    {
        if (player == null) return;
        isAttached = true;
        originalMass = player.mass;
        player.mass = 10f;
        originalGravity = player.gravityScale;
        player.gravityScale = 0f;
        player.linearVelocity = Vector2.zero;

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
        isAttached = false;

        if (playerTransform != null)
            playerTransform.rotation = Quaternion.identity;

        if (player != null)
        {
            player.mass = originalMass;
            player.gravityScale = originalGravity;
            player.MoveRotation(0f);
        }

        if (playerSpriteRenderer != null)
            playerSpriteRenderer.sortingOrder = 3;
        if (cloneCanvas != null)
            cloneCanvas.localPosition = canvasOriginalPosition;

        if (!playerInsideTrigger)
        {
            player = null;
            playerSpriteRenderer = null;
            playerTransform = null;
        }
    }

    public bool IsAtached() => player != null && isAttached;

    private void OnDestroy()
    {
        DetachPlayer();
    }
}