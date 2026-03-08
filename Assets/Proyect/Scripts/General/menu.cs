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
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.AssignSliders(musicSlider, sfxSlider);
        }

        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonPlay);
        }
    }

    private void Update()
    {
        if (Gamepad.all.Count > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
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