using UnityEngine;

public class PlatformClone : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float maxDistance = 5f;
    private int direction = 0;
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    void Update()
    {
        if (direction != 0)
        {
            float distanceFromStart = transform.position.y - startPosition.y;

            if (direction == 1 && distanceFromStart >= maxDistance)
            {
                Stop();
                return;
            }
            else if (direction == -1 && distanceFromStart <= -maxDistance)
            {
                Stop();
                return;
            }

            transform.Translate(Vector3.up * direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void MoveUp()
    {
        direction = 1;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    public void MoveDown()
    {
        direction = -1;
        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;
    }

    public void Stop()
    {
        direction = 0;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}