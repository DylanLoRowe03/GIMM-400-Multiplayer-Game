
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    [Header("Revive Settings")]
    [SerializeField] private int reviveHealth = 1;

    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDowned => currentHealth <= 0;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (IsDowned)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (IsDowned)
        {
            DownPlayer();
        }
    }

    public void Heal(int amount)
    {
        if (IsDowned)
            return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} healed {amount}. Health: {currentHealth}/{maxHealth}");
    }

    public void Revive()
    {
        if (!IsDowned)
            return;

        currentHealth = Mathf.Clamp(reviveHealth, 1, maxHealth);

        Debug.Log($"{gameObject.name} has been REVIVED! Health: {currentHealth}/{maxHealth}");
    }

    private void DownPlayer()
    {
        Debug.Log($"{gameObject.name} is DOWNED!");
    }
}
