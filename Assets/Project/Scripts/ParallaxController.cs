using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;
    float farthestBack;
    float startY;
    float smoothedX;

    [Range(0.01f, 0.05f)]
    public float parallaxSpeed = 0.02f;

    [Range(1f, 30f)]
    public float smoothSpeed = 10f;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        smoothedX = cam.position.x;
        startY = transform.position.y;

        int backCount = transform.childCount;
        mat = new Material[backCount];
        backSpeed = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            mat[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        BackSpeedCalculate(backCount);
    }

    void BackSpeedCalculate(int backCount)
    {
        farthestBack = 0f;

        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            if (depth > farthestBack)
                farthestBack = depth;
        }

        for (int i = 0; i < backCount; i++)
        {
            float depth = backgrounds[i].transform.position.z - cam.position.z;
            backSpeed[i] = (farthestBack != 0f) ? 1f - (depth / farthestBack) : 0f;
        }
    }

    void LateUpdate()
    {
        float lerpFactor = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        smoothedX = Mathf.Lerp(smoothedX, cam.position.x, lerpFactor);

        transform.position = new Vector3(smoothedX - 35f, startY, 0f);

        float distanceX = smoothedX - camStartPos.x;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX * speed, 0f));
        }
    }
}