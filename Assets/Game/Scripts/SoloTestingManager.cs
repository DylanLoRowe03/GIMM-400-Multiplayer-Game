
using UnityEngine;
using UnityEngine.InputSystem;

public class SoloTestingManager : MonoBehaviour
{
    [Header("Testing Settings")]
    [SerializeField] private bool enableSoloTesting = true;

    [Header("Multiplayer Manager")]
    [SerializeField] private PlayerInputManager inputManager;

    private PlayerInput[] players = new PlayerInput[4];

    private int activePlayerIndex = -1;

    private void Start()
    {
        if (!enableSoloTesting)
            return;

        if (inputManager == null)
        {
            Debug.LogError("Player Input Manager is missing!");
            return;
        }

        if (Keyboard.current == null || Mouse.current == null)
        {
            Debug.LogError("Keyboard or mouse not detected!");
            return;
        }

        // Spawn all four players.
        for (int i = 0; i < players.Length; i++)
        {
            PlayerInput player = inputManager.JoinPlayer(
                playerIndex: i,
                pairWithDevice: Keyboard.current
            );

            if (player == null)
            {
                Debug.LogError("Failed to spawn Player " + (i + 1));
                continue;
            }

            players[i] = player;

            PlayerCharacter character =
                player.GetComponent<PlayerCharacter>();

            if (character != null)
            {
                character.SetCharacter(
                    (PlayerCharacter.CharacterType)i
                );
            }

            player.DeactivateInput();
        }

        // Prevent additional players from joining.
        inputManager.DisableJoining();

        // Start by controlling Headlamp.
        SwitchPlayer(0);
    }

    private void Update()
    {
        if (!enableSoloTesting || Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SwitchPlayer(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SwitchPlayer(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SwitchPlayer(2);

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            SwitchPlayer(3);
    }

    private void SwitchPlayer(int index)
    {
        if (index < 0 || index >= players.Length)
            return;

        if (players[index] == null)
            return;

        if (activePlayerIndex == index)
            return;

        // Release control of the previous player.
        if (activePlayerIndex >= 0)
        {
            PlayerInput previousPlayer =
                players[activePlayerIndex];

            if (previousPlayer != null)
            {
                PlayerCharacter previousCharacter =
                    previousPlayer.GetComponent<PlayerCharacter>();

                if (previousCharacter != null)
                {
                    previousCharacter.ReleaseHeldAbilities();
                }

                previousPlayer.DeactivateInput();
            }
        }

        // Give control to the selected player.
        PlayerInput newPlayer = players[index];

        newPlayer.ActivateInput();

        activePlayerIndex = index;

        PlayerCharacter character =
            newPlayer.GetComponent<PlayerCharacter>();

        if (character != null)
        {
            Debug.Log(
                "Now controlling Player " +
                (index + 1) +
                ": " +
                character.SelectedCharacter
            );
        }
    }
}
