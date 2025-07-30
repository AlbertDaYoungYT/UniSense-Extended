using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UniSense.LowLevel
{
    // Define the custom control layout for the touchpad
    [InputControlLayout(commonUsages = new[] { "Touchscreen" })]
    public unsafe class DualSenseTouchpadControl : InputControl<Vector2>
    {
        // This class acts as a container. Its purpose is to define the layout
        // for the 'touchpad' control in DualSenseHIDInputReport.
        // The actual data mapping for the touch points (x, y, down, id)
        // is still handled by FieldOffset attributes in DualSenseHIDInputReport.

        public TouchControl primaryTouch { get; protected set; }
        public TouchControl secondaryTouch { get; protected set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            primaryTouch = GetChildControl<TouchControl>("primaryTouch");
            secondaryTouch = GetChildControl<TouchControl>("secondaryTouch");
        }

        public override Vector2 ReadUnprocessedValueFromState(void* statePtr)
        {
            if (primaryTouch != null)
                return primaryTouch.position.ReadValueFromState(statePtr);
            return default;
        }

        public override void WriteValueIntoState(Vector2 value, void* statePtr)
        {
            // This control is read-only, so we don't need to implement writing.
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 64)]
    internal struct DualSenseHIDInputReport : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'I', 'D');

        [FieldOffset(0)] public byte reportId;

        [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE",
            parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(1)] public byte leftStickX;

        [FieldOffset(2)] public byte leftStickY;

        [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "rightStick/x", offset = 0, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/left", offset = 0, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/right", offset = 0, format = "BYTE",
            parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "rightStick/y", offset = 1, format = "BYTE",
            parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/up", offset = 1, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/down", offset = 1, format = "BYTE",
            parameters =
                "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(3)] public byte rightStickX;
        [FieldOffset(4)] public byte rightStickY;

        [InputControl(name = "leftTrigger", format = "BYTE")]
        [FieldOffset(5)] public byte leftTrigger;

        [InputControl(name = "rightTrigger", format = "BYTE")]
        [FieldOffset(6)] public byte rightTrigger;

        // Byte at offset 7 is unmapped in the original HID report for this section.

        [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
        [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton",
            parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton",
            parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton",
            parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton",
            parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "buttonWest", displayName = "Square", bit = 4)]
        [InputControl(name = "buttonSouth", displayName = "Cross", bit = 5)]
        [InputControl(name = "buttonEast", displayName = "Circle", bit = 6)]
        [InputControl(name = "buttonNorth", displayName = "Triangle", bit = 7)]
        [FieldOffset(8)] public byte buttons1;

        [InputControl(name = "leftShoulder", bit = 0)]
        [InputControl(name = "rightShoulder", bit = 1)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = 2)]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = 3)]
        [InputControl(name = "select", displayName = "Share", bit = 4)]
        [InputControl(name = "start", displayName = "Options", bit = 5)]
        [InputControl(name = "leftStickPress", bit = 6)]
        [InputControl(name = "rightStickPress", bit = 7)]
        [FieldOffset(9)] public byte buttons2;

        [InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0)]
        [InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1)]
        [InputControl(name = "micMuteButton", layout = "Button", displayName = "Mic Mute", bit = 2)]
        [FieldOffset(10)] public byte buttons3;

        // Bytes at offsets 11-15 are unmapped in the original HID report before gyro/accel.

        [InputControl(name = "gyro", format = "VC3S", layout = "Vector3")]
        [InputControl(name = "gyro/x", layout = "Axis", format = "SHRT")]
        [InputControl(name = "gyro/y", offset = 2, layout = "Axis", format = "SHRT")]
        [InputControl(name = "gyro/z", offset = 4, layout = "Axis", format = "SHRT")]
        [FieldOffset(16)] public short gyroPitch;
        [FieldOffset(18)] public short gyroYaw;
        [FieldOffset(20)] public short gyroRoll;

        [InputControl(name = "accel", format = "VC3S", layout = "Vector3")]
        [InputControl(name = "accel/x", layout = "Axis", format = "SHRT")]
        [InputControl(name = "accel/y", offset = 2, layout = "Axis", format = "SHRT")]
        [InputControl(name = "accel/z", offset = 4, layout = "Axis", format = "SHRT")]
        [FieldOffset(22)] public short accelX;
        [FieldOffset(24)] public short accelY;
        [FieldOffset(26)] public short accelZ;

        // --- Touchpad Input Fields ---
        // These fields are placed in the previously unmapped region of the HID report,
        // specifically bytes 28-47, which aligns with the common DualSense HID report structure.
        // Removed 'layout = "Touchpad"' as it's not a pre-defined layout in Input System
        [InputControl(name = "touchpad", layout = "DualSenseTouchpadControl")] // Overall touchpad control group
        // Removed 'layout = "Touch"' as it's not a pre-defined layout for nested controls
        [InputControl(name = "touchpad/primaryTouch")]
        [InputControl(name = "touchpad/primaryTouch/x", format = "UINT")]
        [FieldOffset(28)] public uint touchPoint1X;

        [InputControl(name = "touchpad/primaryTouch/y", format = "UINT")]
        [FieldOffset(32)] public uint touchPoint1Y;

        [InputControl(name = "touchpad/primaryTouch/down", layout = "Button")]
        [FieldOffset(36)] public byte touchPoint1Down; // 0 for up, 1 for down

        [InputControl(name = "touchpad/primaryTouch/id", format = "BYTE")]
        [FieldOffset(37)] public byte touchPoint1Id;

        // Removed 'layout = "Touch"' as it's not a pre-defined layout for nested controls
        [InputControl(name = "touchpad/secondaryTouch")] 
        [InputControl(name = "touchpad/secondaryTouch/x", format = "UINT")]
        [FieldOffset(38)] public uint touchPoint2X;

        [InputControl(name = "touchpad/secondaryTouch/y", format = "UINT")]
        [FieldOffset(42)] public uint touchPoint2Y;

        [InputControl(name = "touchpad/secondaryTouch/down", layout = "Button")]
        [FieldOffset(46)] public byte touchPoint2Down; // 0 for up, 1 for down

        [InputControl(name = "touchpad/secondaryTouch/id", format = "BYTE")]
        [FieldOffset(47)] public byte touchPoint2Id;
        // --- End Touchpad Input Fields ---

        // Bytes at offsets 48-53 are still unmapped in this struct,
        // which aligns with the HID report having other data in this region
        // that might not be explicitly mapped by UniSense.

        [InputControl(name = "batteryCharging", layout = "Button", displayName = "Battery is Charging", bit = 3)]
        [FieldOffset(54)] public byte batteryInfo1;

        [InputControl(name = "batteryFullyCharged", layout = "Button", displayName = "Battery is Fully Charged",
            bit = 5)]
        [InputControl(name = "batteryLevel", layout = "Axis", format = "BIT", displayName = "Battery Level", bit = 0,
            sizeInBits = 4, parameters = "normalize,normalizeMin=0,normalizeMax=1")]
        [FieldOffset(55)] public byte batteryInfo2;

        // Remaining bytes (56-63) are unmapped but part of the 64-byte report size.
    }
}
