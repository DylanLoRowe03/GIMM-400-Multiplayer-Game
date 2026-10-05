// DangerZone.cs
//
// Attach to any GameObject with a BoxCollider to create a zone that buzzes
// the Arduino controller when the Arduino-controlled player enters it.
// Every zone has its own adjustable intensity/duration in the Inspector,
// so you can drop multiple DangerZones in a scene and tune each one separately.
//
// SETUP:
// 1. Tag your Arduino-controlled player's GameObject (e.g. "ArduinoPlayer")
//    and make sure that tag matches the "Player Tag" field below.
// 2. Add a BoxCollider to this GameObject and size/position it to cover the
//    danger area. This script forces it to be a trigger automatically.

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider))]
public class DangerZone : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private string playerTag = "Player";

    [Header("Vibration Settings")]
    [SerializeField, Range(0, 255)] private int intensity = 150;
    [SerializeField, Range(0, 2000)] private int durationMs = 200;

    [Header("Repeat While Inside (optional)")]
    [SerializeField] private bool pulseWhileInside = false;
    [SerializeField, Range(0.1f, 5f)] private float pulseInterval = 1f;

    [Header("Editor")]
    [SerializeField] private Color gizmoColor = new Color(1f, 0.3f, 0.2f, 0.25f);

    private ArduinoController arduinoController;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true; // a danger zone only ever works as a trigger

        arduinoController = FindFirstObjectByType<ArduinoController>();

        if (arduinoController == null)
        {
            Debug.LogWarning($"DangerZone on '{gameObject.name}': no ArduinoController found in the scene.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (arduinoController == null) return;

        arduinoController.TriggerVibration(intensity, durationMs);

        if (pulseWhileInside && pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulseLoop());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
            arduinoController?.StopVibration();
        }
    }

    private IEnumerator PulseLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(pulseInterval);
            arduinoController.TriggerVibration(intensity, durationMs);
        }
    }

    // Draws the zone as a translucent box in the Scene view so designers can
    // see and adjust danger zones visually, even without selecting them.
    private void OnDrawGizmos()
    {
        BoxCollider col = GetComponent<BoxCollider>();
        if (col == null) return;

        Gizmos.color = gizmoColor;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(col.center, col.size);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(col.center, col.size);
    }
}
