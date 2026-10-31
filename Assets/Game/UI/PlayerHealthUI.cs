
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] hearts;

    private PlayerHealth playerHealth;

    public void SetPlayer(PlayerHealth health)
    {
        playerHealth = health;
        UpdateHearts();
    }

    private void Update()
    {
        if (playerHealth != null)
        {
            UpdateHearts();
        }
    }

    private void UpdateHearts()
    {
        if (playerHealth == null)
            return;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(
                i < playerHealth.CurrentHealth
            );
        }
    }
}
