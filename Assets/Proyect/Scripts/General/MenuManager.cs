using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject bindsMenu;
    [SerializeField] private GameObject firstButton;
    [SerializeField] private GameObject bindsButton;
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

    void Start()
    {
        pauseMenu.SetActive(false);
        bindsMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || inputActions.Gameplay.Pause.WasPressedThisFrame())
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            bindsMenu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstButton);
            Time.timeScale = 0;
        }
    }

    public void Exit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("menuPrincipal");
    }

    public void OpenBindsMenu()
    {
        bindsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(bindsButton);
    }

    public void CloseBindsMenu()
    {
        bindsMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstButton);
    }
}