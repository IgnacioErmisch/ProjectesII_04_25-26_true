using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource loopSource;
    public AudioClip background;
    public AudioClip death;
    public AudioClip dash;
    public AudioClip jumpOnEnemies;
    public AudioClip jump;
    public AudioClip spawnClon;
    public AudioClip movementSC;
    public AudioClip movementBC;
    public AudioClip movementP;
    public AudioClip despawnClone;
    public AudioClip bigCloneAttack;
    public AudioClip redAttack;
    public AudioClip blueAttack;
    public AudioClip buttonDoor;
    public AudioClip changePosition;
    public AudioClip antiCloneZone;
    public AudioClip antiGravity;
    public AudioClip deactivateAntiCloneZone;
    public AudioClip wallBreak;
    public AudioClip catapult;
    public AudioClip movingPlatform;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioClip selectButton;
    public AudioClip clickButton;
    private float currentMusicVolume;
    private float currentSFXVolume;
    public AudioMixerGroup sfxMixerGroup;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        currentMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        currentSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        ApplyMusicVolume(currentMusicVolume);
        ApplySFXVolume(currentSFXVolume);
        ConfigureSliders();
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    private void ConfigureSliders()
    {
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(currentMusicVolume);
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            ApplyMusicVolume(currentMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(currentSFXVolume);
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
            ApplySFXVolume(currentSFXVolume);
        }
    }

    public void AssignSliders(Slider music, Slider sfx)
    {
        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);

        musicSlider = music;
        sfxSlider = sfx;

        ConfigureSliders();
    }

    private void ApplyMusicVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20;
        audioMixer.SetFloat("MusicVolume", volume);
    }

    private void ApplySFXVolume(float value)
    {
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20;
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SetMusicVolume(float value)
    {
        currentMusicVolume = value;
        ApplyMusicVolume(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float value)
    {
        currentSFXVolume = value;
        ApplySFXVolume(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoop(AudioClip clip)
    {
        if (loopSource.clip == clip && loopSource.isPlaying) return;
        loopSource.clip = clip;
        loopSource.loop = true;
        loopSource.Play();
    }

    public void StopLoop()
    {
        loopSource.Stop();
        loopSource.clip = null;
    }
}