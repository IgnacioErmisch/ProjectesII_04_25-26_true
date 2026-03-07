using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    [SerializeField] private SoundManager soundManager;

    private void Awake()
    {
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();
    }
    public void OnSelect(BaseEventData eventData)
    {
        soundManager.PlaySFX(soundManager.selectButton);
    }
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        soundManager.PlaySFX(soundManager.clickButton);
    }
}
