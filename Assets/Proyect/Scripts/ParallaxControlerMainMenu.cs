using UnityEngine;

public class ParallaxControllerMainMenu : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxLayer
    {
        public GameObject plano;      
        [Range(0f, 0.5f)]
        public float speed = 0.1f;        
        [HideInInspector] public Material mat;
        [HideInInspector] public float distance;
    }

    [Header("Capas de Parallax")]
    public ParallaxLayer[] layers;

    [Header("Configuración Global")]
    [Tooltip("Multiplicador global que afecta a todas las capas")]
    public float globalSpeedMultiplier = 1f;

    [Tooltip("Activa/desactiva el movimiento del parallax")]
    public bool isPlaying = true;

    [Header("Velocidades por Defecto (si no asignas manualmente)")]
    [Tooltip("Si está activado, asigna velocidades automáticas escalonadas")]
    public bool autoAssignSpeeds = false;
    public float minSpeed = 0.02f;
    public float maxSpeed = 0.15f;

    void Start()
    {
        InitializeLayers();
    }

    void InitializeLayers()
    {
        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].plano == null) continue;

            Renderer rend = layers[i].plano.GetComponent<Renderer>();
            if (rend != null)
            {
               
                layers[i].mat = rend.material;
            }
                  
            if (autoAssignSpeeds && layers.Length > 1)
            {
                layers[i].speed = Mathf.Lerp(minSpeed, maxSpeed, (float)i / (layers.Length - 1));
            }
        }
    }

    void Update()
    {
        if (!isPlaying) return;

        foreach (var layer in layers)
        {
            if (layer.mat == null) continue;

            layer.distance += Time.deltaTime * layer.speed * globalSpeedMultiplier;
            layer.mat.SetTextureOffset("_MainTex", Vector2.right * layer.distance);
        }
    }

    public void SetPlaying(bool playing)
    {
        isPlaying = playing;
    }

    public void ResetLayers()
    {
        foreach (var layer in layers)
        {
            layer.distance = 0f;
            if (layer.mat != null)
                layer.mat.SetTextureOffset("_MainTex", Vector2.zero);
        }
    }

    public void SetGlobalSpeed(float multiplier)
    {
        globalSpeedMultiplier = multiplier;
    }
}