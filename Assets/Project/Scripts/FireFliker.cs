using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FireFlicker : MonoBehaviour
{
    public float minIntensity = 0.8f;
    public float maxIntensity = 1.5f;

    public float flickerSpeed = 8f;

    public float smoothSpeed = 10f;

    private Light2D light2D;
    private float targetIntensity;
    private float noiseOffset;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        noiseOffset = Random.Range(0f, 100f);
        targetIntensity = light2D.intensity;
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(noiseOffset + Time.time * flickerSpeed, 0f);

        targetIntensity = Mathf.Lerp(minIntensity, maxIntensity, noise);

        light2D.intensity = Mathf.Lerp(light2D.intensity, targetIntensity, Time.deltaTime * smoothSpeed);
    }
}
