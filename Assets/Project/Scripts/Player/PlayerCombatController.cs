using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth;
    [SerializeField] private bool canRegenerate = true;
    [SerializeField] private float regenerationRate = 5f;
    [SerializeField] private float regenerationDelay = 3f;

    [Header("Knockback Settings")]
    [SerializeField] private float playerKnockbackForce = 5f;
    [SerializeField] private float playerKnockbackDuration = 0.3f;
    [SerializeField] private float invulnerabilityDuration = 1f;

    [Header("Components")]
    [SerializeField] private Animator animator;

    private HealthSystem healthSystem;
    private KnockbackSystem knockbackSystem;


    [SerializeField] private PlayerMovement playerMovement;

    public event System.Action OnPlayerDeath;
    private SoundManager soundManager;
    public event System.Action<float, float> OnHealthChanged;

    private void Awake()
    {

        healthSystem = gameObject.AddComponent<HealthSystem>();
        healthSystem.SetMaxHealth(maxHealth);
        healthSystem.OnHealthChanged += (currentHealth) =>
        {
            OnHealthChanged?.Invoke(currentHealth, healthSystem.GetMaxHealth());
        };
        healthSystem.OnDeath += HandleDeath;
        soundManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<SoundManager>();


    }

    private void Start()
    {
        OnHealthChanged?.Invoke(healthSystem.GetCurrentHealth(), healthSystem.GetMaxHealth());
    }

 
   

    public void TakeDamage(float damage, Vector2 knockbackDirection)
    {
        if ( healthSystem.IsDead()) return;

        healthSystem.TakeDamage(damage, knockbackDirection);
       

    }

    public bool IsDead()
    {
        return healthSystem.IsDead();
      

    }

    public float GetCurrentHealth()
    {
        return healthSystem.GetCurrentHealth();
    }

    public float GetMaxHealth()
    {
        return healthSystem.GetMaxHealth();
    }

    public float ResetHealth()
    {
        return healthSystem.ResetHealth();
    }

 
    private void HandleDeath()
    {
        soundManager.PlaySFX(soundManager.death);
        OnPlayerDeath?.Invoke();
    }

    public void Heal(float amount)
    {
        healthSystem.Heal(amount);
    }

 
}