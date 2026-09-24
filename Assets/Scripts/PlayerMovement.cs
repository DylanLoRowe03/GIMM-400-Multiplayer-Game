// PlayerMovement.cs
//
// Moves this player using joystick input from ArduinoController, and
// triggers the vibration motor once each time the joystick's button is pressed.
//
// Attach this to your player GameObject (the one player using the Arduino
// controller). Make sure the GameObject has a Rigidbody component.

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArduinoController arduinoController;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Vibration Settings")]
    [SerializeField] private int vibrationIntensity = 200; // 0-255
    [SerializeField] private int vibrationDurationMs = 150;

    private Rigidbody rb;
    private bool wasButtonPressed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Prevents the character from tipping over from movement/rotation forces.
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        if (arduinoController == null)
        {
            arduinoController = FindFirstObjectByType<ArduinoController>();
        }
    }

    private void Update()
    {
        HandleVibrationTrigger();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (arduinoController == null) return;

        float h = arduinoController.Horizontal;
        float v = arduinoController.Vertical;

        // Builds a world-space direction from joystick input (X/Z plane, since
        // this is a top-down game). If left/right or forward/back feel swapped
        // or inverted once you test it, adjust the signs/axes here.
        Vector3 moveDirection = new Vector3(h, 0f, v);

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 targetPosition = rb.position + moveDirection.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);

            // Rotate to face the direction of movement (typical top-down feel).
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void HandleVibrationTrigger()
    {
        if (arduinoController == null) return;

        bool isPressedNow = arduinoController.ButtonPressed;

        // Only fire once per press (on the rising edge), not every frame it's held.
        if (isPressedNow && !wasButtonPressed)
        {
            arduinoController.TriggerVibration(vibrationIntensity, vibrationDurationMs);
        }

        wasButtonPressed = isPressedNow;
    }
}
