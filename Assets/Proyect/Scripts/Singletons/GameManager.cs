using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private PerspectiveSwitch perspectiveSwitch;
    [SerializeField] private CloneSpawner cloneSpawner;
    private GameObject energyBigClone;
    private GameObject energySmallClone;
    private Transform bigCloneCanvas;
    private BigCloneAnimationController _bigCloneController;
    private SmallCloneAnimationController _smallCloneController;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Instance._player = _player;
            Instance.perspectiveSwitch = perspectiveSwitch;
            Instance.cloneSpawner = cloneSpawner;
            Destroy(gameObject);
        }
    }

    public GameObject GetPlayer()
    {
        return _player;
    }
    public void SetPlayer(GameObject player)
    {
        _player = player;
    }
    public void SetControlllingPlayer(PerspectiveSwitch perspective)
    {
        perspectiveSwitch = perspective;
    }
    public bool GetControlllingPlayer()
    {
        return perspectiveSwitch.controllingPlayer;
    }
    public bool GetCloneActive()
    {
        return cloneSpawner.cloneActive;
    }
    public void SetBigCloneEnergy(GameObject energyBigCloneImage)
    {
        energyBigClone = energyBigCloneImage;
    }
    public void SetSmallCloneEnergy(GameObject energySmallCloneImage)
    {
        energySmallClone = energySmallCloneImage;

    }
    public GameObject GetBigCloneEnergy()
    {
        return energyBigClone;
    }

    public GameObject GetSmallCloneEnergy()
    {
        return energySmallClone;
    }
    public void SetBigController(BigCloneAnimationController bigCloneController)
    {
        _bigCloneController = bigCloneController;
    }
    public BigCloneAnimationController GetBigController()
    {
        return _bigCloneController;
    }
    public void SetSmallController(SmallCloneAnimationController smallCloneController)
    {
        _smallCloneController = smallCloneController;
    }
    public SmallCloneAnimationController GetSmallController()
    {
        return _smallCloneController;
    }
    public bool IsPlayerDead()
    {
        HealthSystem healthSystem = _player.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            return healthSystem.IsDead();
        }
        return false;
    }

    public SpriteRenderer GetBigCloneSpriteRenderer()
    {
        if (_bigCloneController == null) return null;
        return _bigCloneController.GetComponentInChildren<SpriteRenderer>();
    }
    public void SetBigCloneCanvas(Transform canvas)
    {
        bigCloneCanvas = canvas;
    }

    public Transform GetBigCloneCanvas()
    {
        return bigCloneCanvas;
    }
}
