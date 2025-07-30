using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using UnityEditor;
using System.Collections.Generic;

// ====================================================================================================
// DualSenseHID - The Unity Input System Device Class
// This class registers your custom DualSense controller with Unity's Input System.
// ====================================================================================================

[InputControlLayout(stateType = typeof(DS5InputState), displayName = "Custom DualSense HID", canRunInBackground = true)]
#if UNITY_EDITOR
[InitializeOnLoad] // Make sure static constructor is called during startup.
#endif
public class CustomDualSenseHID : InputDevice
{
    public DualSenseController _controller;

    // Public properties to expose the controls
    public StickControl leftStick { get; private set; }
    public StickControl rightStick { get; private set; }
    public ButtonControl crossButton { get; private set; }
    public ButtonControl circleButton { get; private set; }
    public ButtonControl squareButton { get; private set; }
    public ButtonControl triangleButton { get; private set; }
    public DpadControl dpad { get; private set; }
    public ButtonControl leftBumper { get; private set; }
    public ButtonControl rightBumper { get; private set; }
    public AxisControl leftTrigger { get; private set; }
    public AxisControl rightTrigger { get; private set; }
    public ButtonControl leftTriggerButton { get; private set; }
    public ButtonControl rightTriggerButton { get; private set; }
    public ButtonControl shareButton { get; private set; }
    public ButtonControl optionsButton { get; private set; }
    public ButtonControl leftStickClick { get; private set; }
    public ButtonControl rightStickClick { get; private set; }
    public ButtonControl playStationButton { get; private set; }
    public ButtonControl touchpadButton { get; private set; }
    public ButtonControl micButton { get; private set; }
    public Vector3Control accelerometer { get; private set; }
    public Vector3Control gyroscope { get; private set; }
    public AxisControl touch0X { get; private set; }
    public AxisControl touch0Y { get; private set; }
    public ButtonControl touch0Press { get; private set; }
    public AxisControl touch1X { get; private set; }
    public AxisControl touch1Y { get; private set; }
    public ButtonControl touch1Press { get; private set; }
    public AxisControl batteryLevel { get; private set; }
    public ButtonControl batteryCharging { get; private set; }
    public ButtonControl batteryFullyCharged { get; private set; }
    public ButtonControl headphoneConnected { get; private set; }


    static CustomDualSenseHID()
    {
        InputSystem.onFindLayoutForDevice +=
        (ref InputDeviceDescription description, string matchedLayout, InputDeviceExecuteCommandDelegate executeDeviceCommand) =>
            {
                if (!string.IsNullOrEmpty(matchedLayout))
                {
                    if (matchedLayout == "DualSenseGamepadHID")
                    {
                        // Return the name of your custom DualSense HID layout.
                        Debug.Log($"Overriding layout for {description.product} to Custom DualSense HID.");
                        return "CustomDualSenseHID";
                    }
                }

                return null;
            };

        // Alternatively, you can also match by PID and VID, which is generally
        // more reliable for HIDs.
        InputSystem.RegisterLayout<CustomDualSenseHID>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x54C) // Sony Entertainment.
                .WithCapability("productId", 0xCE6)); // DualSense Wireless controller.
        
        InputSystem.RegisterLayout<CustomDualSenseHID>(
            matches: new InputDeviceMatcher()
                .WithInterface("HID")
                .WithCapability("vendorId", 0x54C) // Sony Entertainment.
                .WithCapability("productId", 0xDF2)); // DualSense Wireless controller.

        Debug.Log("[DualSenseHID] Layout registered.");
    }

    // Static constructor to register the device layout and override
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        List<DeviceEnumInfo> devices = DualSenseController.GetConnectedDevices(); // Corrected type for 'info'
        foreach (DeviceEnumInfo info in devices)
        {
            if (info.connection == DeviceConnection.USB)
            {
                // For USB, we can directly create and add the device
                // The Input System will then call OnAdded and FinishSetup
                InputSystem.AddDevice(new InputDeviceDescription
                {
                    interfaceName = "HID",
                    product = "DualSense Wireless Controller",
                    manufacturer = "Sony Interactive Entertainment",
                    // vendorId = 0x54C, // InputDeviceDescription does not contain a definition for 'vendorId'
                    // productId = 0xCE6 // Or 0xDF2 for newer models/revisions
                });
                Debug.Log($"[DualSenseHID] Found and added USB DualSense: {info.path}");
            }
            else if (info.connection == DeviceConnection.BT)
            {
                // For Bluetooth, the Input System might already have a generic HID device.
                // We need to find it and potentially re-layout it.
                // This part is more complex as Input System might not expose the exact device path.
                // For now, we'll rely on the layout override in the static constructor.
                Debug.LogWarning($"[DualSenseHID] Found Bluetooth DualSense: {info.path}. " +
                                 "Input System integration for BT devices might require manual pairing " +
                                 "or a generic HID device to be present first.");
                // You might need to manually add a device if it's not picked up,
                // but this can lead to duplicates if InputSystem already sees it.
                // InputSystem.AddDevice(new InputDeviceDescription
                // {
                //     interfaceName = "HID",
                //     product = "Wireless Controller", // Generic name for BT
                //     vendorId = 0x54C,
                //     productId = 0xCE6
                // });
            }
        }
    }

    // Called when the device is added to the Input System.
    protected override void OnAdded()
    {
        Debug.Log("[DualSenseHID] OnAdded called. Attempting to initialize controller.");
        base.OnAdded();
        _controller = new DualSenseController();
        if (!_controller.InitializeFirstController())
        {
            Debug.LogError("[DualSenseHID] Failed to initialize DualSenseController. Removing device.");
            InputSystem.RemoveDevice(this);
            return;
        }

        Debug.Log("[DualSenseHID] DualSenseController initialized successfully.");
        // Subscribe to controller events for connection/disconnection
        _controller.OnDisconnect += HandleControllerDisconnect;
        _controller.OnConnect += HandleControllerConnect;
        _controller.OnControllerError += HandleControllerError;
    }

    // Called when the device is removed from the Input System.
    protected override void OnRemoved()
    {
        if (_controller != null)
        {
            _controller.OnDisconnect -= HandleControllerDisconnect;
            _controller.OnConnect -= HandleControllerConnect;
            _controller.OnControllerError -= HandleControllerError;
            _controller.Dispose();
            _controller = null;
        }
        base.OnRemoved();
    }

    // FinishSetup is called after all controls have been added.
    protected override void FinishSetup()
    {
        base.FinishSetup();
        
        leftStick = GetChildControl<StickControl>("leftStick");
        rightStick = GetChildControl<StickControl>("rightStick");
        dpad = GetChildControl<DpadControl>("dpad");

        crossButton = GetChildControl<ButtonControl>("crossButton");
        circleButton = GetChildControl<ButtonControl>("circleButton");
        squareButton = GetChildControl<ButtonControl>("squareButton");
        triangleButton = GetChildControl<ButtonControl>("triangleButton");
        leftBumper = GetChildControl<ButtonControl>("leftBumper");
        rightBumper = GetChildControl<ButtonControl>("rightBumper");

        leftTrigger = GetChildControl<AxisControl>("leftTrigger");
        rightTrigger = GetChildControl<AxisControl>("rightTrigger");
        leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
        rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");

        shareButton = GetChildControl<ButtonControl>("share");
        optionsButton = GetChildControl<ButtonControl>("options");
        leftStickClick = GetChildControl<ButtonControl>("leftStickClick");
        rightStickClick = GetChildControl<ButtonControl>("rightStickClick");
        playStationButton = GetChildControl<ButtonControl>("playStationButton");
        touchpadButton = GetChildControl<ButtonControl>("touchpadButton");
        micButton = GetChildControl<ButtonControl>("micButton");

        accelerometer = GetChildControl<Vector3Control>("accelerometer");
        gyroscope = GetChildControl<Vector3Control>("gyroscope");

        touch0X = GetChildControl<AxisControl>("touch/touch0/x");
        touch0Y = GetChildControl<AxisControl>("touch/touch0/y");
        touch0Press = GetChildControl<ButtonControl>("touch/touch0/pressure");
        touch1X = GetChildControl<AxisControl>("touch/touch1/x");
        touch1Y = GetChildControl<AxisControl>("touch/touch1/y");
        touch1Press = GetChildControl<ButtonControl>("touch/touch1/pressure");

        batteryLevel = GetChildControl<AxisControl>("battery/level");
        batteryCharging = GetChildControl<ButtonControl>("battery/charging");
        batteryFullyCharged = GetChildControl<ButtonControl>("battery/fullyCharged");
        headphoneConnected = GetChildControl<ButtonControl>("headphoneConnected");
    }


    // ====================================================================================================
    // Public methods to control DualSense specific features (e.g., haptics, adaptive triggers, lightbar)
    // These methods will interact with your _controller to send output reports.
    // ====================================================================================================

    public void SetRumble(float lowFrequency, float highFrequency)
    {
        if (_controller != null && _controller.IsConnected)
        {
            // Create a new output state based on current, then modify rumble
            DS5OutputState output = _controller.CurrentOutputState;
            output.leftRumble = (byte)(lowFrequency * 255);
            output.rightRumble = (byte)(highFrequency * 255);
            _controller.SetOutputState(output);
        }
    }

    public void SetLightbarColor(Color color)
    {
        if (_controller != null && _controller.IsConnected)
        {
            DS5OutputState output = _controller.CurrentOutputState;
            output.lightbar.r = (byte)(color.r * 255);
            output.lightbar.g = (byte)(color.g * 255);
            output.lightbar.b = (byte)(color.b * 255);
            _controller.SetOutputState(output);
        }
    }

    // SetTriggerEffect now uses the correct TriggerEffect_Internal struct
    public void SetTriggerEffect(TriggerEffectType type, byte parameter1 = 0, byte parameter2 = 0, byte parameter3 = 0, byte parameter4 = 0, byte parameter5 = 0)
    {
        if (_controller != null && _controller.IsConnected)
        {
            DS5OutputState output = _controller.CurrentOutputState;
            output.leftTriggerEffect = new TriggerEffect(type); // Initialize with type

            // Assign parameters based on effect type
            if (type == TriggerEffectType.ContinuousResitance)
            {
                output.leftTriggerEffect.Continuous_StartPosition = parameter1;
                output.leftTriggerEffect.Continuous_Force = parameter2;
            }
            else if (type == TriggerEffectType.SectionResitance)
            {
                output.leftTriggerEffect.Section_StartPosition = parameter1;
                output.leftTriggerEffect.Section_EndPosition = parameter2;
            }
            else if (type == TriggerEffectType.EffectEx)
            {
                output.leftTriggerEffect.EffectEx_StartPosition = parameter1;
                output.leftTriggerEffect.EffectEx_KeepEffect = parameter2 > 0; // bool
                output.leftTriggerEffect.EffectEx_BeginForce = parameter3;
                output.leftTriggerEffect.EffectEx_MiddleForce = parameter4;
                output.leftTriggerEffect.EffectEx_EndForce = parameter5;
                // Frequency is not exposed in this method signature, but could be added
            }
            // Add logic for other effect types if needed

            // Apply to right trigger as well, or create separate methods for left/right
            output.rightTriggerEffect = output.leftTriggerEffect; // Assuming symmetric effect for now

            _controller.SetOutputState(output);
        }
    }

    public void AddEffect(DS5_Effect effect)
    {
        _controller?.AddEffect(effect);
    }

    public void ClearEffects()
    {
        _controller?.ClearEffects();
    }

    public void StopCurrentEffect()
    {
        _controller?.StopCurrentEffect();
    }

    // ====================================================================================================
    // Event Handlers for DualSenseController
    // ====================================================================================================

    private void HandleControllerDisconnect(DualSenseController controller, uint uniqueID)
    {
        Debug.Log($"DualSenseHID: Controller {uniqueID} disconnected.");
        // You might want to signal this to your game logic or UI
        // Or if the device is truly disconnected, you can tell Input System to remove it:
        InputSystem.RemoveDevice(this); // This might happen automatically but good to know
    }

    private void HandleControllerConnect(DualSenseController controller, DeviceConnection connectionType, uint uniqueID)
    {
        Debug.Log($"DualSenseHID: Controller {uniqueID} connected via {connectionType}.");
        // Set initial state or effects upon reconnection
        SetLightbarColor(Color.blue); // Example: Blue light on connect
    }

    private void HandleControllerError(DualSenseController controller, DS5W_ReturnValue errorCode)
    {
        Debug.LogError($"DualSenseHID: Controller error {errorCode} for device ID {controller.UniqueID}.");
        // Handle specific errors as needed
    }
}