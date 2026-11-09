
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI[] playerHUDs;
    [SerializeField] private PlayerLightUI[] playerLightHUDs;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;

        if (index >= 0 && index < playerHUDs.Length)
        {
            PlayerHealth health =
                playerInput.GetComponent<PlayerHealth>();

            if (health != null)
            {
                playerHUDs[index].SetPlayer(health);
            }
        }

        if (index >= 0 && index < playerLightHUDs.Length)
        {
            PlayerLightResource resource =
                playerInput.GetComponent<PlayerLightResource>();

            if (resource != null && playerLightHUDs[index] != null)
            {
                playerLightHUDs[index].SetPlayer(resource);
            }
        }
    }
}
