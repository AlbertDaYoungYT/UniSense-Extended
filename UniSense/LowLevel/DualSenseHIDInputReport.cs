using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UniSense.LowLevel
{
    [StructLayout(LayoutKind.Explicit, Size = 64)] // Size 64
    internal unsafe struct DualSenseHIDInputReport : IInputStateTypeInfo
    {
        public FourCC format => new FourCC('H', 'I', 'D');

        [InputControl(name = "reportId", format = "BYTE", layout = "Integer", sizeInBits = 8)]
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
        [InputControl(name = "seqNo", format = "BYTE", layout = "Integer", sizeInBits = 8)]
        [FieldOffset(7)] public byte seqNo;

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
        [InputControl(name = "unkCounter", format = "UINT", layout = "Integer", sizeInBits = 32)]
        [FieldOffset(12)] public uint unkCounter;

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

        [InputControl(name = "sensorTimestamp", format = "INT", layout = "Integer", sizeInBits = 32)]
        [FieldOffset(28)] public int sensorTimestamp;

        [InputControl(name = "temperature", format = "BYTE", layout = "Integer", sizeInBits = 8)]
        [FieldOffset(32)] public byte temperature;

        // --- Touchpad Input Fields ---
        // These fields are placed in the previously unmapped region of the HID report,
        // specifically bytes 28-47, which aligns with the common DualSense HID report structure.

        //[InputControl(name = "touch0", layout = "DualSenseTouchPoint")]
        //[FieldOffset(33)] public DualSenseTouchPointState touch0;

        //[InputControl(name = "touch1", layout = "DualSenseTouchPoint")]
        //[FieldOffset(37)] public DualSenseTouchPointState touch1;

        [FieldOffset(33)]
        //[InputControl(name = "touch0Detection", layout = "DS5_IsTouchingControl", format = "BIT", offset = 33, bit = 7, sizeInBits = 1)]
        //[InputControl(name = "touch0Index", layout = "DS5_TouchIndexControl", format = "BYTE", offset = 33, bit = 0, sizeInBits = 7)]
        //[InputControl(name = "touch0X", layout = "DS5_TouchXAxisControl", format = "SHRT", offset = 34, sizeInBits = 12, bit = 0)]
        //[InputControl(name = "touch0Y", layout = "DS5_TouchYAxisControl", format = "SHRT", offset = 35, sizeInBits = 12, bit = 4)]
        [InputControl(name = "touchpad", layout = "DS5_TouchpadControl")]
        public byte touchData0;

        //[FieldOffset(37)]
        //[InputControl(name = "touch1", layout = "DS5_TouchpadControl")]
        //public byte touchData1;

        // --- End Touchpad Input Fields ---

        // Bytes at offsets 48-53 are still unmapped in this struct,
        // which aligns with the HID report having other data in this region
        // that might not be explicitly mapped by UniSense.

        [InputControl(name = "batteryPercent", layout = "Integer", displayName = "Battery Percent", bit = 0, sizeInBits = 4)]
        [InputControl(name = "batteryState", format = "BYTE", layout = "Integer", displayName = "Battery State", bit = 4, sizeInBits = 4)]
        [FieldOffset(52)] public byte batteryInfo1;

        // Remaining bytes (56-63) are unmapped but part of the 64-byte report size.
        [FieldOffset(64)] public byte overflow;
    }
}
