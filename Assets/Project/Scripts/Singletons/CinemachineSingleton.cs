using JetBrains.Annotations;
using UnityEngine;

public class CinemachineSingleton : MonoBehaviour
{
    private Transform bigClone;
    private Transform smallClone;
    public static CinemachineSingleton Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetBigClone(Transform transformClone)
    {
        bigClone = transformClone;
    }
    public void SetSmallClone(Transform transformClone)
    {
        smallClone = transformClone;


    }
    public Transform GetBigClone()
    {
        return bigClone;
    }

    public Transform GetSmallClone()
    {
        return smallClone;
    }
}
