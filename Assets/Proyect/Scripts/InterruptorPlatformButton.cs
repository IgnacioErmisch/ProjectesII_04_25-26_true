using UnityEngine;

public class InterruptorPlatformButton : MonoBehaviour
{
    [SerializeField] private GameObject platforms;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Color colorOff;
    private SpriteRenderer spriteRenderer;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platforms.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        
            platforms.SetActive(true);
            spriteRenderer.color = Color.green;
        
    }
    void OnTriggerExit2D(Collider2D other)
    {
        platforms.SetActive(false);
        spriteRenderer.color = colorOff;
    }
}
