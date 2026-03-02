using UnityEngine;

public class RotatingWheel : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 45f; 
    public bool rotateClockwise = true;

    void Update()
    {
        float direction = rotateClockwise ? -1f : 1f;
        transform.Rotate(0f, 0f, direction * rotationSpeed * Time.deltaTime);
    }
}