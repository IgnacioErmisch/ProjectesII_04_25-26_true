using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelInitializer : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        InitializeSliders();
    }

    private void OnEnable()
    {
        // Por si el panel se activa después del Start
        InitializeSliders();
    }

    private void InitializeSliders()
    {
        if (SoundManager.Instance != null && musicSlider != null && sfxSlider != null)
        {
            SoundManager.Instance.AssignSliders(musicSlider, sfxSlider);
        }
    }
}
