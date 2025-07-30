using UnityEngine;
using UnityEngine.InputSystem; // Needed for InputSystem.GetDevice<T>()

public class DualSenseInputReader : MonoBehaviour
{
    private CustomDualSenseHID dualSense;

    void OnEnable()
    {
        // Subscribe to device change events
        InputSystem.onDeviceChange += OnDeviceChange;

        // Try to find the device immediately in case it's already connected
        FindDualSenseDevice();
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
        // Clean up effects when script is disabled
        if (dualSense != null)
        {
            dualSense.ClearEffects();
        }
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change == InputDeviceChange.Added)
        {
            // If the added device is a DualSenseHID, assign it
            if (device is CustomDualSenseHID)
            {
                dualSense = device as CustomDualSenseHID;
                Debug.Log("DualSenseHID device found and assigned via device change event.");
            }
        }
        else if (change == InputDeviceChange.Removed)
        {
            // If the removed device is our current one, null it out
            if (device == dualSense)
            {
                dualSense = null;
                Debug.Log("DualSenseHID device was removed.");
            }
        }
    }

    private void FindDualSenseDevice()
    {
        dualSense = InputSystem.GetDevice<CustomDualSenseHID>();
        if (dualSense == null)
        {
            Debug.LogWarning("DualSenseHID device not found initially. Waiting for it to be added.");
        }
    }

    void Update()
    {
        if (dualSense == null)
        {
            if (Time.time % 2 == 0)
            {
                FindDualSenseDevice();
            }
            return;
        }
        else
        {
            // Access buttons
            if (dualSense.crossButton.wasPressedThisFrame)
            {
                Debug.Log("Cross button pressed!");
                // Example: Trigger a short rumble
                dualSense.SetRumble(0.5f, 0.5f);
            }

            // Access stick values
            Vector2 leftStickValue = dualSense.leftStick.ReadValue();
            // Debug.Log($"Left Stick: {leftStickValue}");

            // Access triggers
            float leftTriggerValue = dualSense.leftTrigger.ReadValue();
            // Debug.Log($"Left Trigger: {leftTriggerValue}");

            // Access motion data
            Vector3 accel = dualSense.accelerometer.ReadValue();
            Vector3 gyro = dualSense.gyroscope.ReadValue();
            // Debug.Log($"Accel: {accel}, Gyro: {gyro}");

            // Example: Change lightbar color based on left trigger
            dualSense.SetLightbarColor(new Color(leftTriggerValue, 0, 1 - leftTriggerValue));
        }
    }
}