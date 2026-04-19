using UnityEngine;

public class ButtonPlatformClone : MonoBehaviour
{
    [SerializeField] private PlatformClone platform;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BigClone"))
        {
            platform.MoveUp();
        }
        else if (collision.gameObject.CompareTag("SmallClone"))
        {
            platform.MoveDown();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BigClone") ||
            collision.gameObject.CompareTag("SmallClone"))
        {
            platform.Stop();
        }
    }
}