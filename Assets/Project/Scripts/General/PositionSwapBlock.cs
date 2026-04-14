using UnityEngine;

public class PositionSwapBlock : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CloneSpawner bigCloneSpawner;
    [SerializeField] private CloneSpawner smallCloneSpawner;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private SoundManager soundManager;


    private AudioSource audioSource;
    private float swapCooldown = 5f;
    private float lastSwapTime = -99f;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void Update()
    {
        if (spriteRenderer == null) return;

        float timeSinceSwap = Time.time - lastSwapTime;

        if (timeSinceSwap < swapCooldown)
        {
            float t = timeSinceSwap / swapCooldown;
            spriteRenderer.color = Color.Lerp(Color.black, originalColor, t);
        }
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool isPlayer = collision.CompareTag("Player");
        bool isBigClone = collision.CompareTag("BigClone");
        bool isSmallClone = collision.CompareTag("SmallClone");
        bool isClone = isBigClone || isSmallClone;

        if (!isPlayer && !isClone)
            return;

        if (Time.time - lastSwapTime < swapCooldown)
            return;

        CloneSpawner activeSpawner = GetActiveSpawner();
        if (activeSpawner == null)
            return;

        GameObject currentClone = activeSpawner.GetCurrentClone();

        if (currentClone != null && (isPlayer || collision.gameObject == currentClone))
        {
            SwapPositions(activeSpawner);
        }
    }

    private void SwapPositions(CloneSpawner activeSpawner)
    {
        GameObject currentClone = activeSpawner.GetCurrentClone();

        Vector3 playerPosition = player.transform.position;
        Vector3 clonePosition = currentClone.transform.position;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        Rigidbody2D cloneRb = currentClone.GetComponent<Rigidbody2D>();

        Vector2 playerVelocity = Vector2.zero;
        Vector2 cloneVelocity = Vector2.zero;

        if (playerRb != null) playerVelocity = playerRb.linearVelocity;
        if (cloneRb != null) cloneVelocity = cloneRb.linearVelocity;

        Vector3 offset = new Vector3(1f, 0, 0);
        player.transform.position = clonePosition + offset;
        currentClone.transform.position = playerPosition + offset;

        if (playerRb != null) playerRb.linearVelocity = cloneVelocity;
        if (cloneRb != null) cloneRb.linearVelocity = playerVelocity;

        SwitchCameraToPlayer();
        soundManager.PlaySFX(soundManager.changePosition);

        lastSwapTime = Time.time; 
    }

    private void SwitchCameraToPlayer()
    {
        if (playerCamera == null || player == null)
            return;

        playerCamera.transform.SetParent(player.transform);
        playerCamera.transform.localPosition = new Vector3(2, 2, -5);

        if (perspectiveSwitch != null)
            perspectiveSwitch.SwitchToPlayer();
    }

    private CloneSpawner GetActiveSpawner()
    {
        if (bigCloneSpawner != null && bigCloneSpawner.cloneActive)
            return bigCloneSpawner;
        else if (smallCloneSpawner != null && smallCloneSpawner.cloneActive)
            return smallCloneSpawner;

        return null;
    }
}