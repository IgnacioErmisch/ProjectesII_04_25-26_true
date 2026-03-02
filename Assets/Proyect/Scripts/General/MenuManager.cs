using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject bindsMenu;
    void Start()
    {
        pauseMenu.SetActive(false);
        bindsMenu.SetActive(false);
    
    }

    // Update is called once per frame
    void Update()
    {
        OpenMenu();
       
    }

    public void OpenMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseMenu.SetActive(true);
            Time.timeScale= 0;
         
        }    
    }
    public void Return()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void Exit()
    {
        SceneManager.LoadScene("menuPrincipal");
    }

    public void OpenBindsMenu()
    {
        bindsMenu.SetActive(true);
    }
    public void CloseBindsMenu()
    {
        bindsMenu.SetActive(false);
    }

}
