using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;
public class CristalLights : MonoBehaviour
{
    private Light2D light2D;

    public float normalIntensity = 3.5f;
    public float maxIntensity = 10f;
    public float lerpDuration = 0.5f;
    public float holdDuration = 2f;

    private enum LightState { Idle, FadingIn, Holding, FadingOut }
    private LightState state = LightState.Idle;

    private float elapsed = 0f;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        light2D.intensity = normalIntensity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        elapsed = 0f;
        state = LightState.FadingIn;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        switch (state)
        {
            case LightState.FadingIn:
                float t1 = Mathf.Clamp01(elapsed / lerpDuration);
                light2D.intensity = Mathf.Lerp(normalIntensity, maxIntensity, t1);

                if (elapsed >= lerpDuration)
                {
                    light2D.intensity = maxIntensity;
                    elapsed = 0f;
                    state = LightState.Holding;
                }
                break;

            case LightState.Holding:
                if (elapsed >= holdDuration)
                {
                    elapsed = 0f;
                    state = LightState.FadingOut;
                }
                break;

            case LightState.FadingOut:
                float t2 = Mathf.Clamp01(elapsed / lerpDuration);
                light2D.intensity = Mathf.Lerp(maxIntensity, normalIntensity, t2);

                if (elapsed >= lerpDuration)
                {
                    light2D.intensity = normalIntensity;
                    elapsed = 0f;
                    state = LightState.Idle;
                }
                break;
        }
    }
    
}
