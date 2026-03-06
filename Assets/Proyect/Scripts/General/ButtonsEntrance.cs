using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonsEntrance : MonoBehaviour
{
    [Header("Botones en orden de aparición")]
    public RectTransform[] buttons;

    [Header("Animación")]
    public float slideDistance = 80f;     
    public float duration = 0.45f;        
    public float delayBetween = 0.12f;    
    public float initialDelay = 0.3f;     

    void Start()
    {
        StartCoroutine(AnimateButtons());
    }

    IEnumerator AnimateButtons()
    {
     
        Vector2[] originalPos = new Vector2[buttons.Length];
        CanvasGroup[] groups = new CanvasGroup[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            originalPos[i] = buttons[i].anchoredPosition;
            buttons[i].anchoredPosition = originalPos[i] - new Vector2(0, slideDistance);
            groups[i] = buttons[i].GetComponent<CanvasGroup>();
            if (groups[i] == null)
                groups[i] = buttons[i].gameObject.AddComponent<CanvasGroup>();
            groups[i].alpha = 0f;
        }

        yield return new WaitForSeconds(initialDelay);

        for (int i = 0; i < buttons.Length; i++)
        {
            StartCoroutine(AnimateSingle(buttons[i], groups[i], originalPos[i]));
            yield return new WaitForSeconds(delayBetween);
        }
    }

    IEnumerator AnimateSingle(RectTransform rt, CanvasGroup cg, Vector2 targetPos)
    {
        float elapsed = 0f;
        Vector2 startPos = rt.anchoredPosition;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;         
            float eased = EaseOutBack(t);

            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, eased);
            cg.alpha = Mathf.Clamp01(t * 2f); 

            yield return null;
        }

        rt.anchoredPosition = targetPos;
        cg.alpha = 1f;
    }

    float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}