using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image fillHealth;
    [SerializeField] private PlayerCombatController combatController;
    private float maxHealth;

    void Start()
    {
        maxHealth = combatController.GetMaxHealth();
    }

    // Update is called once per frame
    void Update()
    {
        fillHealth.fillAmount = combatController.GetCurrentHealth() / maxHealth;
    }

}
