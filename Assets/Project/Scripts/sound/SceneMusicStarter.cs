using UnityEngine;

public class SceneMusicStarter : MonoBehaviour
{
    public AudioClip music;

    private void Start()
    {
        if (SoundManager.Instance != null && music != null)
            SoundManager.Instance.PlayMusic(music);
    }
}
