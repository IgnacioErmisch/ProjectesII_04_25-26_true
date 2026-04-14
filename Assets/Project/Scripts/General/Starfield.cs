using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Attach a un GameObject vacío hijo del Canvas.
/// Genera estrellas parpadeantes en la zona superior de la pantalla.
/// No necesita ningún sprite externo — dibuja círculos blancos con Image.
/// </summary>
public class StarField : MonoBehaviour
{
    [Header("Cantidad")]
    public int starCount = 60;

    [Header("Zona de aparición (en % de pantalla, 0-1)")]
    public float minY = 0.55f;   // las estrellas sólo aparecen en el 45% superior
    public float maxY = 1.0f;

    [Header("Tamaño")]
    public float minSize = 2f;
    public float maxSize = 6f;

    [Header("Parpadeo")]
    public float minBlinkSpeed = 0.5f;
    public float maxBlinkSpeed = 2.5f;

    private class Star
    {
        public Image img;
        public float blinkSpeed;
        public float blinkOffset;
    }

    private List<Star> stars = new List<Star>();
    private RectTransform canvasRect;

    void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float W = canvasRect.rect.width;
        float H = canvasRect.rect.height;

        for (int i = 0; i < starCount; i++)
        {
            // Crear GameObject
            GameObject go = new GameObject("Star_" + i, typeof(Image));
            go.transform.SetParent(transform, false);

            Image img = go.GetComponent<Image>();
            img.color = Color.white;

            // Posición aleatoria en la zona superior
            RectTransform rt = go.GetComponent<RectTransform>();
            float x = Random.Range(-W / 2f, W / 2f);
            float y = Random.Range(H * (minY - 0.5f), H * (maxY - 0.5f));
            rt.anchoredPosition = new Vector2(x, y);

            // Tamaño aleatorio
            float size = Random.Range(minSize, maxSize);
            rt.sizeDelta = new Vector2(size, size);

            // Datos de parpadeo
            Star star = new Star
            {
                img = img,
                blinkSpeed = Random.Range(minBlinkSpeed, maxBlinkSpeed),
                blinkOffset = Random.Range(0f, Mathf.PI * 2f)
            };
            stars.Add(star);
        }
    }

    void Update()
    {
        foreach (var star in stars)
        {
            float alpha = (Mathf.Sin(Time.time * star.blinkSpeed + star.blinkOffset) + 1f) / 2f;
            // Rango de alpha entre 0.2 y 1 para que nunca desaparezcan del todo
            alpha = Mathf.Lerp(0.15f, 1f, alpha);
            star.img.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}