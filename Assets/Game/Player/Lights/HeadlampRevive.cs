
using UnityEngine;

public class HeadlampRevive : MonoBehaviour
{
    [Header("Revive Settings")]
    [SerializeField] private float reviveRange = 2f;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public bool TryRevive()
    {
        if (playerHealth == null || playerHealth.IsDowned)
            return false;

        PlayerHealth[] players = FindObjectsByType<PlayerHealth>(
            FindObjectsSortMode.None
        );

        PlayerHealth closestPlayer = null;
        float closestDistance = reviveRange;

        foreach (PlayerHealth player in players)
        {
            if (player == playerHealth || !player.IsDowned)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                player.transform.position
            );

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            closestPlayer.Revive();
            Debug.Log("Successfully revived teammate!");
            return true;
        }

        return false;
    }
}
