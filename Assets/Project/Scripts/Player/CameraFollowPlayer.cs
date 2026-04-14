using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform character;
    public float smoothness;
    public Vector3 offset;
    private Vector3 speed;

    [SerializeField] private PlayerMovement playerMovement;

    void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        Vector3 targetOffset = offset;

        if (playerMovement.horizontal > 0.1f)
        {
            targetOffset = new Vector3(Mathf.Abs(offset.x), offset.y, offset.z);
        }

        else if (playerMovement.horizontal < -0.1f)
        {
            targetOffset = new Vector3(-Mathf.Abs(offset.x), offset.y, offset.z);
        }

        else if (playerMovement.horizontal == 0)
        {
            targetOffset = targetOffset = new Vector3(0, 2.5f, 0);
        }


        Vector3 targetPos = character.position + targetOffset;

       
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, smoothness);
    }
}
