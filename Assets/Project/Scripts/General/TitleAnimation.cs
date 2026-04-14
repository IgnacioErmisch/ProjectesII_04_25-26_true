using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    [Header("Bobbing")]
    public float bobbingAmount = 10f;   // píxeles que sube/baja
    public float bobbingSpeed = 1.5f;

    [Header("Pulse")]
    public float pulseAmount = 0.04f;   // cuánto escala crece/decrece
    public float pulseSpeed = 1.2f;

    private RectTransform rt;
    private Vector2 startPos;
    private Vector3 startScale;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
        startScale = rt.localScale;
    }

    void Update()
    {
        // Bobbing vertical
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;
        rt.anchoredPosition = new Vector2(startPos.x, newY);

        // Pulse de escala
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        rt.localScale = startScale * pulse;
    }
}