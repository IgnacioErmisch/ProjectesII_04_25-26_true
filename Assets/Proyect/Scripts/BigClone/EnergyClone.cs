using UnityEngine;
using UnityEngine.UI;

public class EnergyClone : MonoBehaviour
{
    
    private Image myImage;
    private EnergyController myController;
    void Start()
    {
        myImage = GetComponent<Image>();
        myController = GameManager.Instance.GetPlayer().GetComponentInChildren<EnergyController>();
        myController.GetBigClone(myImage);
    }

    
}
