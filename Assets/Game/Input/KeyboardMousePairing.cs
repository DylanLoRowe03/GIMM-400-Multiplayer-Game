
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class KeyboardMousePairing : MonoBehaviour
{
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        if (Keyboard.current == null || Mouse.current == null)
            return;

        bool hasKeyboard = false;

        foreach (InputDevice device in playerInput.devices)
        {
            if (device == Keyboard.current)
            {
                hasKeyboard = true;
                break;
            }
        }

        if (hasKeyboard)
        {
            InputUser.PerformPairingWithDevice(
                Mouse.current,
                playerInput.user
            );

            Debug.Log("Mouse paired with keyboard player!");
        }
    }
}
