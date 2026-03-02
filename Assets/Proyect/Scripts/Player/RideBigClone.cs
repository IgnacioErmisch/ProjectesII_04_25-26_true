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

    void Start()
    {
        originalParent = this.transform.parent;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (!isOnClone) return;

        if (!GameManager.Instance.GetControlllingPlayer())
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            spriteRenderer.sortingOrder = 1;
            if (cloneCanvas != null)
                cloneCanvas.localPosition = canvasOriginalPosition + new Vector3(0, 0.5f, 0);
            if (cloneSpriteRenderer != null)
                spriteRenderer.flipX = cloneSpriteRenderer.flipX;
        }
        else
        {
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
            cloneSpriteRenderer = GameManager.Instance.GetBigCloneSpriteRenderer();
            cloneCanvas = GameManager.Instance.GetBigCloneCanvas();
            if (cloneCanvas != null)
                canvasOriginalPosition = cloneCanvas.localPosition;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("topCollision") && !GameManager.Instance.GetControlllingPlayer() && this.transform.parent != other.transform.parent)
        {
            this.transform.SetParent(other.transform.parent);
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("topCollision"))
        {
            isOnClone = false;
            cloneSpriteRenderer = null;
            spriteRenderer.sortingOrder = 3;
            if (cloneCanvas != null)
                cloneCanvas.localPosition = canvasOriginalPosition;
            cloneCanvas = null;
            this.transform.SetParent(originalParent);
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}