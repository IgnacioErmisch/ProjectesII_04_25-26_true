using UnityEngine;

public class AntiGravityField : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        CloneGravity clone = other.GetComponent<CloneGravity>();
        if (clone != null)
        {
            clone.InvertGravity();
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