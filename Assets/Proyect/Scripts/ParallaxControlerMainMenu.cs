using UnityEngine;

public class ParallaxControllerMainMenu : MonoBehaviour
{
    Material[] mat;
    float[] backSpeed;

    [Range(0.00001f, 0.05f)]
    public float parallaxSpeed = 0.0001f;

    private float offset = 0f;

    void Start()
    {
        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];

        for (int i = 0; i < backCount; i++)
        {
            GameObject bg = transform.GetChild(i).gameObject;
            mat[i] = bg.GetComponent<Renderer>().material;
            // Cada capa se mueve a distinta velocidad según su índice
            backSpeed[i] = (i + 1f) / backCount;
        }
    }

    void Update()
    {
        offset += Time.deltaTime * parallaxSpeed;

        for (int i = 0; i < mat.Length; i++)
        {
            mat[i].SetTextureOffset("_MainTex", new Vector2(offset * backSpeed[i], 0));
        }
    }
}