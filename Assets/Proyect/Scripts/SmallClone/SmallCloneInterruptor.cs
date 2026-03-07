using UnityEngine;

public class SmallCloneInterruptor : MonoBehaviour
{
    [SerializeField] private GameObject interruptorWall;
    [SerializeField] private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SmallClone"))
        {
            soundManager.PlaySFX(soundManager.buttonDoor);
            Destroy(gameObject);
            Destroy(interruptorWall);
        }
    }
}
