using UnityEngine;

public class AntiGravityField : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        CloneGravity clone = other.GetComponent<CloneGravity>();
        if (clone != null)
        {
            clone.InvertGravity();
            soundManager.PlaySFX(soundManager.antiGravity);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        CloneGravity clone = other.GetComponent<CloneGravity>();
        if (clone != null)
        {
            clone.InvertGravity();
        }
    }
}