using UnityEngine;

public class BreakableZone : MonoBehaviour
{

    [SerializeField] private AntiCloneZone antiCloneZone;
    [SerializeField] private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();

    }
    private void Break()
    {
        soundManager.PlaySFX(soundManager.deactivateAntiCloneZone);

        if (antiCloneZone != null)
        {
            antiCloneZone.GetComponent<CapsuleCollider2D>().enabled = false;
            antiCloneZone.GetComponentInChildren<ParticleSystem>().Stop();
            antiCloneZone.GetComponentInChildren<ParticleSystem>().Clear();
        }


        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Break();
        }
    }
}
