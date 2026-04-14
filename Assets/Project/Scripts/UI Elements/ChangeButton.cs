using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    public Sprite[] sprites;
    private Image myImage;

    private void Start()
    {
        myImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Cambiar(1);
    }
    public void OnPointerExit(PointerEventData eventData)
    { 
        Cambiar(0);
    }
    public void OnSelect(BaseEventData eventData)
    {
        Cambiar(1);
    }
     
    public void OnDeselect(BaseEventData eventData)
    {
        Cambiar(0);
    }

    private void Cambiar(int indice)
    {
        if (sprites.Length > indice && myImage != null)
        {
            myImage.sprite = sprites[indice];
        }
    }
}