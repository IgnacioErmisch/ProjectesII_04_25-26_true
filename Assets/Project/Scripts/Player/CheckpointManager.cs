using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public List<Vector3> checkpoints = new List<Vector3>();

    public void AddCheckPoint(Vector3 transform)
    {
        if (!checkpoints.Contains(transform))
        {
            checkpoints.Add(transform); 
        }
    }

    public Vector3 GetLastCheckpoint()
    {
       return checkpoints[checkpoints.Count - 1];

    }
}
