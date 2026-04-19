using UnityEngine;

public class BiomeTriggerZone : MonoBehaviour
{
    public Color zoneColor = Color.white;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var particles = other.GetComponentsInChildren<ParticleSystem>();
        foreach (var ps in particles)
        {
            if (ps.gameObject.name.Contains("Land") || ps.gameObject.name.Contains("Run"))
            {
                var main = ps.main;
                main.startColor = zoneColor;
            }
        }
    }

    
}