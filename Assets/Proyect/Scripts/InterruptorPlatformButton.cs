using UnityEngine;

public class InterruptorPlatformButton : MonoBehaviour
{
    [SerializeField] private GameObject platforms;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Color colorOff;
    [SerializeField] private SoundManager soundManager;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        platforms.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        platforms.SetActive(true);
        spriteRenderer.color = Color.green;
        soundManager.PlaySFX(soundManager.showPlatforms);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        platforms.SetActive(false);
        spriteRenderer.color = colorOff;
    }
}
