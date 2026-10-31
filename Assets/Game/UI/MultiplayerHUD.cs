
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerHealthUI[] playerHUDs;

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
    }
}
