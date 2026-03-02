using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class menuScene : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject primerBotonMenuPrincipal;
    public GameObject sfx;

    void Start()
    {
        Slider musicSlider = GameObject.Find("Musica")?.GetComponent<Slider>();
        Slider sfxSlider = GameObject.Find("SFX")?.GetComponent<Slider>();
    }

    public void LoadSelector()
    {
        SceneManager.LoadScene("LevelSelector");
        Time.timeScale = 1;
    }
    public void ExitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene("menuPrincipal");
    }
    public void Settings()
    {
        panelSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(sfx);

    }
    public void closeSettings()
    {
        panelSettings.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(primerBotonMenuPrincipal);
    }
}