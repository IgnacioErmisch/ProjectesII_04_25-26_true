using UnityEngine;

public class PerspectiveSwitch : MonoBehaviour
{
    [SerializeField] private CloneSpawner bigCloneSpawner;
    [SerializeField] private CloneSpawner smallCloneSpawner;
    [SerializeField] private PlayerAnimatorController playerAnimatorController;
    [SerializeField] private BigCloneAnimationController bigCloneAnimatorController;
    [SerializeField] private SmallCloneAnimationController smallCloneAnimatorController;
    public GameObject player;
    public Camera playerCamera;
    public bool controllingPlayer = true;

    private Controller inputActions;

    private void Awake()
    {
        inputActions = new Controller();
    }

    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    void Update()
    {
        SetClones();

        if (inputActions.Gameplay.SwitchCamera.triggered)
        {
            SwitchCamera();
        }

        if (!controllingPlayer)
        {
            playerAnimatorController.ResetAnimations();

        }
        else
        {
            if (bigCloneAnimatorController != null)
            {
                GameManager.Instance.GetBigController().ResetAnimations();
            }
            if (smallCloneAnimatorController != null)
            {
                GameManager.Instance.GetSmallController().ResetAnimations();
            }

        }
    }

    private void SetClones()
    {
        if (bigCloneAnimatorController == null)
        {
            bigCloneAnimatorController = GameManager.Instance.GetBigController();
        }
        if (smallCloneAnimatorController == null)
        {
            smallCloneAnimatorController = GameManager.Instance.GetSmallController();
        }
    }

    private void Start()
    {
        controllingPlayer = true;
        GameManager.Instance.SetControlllingPlayer(this);

    }

    private void SwitchCamera()
    {
        CloneSpawner activeSpawner = GetActiveSpawner();

        if (activeSpawner == null)
            return;

        GameObject currentClone = activeSpawner.GetCurrentClone();
        Rigidbody2D cloneRb = currentClone.GetComponent<Rigidbody2D>();

        if (controllingPlayer)
        {
            playerCamera.transform.SetParent(currentClone.transform);
            playerCamera.transform.localPosition = new Vector3(2, 1, -5);

        }
        else
        {
            playerCamera.transform.SetParent(player.transform);
            playerCamera.transform.localPosition = new Vector3(2, 2, -5);

        }



        controllingPlayer = !controllingPlayer;
    }

    private CloneSpawner GetActiveSpawner()
    {
        if (bigCloneSpawner.cloneActive)
        {
            return bigCloneSpawner;
        }
        else if (smallCloneSpawner.cloneActive)
        {
            return smallCloneSpawner;
        }

        return null;
    }

    public void SwitchToClone()
    {
        controllingPlayer = false;
    }


    public void SwitchToPlayer()
    {
        controllingPlayer = true;
    }

    public bool GetControllingPlayer()
    {
        return controllingPlayer;
    }
}