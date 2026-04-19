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
    public GameObject exitCredit;
    public GameObject buttonCredits;
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject panelCredits;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.AssignSliders(musicSlider, sfxSlider);
        }

        // Suscribirse al evento de conexión de gamepad
        InputSystem.onDeviceChange += OnDeviceChange;

        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botonPlay);
        }
    }

    void OnDestroy()
    {
        // Siempre desuscribirse para evitar memory leaks
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            if (change == InputDeviceChange.Added || change == InputDeviceChange.Reconnected)
            {
                // Pequeño delay para que el EventSystem procese el cambio correctamente
                StartCoroutine(SelectPlayButtonNextFrame());
            }
        }
    }

    private System.Collections.IEnumerator SelectPlayButtonNextFrame()
    {
        yield return null; // Espera un frame

        // Si el panel de settings está abierto, seleccionar sfx en su lugar
        if (panelSettings.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(sfx);
        }
        else
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
    public void Credits()
    {
        panelCredits.SetActive(true);
        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(exitCredit);
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
    public void closeCredits()
    {
        panelCredits.SetActive(false);
        if (Gamepad.all.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonCredits);
        }
    }
}