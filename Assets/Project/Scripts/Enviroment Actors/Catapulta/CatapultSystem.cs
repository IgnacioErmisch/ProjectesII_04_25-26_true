using UnityEngine;

public class CatapultSystem : MonoBehaviour
{
    [Header("Platform References")]
    [SerializeField] private CatapultPlatform catapultPlatform;
    [SerializeField] private LaunchPlatform launchPlatform;
    [SerializeField] private GameObject player;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;

    [Header("Settings")]
    [SerializeField] private float launchDelay = 0f;

    private bool isLaunching = false;

    private void Awake()
    {
        if (catapultPlatform == null)
            catapultPlatform = FindFirstObjectByType<CatapultPlatform>();

        if (launchPlatform == null)
            launchPlatform = FindFirstObjectByType<LaunchPlatform>();
    }

    private void SwitchCameraToPlayer()
    {
        if (playerCamera == null || player == null) return;

        playerCamera.transform.SetParent(player.transform);
        playerCamera.transform.localPosition = new Vector3(2, 2, -5);

        if (perspectiveSwitch != null)
            perspectiveSwitch.SwitchToPlayer();
    }

    public void OnCatapultActivated(float impactForce)
    {
        if (isLaunching) return;
        isLaunching = true;

        SwitchCameraToPlayer();

        if (launchPlatform != null)
        {
            if (launchDelay <= 0f)
                TriggerLaunch();
            else
                Invoke(nameof(TriggerLaunch), launchDelay);
        }
    }

    private void TriggerLaunch()
    {
        if (launchPlatform == null) return;

        if (!launchPlatform.IsPlayerReady())
        {
            isLaunching = false;
            return;
        }

        launchPlatform.LaunchPlayer();
        Invoke(nameof(ResetLaunchFlag), 2f);
    }

    private void ResetLaunchFlag()
    {
        isLaunching = false;
    }

    public void ResetSystem()
    {
        CancelInvoke();
        isLaunching = false;

        if (catapultPlatform != null)
            catapultPlatform.ForceReset();

        if (launchPlatform != null)
            launchPlatform.ForceReset();
    }
}