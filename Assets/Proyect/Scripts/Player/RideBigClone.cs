using UnityEngine;

public class RideBigClone : MonoBehaviour
{
    [SerializeField] private FixedJoint2D joint;
    private Rigidbody2D player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.attachedRigidbody;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
        }
    }

    public void AttachPlayer()
    {
        if (player == null)
            return;

        joint.enabled = true;
        joint.connectedBody = player;
    }

    public void DetachPlayer()
    {
        joint.enabled = false;
    }
    public bool IsAtached()
    {
        return player != null;
    }
}