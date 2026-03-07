using UnityEngine;

public class mushroomsParticles : MonoBehaviour
{
    [SerializeField] float timeToReload = 5f;
        private float timer = 5f;
    ParticleSystem particles;

    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if(timer < timeToReload)
        {
            timer += Time.deltaTime;
        }
        Debug.Log(timer);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (timer >= timeToReload)
        {
            particles.Play();
            timer = 0f;
        }
        
    }
}
