using UnityEngine;
public class ParallaxController : MonoBehaviour
{
    Transform cam;
    Vector3 camStartPos;
    float distance;
    GameObject[] backgrounds;
    Material[] mat;
    float[] backSpeed;
    float farthestBack;
    [Range(0.01f, 0.05f)]
    public float parallaxSpeed;

    [Range(0f, 1f)]
    public float verticalSmoothFactor = 0.1f; // 0 = sin movimiento vertical, 1 = movimiento completo

    float smoothedY;

    void Start()
    {
        cam = Camera.main.transform;
        camStartPos = cam.position;
        smoothedY = cam.position.y; // <-- inicializar Y suavizada

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
        for (int i = 0; i < backCount; i++)
        {
            if ((backgrounds[i].transform.position.z - cam.position.z) > farthestBack)
            {
                farthestBack = backgrounds[i].transform.position.z - cam.position.z;
            }
        }
        for (int i = 0; i < backCount; i++)
        {
            backSpeed[i] = 1 - (backgrounds[i].transform.position.z - cam.position.z) / farthestBack;
        }
    }
    private void LateUpdate()
    {
        float distanceX = cam.position.x - camStartPos.x;

        // Suavizar el Y para reducir vibración al saltar
        smoothedY = Mathf.Lerp(smoothedY, cam.position.y, verticalSmoothFactor);

        transform.position = new Vector3(cam.position.x - 35, smoothedY, 0);

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float speed = backSpeed[i] * parallaxSpeed;
            mat[i].SetTextureOffset("_MainTex", new Vector2(distanceX, 0) * speed);
        }
    }
}