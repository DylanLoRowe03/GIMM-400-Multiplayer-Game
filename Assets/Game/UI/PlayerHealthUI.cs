
using UnityEngine;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] hearts;

    private PlayerHealth playerHealth;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 0f;
    }

    public void SetPlayer(PlayerHealth health)
    {
        playerHealth = health;
        canvasGroup.alpha = 1f;
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
        if (playerHealth == null) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].gameObject.SetActive(
                i < playerHealth.CurrentHealth
            );
        }
    }
}
