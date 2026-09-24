// ArduinoController.cs
//
// Reads joystick/button data from the Arduino over serial and exposes it
// as simple public fields other scripts (e.g. a PlayerMovement script) can read.
// Also lets you trigger the vibration motor from anywhere in your game code.
//
// SETUP NOTES:
// - Requires "Api Compatibility Level" set to ".NET Framework" (not .NET Standard 2.1)
//   in Player Settings, OR install the "System.IO.Ports" NuGet package via NuGetForUnity
//   if you need .NET Standard. Editor testing usually works with either.
// - Set the correct COM port / device path in the Inspector (e.g. "COM3" on Windows,
//   "/dev/tty.usbmodemXXXX" on Mac).
// - Runs the serial read on a background thread so it doesn't block Unity's main thread
//   or get skipped during frame drops.

using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    [Header("Serial Settings")]
    [SerializeField] private string portName = "COM3";
    [SerializeField] private int baudRate = 9600;

    [Header("Joystick Calibration")]
    [SerializeField] private int analogMin = 0;
    [SerializeField] private int analogMax = 1023;
    [SerializeField] private int analogCenter = 512;
    [SerializeField] private float deadzone = 0.1f;

    // Public read-only state other scripts can poll each frame.
    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public bool ButtonPressed { get; private set; }
    public bool IsConnected { get; private set; }

    private SerialPort serialPort;
    private Thread readThread;
    private volatile bool keepReading;

    // Thread-safe latest values, copied into the public properties on the main thread.
    private volatile int latestX = 512;
    private volatile int latestY = 512;
    private volatile bool latestButton = false;

    private void Start()
    {
        try
        {
            serialPort = new SerialPort(portName, baudRate);
            serialPort.ReadTimeout = 100;
            serialPort.Open();
            IsConnected = true;

            keepReading = true;
            readThread = new Thread(ReadSerialLoop);
            readThread.IsBackground = true;
            readThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ArduinoController: could not open port {portName}: {e.Message}");
            IsConnected = false;
        }
    }

    private void ReadSerialLoop()
    {
        while (keepReading && serialPort != null && serialPort.IsOpen)
        {
            try
            {
                string line = serialPort.ReadLine(); // blocks until '\n' or timeout
                ParseLine(line);
            }
            catch (TimeoutException)
            {
                // Expected periodically; just try again.
            }
            catch (Exception e)
            {
                Debug.LogWarning($"ArduinoController read error: {e.Message}");
            }
        }
    }

    private void ParseLine(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return;

        string[] parts = line.Trim().Split(',');
        if (parts.Length != 3) return;

        if (int.TryParse(parts[0], out int x) &&
            int.TryParse(parts[1], out int y) &&
            int.TryParse(parts[2], out int button))
        {
            latestX = x;
            latestY = y;
            latestButton = button == 1;
        }
    }

    private void Update()
    {
        // Copy thread-written values into properties, applying calibration/deadzone.
        Horizontal = ApplyDeadzone(Normalize(latestX));
        Vertical = ApplyDeadzone(Normalize(latestY));
        ButtonPressed = latestButton;
    }

    private float Normalize(int raw)
    {
        if (raw >= analogCenter)
        {
            return (float)(raw - analogCenter) / (analogMax - analogCenter);
        }
        else
        {
            return (float)(raw - analogCenter) / (analogCenter - analogMin);
        }
    }

    private float ApplyDeadzone(float value)
    {
        if (Mathf.Abs(value) < deadzone) return 0f;
        return value;
    }

    /// <summary>
    /// Call this from your game code to buzz the controller.
    /// intensity: 0-255, duration in milliseconds.
    /// </summary>
    public void TriggerVibration(int intensity, int durationMs)
    {
        if (!IsConnected || serialPort == null || !serialPort.IsOpen) return;

        intensity = Mathf.Clamp(intensity, 0, 255);
        string command = $"V{intensity},{durationMs}\n";

        try
        {
            serialPort.Write(command);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ArduinoController: failed to send vibration command: {e.Message}");
        }
    }

    public void StopVibration()
    {
        if (!IsConnected || serialPort == null || !serialPort.IsOpen) return;

        try
        {
            serialPort.Write("S\n");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"ArduinoController: failed to send stop command: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    private void CloseConnection()
    {
        keepReading = false;

        if (readThread != null && readThread.IsAlive)
        {
            readThread.Join(200);
        }

        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
    }
}
