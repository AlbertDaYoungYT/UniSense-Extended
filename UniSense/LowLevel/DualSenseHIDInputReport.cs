using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UniSense.LowLevel
{
    /*
    [StructLayout(LayoutKind.Explicit, Size = 6)]
    // ❌ REMOVE THIS:
    // [InputControlLayout(displayName = "DualSense Touch Point")]
    internal struct DualSenseTouchPoint
    {
        [InputControl(name = "touchId", offset = 0, sizeInBits = 7, layout = "Integer", format = "BYTE")]
        [InputControl(name = "press", offset = 0, sizeInBits = 1, layout = "Button", bit = 7)]
        [InputControl(name = "x", offset = 1, sizeInBits = 12, layout = "Axis")]
        [InputControl(name = "y", offset = 2, sizeInBits = 12, layout = "Axis")]
        [InputControl(name = "time", offset = 4, layout = "Integer", format = "INT")]
        [FieldOffset(0)] public byte rawIdAndActive;
        [FieldOffset(1)] public byte xLow;
        [FieldOffset(2)] public byte xHigh_yLow;
        [FieldOffset(3)] public byte yHigh;
        [FieldOffset(4)] public ushort timestamp;

        public bool IsActive => (rawIdAndActive & 0x80) == 0;
        public int TouchId => rawIdAndActive & 0x7F;

        public int X => (xLow | ((xHigh_yLow & 0x0F) << 8));
        public int Y => ((xHigh_yLow >> 4) | (yHigh << 4));
    }*/

    // This struct represents the raw binary data for a single touch point
    // within the DualSense HID report.
    // IMPORTANT: The Size and FieldOffset values are examples.
    // You MUST verify these against the actual DualSense HID report structure.
    // Example: 8 bytes per touch point
    [StructLayout(LayoutKind.Explicit, Size = 8)]
    public struct DualSenseTouchPointState : IInputStateTypeInfo
    {
        // A custom FourCC for DualSense touch point data.
        // This is used internally by the Input System to identify the data format.
        public FourCC format => new FourCC('D', 'S', 'T', 'P'); // DualSense Touch Point

        // FieldOffset(0) is the start of this touch point's data block.

        // Touch ID and 'Is Pressed' bit (often combined in one byte)
        // Example: Byte 0 contains touch ID (bits 0-6) and 'isPressed' (bit 7)
        [FieldOffset(0)]
        // Touch ID (0-127)
        // Is touch active?
        public byte touchStatusByte;

        // X and Y coordinates (often 12-bit values packed into 2 bytes each)
        // For simplicity, assuming 16-bit ushorts here. Actual packing might need custom processors.
        // Example: X coordinate (12 bits) starts at bit 0 of byte 1, Y coordinate (12 bits) starts at bit 4 of byte 2.
        [FieldOffset(1)]
        [InputControl(name = "position", layout = "Vector2", format = "VEC2")] // Will be mapped to a Vector2Control
        public ushort rawX; // Raw 12-bit X value (packed into 16-bit ushort)
        [FieldOffset(2)] // Assuming rawX and rawY are consecutive in the report
        public ushort rawY; // Raw 12-bit Y value (packed into 16-bit ushort)

        // Pressure value (often 1 byte)
        [FieldOffset(3)] // Example offset
    
        public byte rawPressure;

        // Note: If X/Y are packed into fewer bytes or use different bit offsets,
        // you might need custom processors or more granular bit offsets.
        // For instance, if X is 12 bits and Y is 12 bits, they might span 3 bytes.
        // E.g., Byte 1 (8 bits of X), Byte 2 (4 bits of X, 4 bits of Y), Byte 3 (8 bits of Y)
    }

    public unsafe class DualSenseTouchPoint : InputControl
    {
        // Expose the individual controls that are defined in DualSenseTouchPointState
        public IntegerControl touchId { get; private set; }
        public Vector2Control position { get; private set; }
        public AxisControl pressure { get; private set; }
        public ButtonControl isPressed { get; private set; }

        protected override void FinishSetup()
        {
            // Get references to the controls from the builder based on their names
            // as defined in the InputControl attributes within DualSenseTouchPointState.
            touchId = GetChildControl<IntegerControl>(this, "touchId");
            position = GetChildControl<Vector2Control>(this, "position");
            pressure = GetChildControl<AxisControl>(this, "pressure");
            isPressed = GetChildControl<ButtonControl>(this, "isPressed");

            // If rawX/rawY from DualSenseTouchPointState need custom processing to form Vector2,
            // this is where you might add processors or custom logic.
            // For example:
            // position.SetProcessor(new DualSenseTouchpadPositionProcessor());
            base.FinishSetup();
        }

        protected override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
        {
            var firstValue = (DualSenseTouchPointState*)firstStatePtr;
            var secondValue = (DualSenseTouchPointState*)secondStatePtr;

            // Compare individual fields for changes
            if (firstValue->touchStatusByte != secondValue->touchStatusByte)
                return false;
            if (firstValue->rawX != secondValue->rawX)
                return false;
            if (firstValue->rawY != secondValue->rawY)
                return false;
            if (firstValue->rawPressure != secondValue->rawPressure)
                return false;

            return true;
        }

        protected override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
        {
            var state = (DualSenseTouchPointState*)buffer;
            return new DualSenseTouchPointState
            {
                touchStatusByte = state->touchStatusByte,
                rawX = state->rawX,
                rawY = state->rawY,
                rawPressure = state->rawPressure
            };
        }

        protected override object ReadValueFromStateAsObject(void* statePtr)
        {
            var state = (DualSenseTouchPointState*)statePtr;
            return new DualSenseTouchPointState
            {
                touchStatusByte = state->touchStatusByte,
                rawX = state->rawX,
                rawY = state->rawY,
                rawPressure = state->rawPressure
            };
        }

        protected override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
        {
            if (bufferPtr == null)
                return;

            var state = (DualSenseTouchPointState*)statePtr;
            var buffer = (DualSenseTouchPointState*)bufferPtr;

            *buffer = *state;
            if (bufferSize < Marshal.SizeOf<DualSenseTouchPointState>())
                return;
                
            if (bufferSize > Marshal.SizeOf<DualSenseTouchPointState>())
            {
                // If the buffer is larger, zero out the rest of it.
                // This prevents old data from lingering if the new state is smaller.
                var remainingBytes = bufferSize - Marshal.SizeOf<DualSenseTouchPointState>();
                var remainingPtr = (byte*)bufferPtr + Marshal.SizeOf<DualSenseTouchPointState>();
                for (int i = 0; i < remainingBytes; ++i)
                {
                    remainingPtr[i] = 0;
                }
            }
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 78)] // Size 64
    internal struct DualSenseHIDInputReport : IInputStateTypeInfo
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

        [InputControl(name = "touch0", layout = "DualSenseTouchPoint")]
        [FieldOffset(32)] public DualSenseTouchPointState touch0;

        [InputControl(name = "touch1", layout = "DualSenseTouchPoint")]
        [FieldOffset(40)] public DualSenseTouchPointState touch1;


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
