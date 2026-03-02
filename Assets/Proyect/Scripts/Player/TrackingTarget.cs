using Unity.Cinemachine;
using UnityEngine;

public class TrackingTarget : MonoBehaviour
{
    public Transform playerTracking;
    public Transform bigCloneTracking;
    public Transform smallCloneTracking;
    [SerializeField] GameManager gameManager;
    private CinemachineCamera cm;
    void Start()
    {
        cm = GetComponent<CinemachineCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameManager.GetControlllingPlayer())
        {
            if (CheckBigClone())
            {
                cm.Target.TrackingTarget = bigCloneTracking;
            }
            else if(CheckSmallClone()){
                cm.Target.TrackingTarget = smallCloneTracking;
            }
        }
        else
        {
            cm.Target.TrackingTarget = playerTracking;
        }
    }

    private bool CheckBigClone()
    {
        bigCloneTracking = CinemachineSingleton.Instance.GetBigClone();

        if (bigCloneTracking != null)
            return true;
        else
            return false;
    }

    private bool CheckSmallClone()
    {
        smallCloneTracking = CinemachineSingleton.Instance.GetSmallClone();

        if (smallCloneTracking != null)
            return true;
        else
            return false;
    }

}
