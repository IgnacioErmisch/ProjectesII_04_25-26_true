using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;
    float farthestBack;
    [Range(0.01f, 0.05f)]
    public float parallaxSpeed = 0.02f;
    [Range(0f, 1f)]
    public float verticalSmoothFactor = 0f;
    float smoothedY;
    float smoothedX;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        smoothedY = cam.position.y;
        smoothedX = cam.position.x;

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

    private void LateUpdate()
    {

        smoothedX = Mathf.Lerp(smoothedX, cam.position.x, 0.15f);
        smoothedY = (verticalSmoothFactor > 0f)
            ? Mathf.Lerp(smoothedY, cam.position.y, verticalSmoothFactor)
            : camStartPos.y;


        transform.position = new Vector3(smoothedX - 35f, smoothedY, 0f);

        float distanceX = smoothedX - camStartPos.x;
        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX * speed, 0f));
        }
    }
}