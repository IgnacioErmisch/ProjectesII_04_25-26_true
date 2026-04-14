using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwitchInterface : MonoBehaviour
{
    [SerializeField] public bool IsBigCloneSelected;
    [SerializeField] private Image BigClone;
    [SerializeField] private Image SmallClone;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Color bigSelectedColor;
    [SerializeField] private Color smallSelectedColor;

    [SerializeField] private bool smallCloneAvailable = true;
    [SerializeField] private bool bigCloneAvailable = true;

    Color darkColor = new Color32(41, 39, 39, 255);

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

    private void Start()
    {
        gameManager = GameManager.Instance;
        UpdateUI();
    }

    public void SelectBigClone()
    {
        if (!bigCloneAvailable) return; 

        if (!IsBigCloneSelected)
        {
            IsBigCloneSelected = true;
            UpdateUI();
        }
    }

    public void SelectSmallClone()
    {
        if (!smallCloneAvailable) return; 

        if (IsBigCloneSelected)
        {
            IsBigCloneSelected = false;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (bigCloneAvailable)
        {
            BigClone.color = IsBigCloneSelected ? bigSelectedColor : darkColor;
        }
    
        if (smallCloneAvailable)
        {
            SmallClone.color = IsBigCloneSelected ? darkColor : smallSelectedColor;
        }
       
    }

    public bool IsSmallCloneAvailable()
    {
        return smallCloneAvailable;
    }

    public bool IsBigCloneAvailable()
    {
        return bigCloneAvailable;
    }
}