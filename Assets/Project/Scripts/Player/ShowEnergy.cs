using UnityEngine;

public class ShowEnergy : MonoBehaviour
{
    public GameObject energyPlayer;
    private GameObject energyBigClone;
    private GameObject energySmallClone;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private CloneSpawner bigCloneSpawner;
    [SerializeField] private CloneSpawner smallCloneSpawner;
    [SerializeField] private EnergyController energyController;

    private void Start()
    {
        energyPlayer.SetActive(false);
        if (energyBigClone != null) energyBigClone.SetActive(false);
        if (energySmallClone != null) energySmallClone.SetActive(false);
    }

    void Update()
    {
       
        if (energyBigClone == null)
        {
            energyBigClone = GameManager.Instance.GetBigCloneEnergy();
            
        }
        if (energySmallClone == null)
        {
            energySmallClone = GameManager.Instance.GetSmallCloneEnergy();
            
        }

        UpdateEnergyVisibility();
    }

    private void UpdateEnergyVisibility()
    {

        if (perspectiveSwitch.controllingPlayer && energyController.GetCurrentEnergy() != energyController.GetMaxEnergy())
        {
           
            energyPlayer.SetActive(true);
            if (energyBigClone != null && bigCloneSpawner.CloneActive) 
                energyBigClone.SetActive(false);
            if (energySmallClone != null && smallCloneSpawner.CloneActive) 
                energySmallClone.SetActive(false);
        
        }

        if (!perspectiveSwitch.controllingPlayer)
        {
            energyPlayer.SetActive(false);
            if (energyBigClone != null && bigCloneSpawner.CloneActive)
            {
                energyBigClone.SetActive(true);
            }
            if (energySmallClone != null && smallCloneSpawner.CloneActive)
            {
                energySmallClone.SetActive(true);
            }
        }

        if (!bigCloneSpawner.CloneActive && !smallCloneSpawner.CloneActive && energyController.GetCurrentEnergy() == energyController.GetMaxEnergy())
        {

            energyPlayer.SetActive(false);
        }
      
    }
}