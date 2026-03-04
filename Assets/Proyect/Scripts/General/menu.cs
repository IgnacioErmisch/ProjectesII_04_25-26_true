using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class menuScene : MonoBehaviour
{
    public GameObject panelSettings;
    public GameObject botonSettings;
    public GameObject botonPlay;
    public GameObject sfx;

    void Start()
    {
        Slider musicSlider = GameObject.Find("Musica")?.GetComponent<Slider>();
        Slider sfxSlider = GameObject.Find("SFX")?.GetComponent<Slider>();

        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonPlay);
        }
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

        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(sfx);
        }

    }
    public void closeSettings()
    {
        panelSettings.SetActive(false);

        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonSettings);
        }
       
    }
}