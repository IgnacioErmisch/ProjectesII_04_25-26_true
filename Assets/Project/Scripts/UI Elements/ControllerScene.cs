using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class FadeInText : MonoBehaviour
{
    public TextMeshProUGUI miTexto;
    public Image mando;
    public float duracion;

    void Awake()
    {
        Color color = miTexto.color;
        color.a = 0f;
        miTexto.color = color;
        mando.color = color;
    }

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        Color color = miTexto.color;
        Color colorMando = mando.color;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            color.a = tiempo / duracion;
            miTexto.color = color;
            mando.color = color;
            yield return null;
        }

        color.a = 1f;
        miTexto.color = color;
        mando.color = color;
        SceneManager.LoadScene("menuPrincipal");

    }
}