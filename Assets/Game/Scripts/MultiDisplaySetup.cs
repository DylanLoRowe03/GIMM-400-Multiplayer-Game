
using UnityEngine;

public class MultiDisplaySetup : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Displays detected: " + Display.displays.Length);

        for (int i = 0; i < Display.displays.Length; i++)
        {
            Display display = Display.displays[i];

            Debug.Log(
                "Unity Display " + (i + 1) +
                " | System: " + display.systemWidth + "x" + display.systemHeight +
                " | Rendering: " + display.renderingWidth + "x" + display.renderingHeight +
                " | Active: " + display.active
            );
        }

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            Debug.Log("Second display activated!");
        }
        else
        {
            Debug.LogWarning("Only one display detected.");
        }
    }
}
