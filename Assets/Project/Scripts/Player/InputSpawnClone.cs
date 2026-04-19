using UnityEngine;

public class InputSpawnClone : MonoBehaviour
{
    [SerializeField] private CloneSpawner CloneSpawner;
    [SerializeField] private SwitchInterface switchInterface;
    [SerializeField] private Transform cloneSpawnerPoint;
    [SerializeField] private PlayerJump playerJump;
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
        if (inputActions.Gameplay.SelectBigClone.triggered)
        {
            if (IsAnyCloneActive())
            {
                CloneSpawner.TryDespawnClone();
            }
            else if (playerJump.isGrounded && switchInterface.IsBigCloneAvailable())
            {
                switchInterface.SelectBigClone();
                CloneSpawner.TrySpawnClone();
            }
        }

        if (inputActions.Gameplay.SelectSmallClone.triggered)
        {
            if (IsAnyCloneActive())
            {
                CloneSpawner.TryDespawnClone();
            }
            else if (playerJump.isGrounded && switchInterface.IsSmallCloneAvailable())
            {
                switchInterface.SelectSmallClone();
                CloneSpawner.TrySpawnClone();
            }
        }
    }

    public bool IsAnyCloneActive()
    {
        return CloneSpawner.CloneActive;
    }
}