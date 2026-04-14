using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Transform checkpointTransform;
    [SerializeField] CheckpointManager checkpointManager;
    void Start()
    {
        checkpointTransform = GetComponent<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        checkpointManager.AddCheckPoint(checkpointTransform.position);
    }
}
