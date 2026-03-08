using UnityEngine;

public class PlatformClone : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float maxDistance = 5f;
    private int direction = 0;
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private SoundManager soundManager;

    private AudioSource platformAudioSource;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }

    void Start()
    {
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
        SetArrowsActive(true);
        platformAudioSource = gameObject.AddComponent<AudioSource>();
        platformAudioSource.loop = true;
        platformAudioSource.playOnAwake = false;
        platformAudioSource.clip = soundManager.movingPlatform;
        platformAudioSource.outputAudioMixerGroup = soundManager.sfxMixerGroup;
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

    private void PlayPlatformLoop()
    {
        if (platformAudioSource.isPlaying) return;
        platformAudioSource.Play();
    }

    private void StopPlatformLoop()
    {
        platformAudioSource.Stop();
    }

    public void MoveUp()
    {
        direction = 1;
        PlayPlatformLoop();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
        SetArrowsActive(false);
    }

    public void MoveDown()
    {
        direction = -1;
        PlayPlatformLoop();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;
        SetArrowsActive(false);
    }

    public void Stop()
    {
        direction = 0;
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
        StopPlatformLoop();
        SetArrowsActive(true);
    }

    private void SetArrowsActive(bool active)
    {
        foreach (GameObject arrow in arrows)
            arrow.SetActive(active);
    }
}