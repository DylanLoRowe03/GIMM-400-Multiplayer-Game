using UnityEngine;
using UnityEngine.InputSystem;

public class MultiplayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int playerIndex = playerInput.playerIndex;

        if (playerIndex < spawnPoints.Length)
        {
            CharacterController controller =
                playerInput.GetComponent<CharacterController>();

            // Temporarily disable the CharacterController
            // while changing the player's position.
            if (controller != null)
            {
                controller.enabled = false;
            }

            playerInput.transform.position =
                spawnPoints[playerIndex].position;

            playerInput.transform.rotation =
                spawnPoints[playerIndex].rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }
}