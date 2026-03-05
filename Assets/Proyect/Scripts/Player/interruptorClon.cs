using UnityEngine;

public class BreakableZone : MonoBehaviour
{

    [SerializeField] private AntiCloneZone antiCloneZone;

    private void Break()
    {

        if (antiCloneZone != null)
        {
            antiCloneZone.GetComponent<CapsuleCollider2D>().enabled = false;
            antiCloneZone.GetComponentInChildren<ParticleSystem>().Stop();
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
