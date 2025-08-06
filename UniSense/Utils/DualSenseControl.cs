using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using System.Runtime.InteropServices;
using System;

namespace UniSense.Utils
{

    [InputControlLayout(displayName = "DualSense Touch Index", stateType = typeof(IntegerControl))]
    public class DS5_TouchIndexControl : IntegerControl
    {
        public DS5_TouchIndexControl()
        {
            m_StateBlock.format = InputStateBlock.FormatInt;
        }

        public unsafe override int ReadUnprocessedValueFromState(void* statePtr)
        {
            // The touch index is a 7-bit value.
            // The base IntegerControl expects a full integer.
            // We need to read the byte and mask out the relevant 7 bits.
            byte value = *(byte*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
            return value & 0x7F; // Mask to get the first 7 bits
        }

        public unsafe override void WriteValueIntoState(int value, void* statePtr)
        {
            // When writing, we only care about the lower 7 bits.
            // The 8th bit is used for touch detection.
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;
            *ptr = (byte)(value & 0x7F); // Write only the lower 7 bits
        }

        protected override FourCC CalculateOptimizedControlDataType()
        {
            if (m_StateBlock.format == InputStateBlock.FormatInt && m_StateBlock.sizeInBits == 7 && m_StateBlock.byteOffset == 0)
            {
                return InputStateBlock.FormatInt;
            }
            return InputStateBlock.FormatInvalid;
        }
    }


    [InputControlLayout(displayName = "DualSense Is Touching", stateType = typeof(ButtonControl))]
    public class DS5_IsTouchingControl : ButtonControl
    {
        public DS5_IsTouchingControl()
        {
            m_StateBlock.format = InputStateBlock.FormatBit;
        }

        public unsafe override float ReadUnprocessedValueFromState(void* statePtr)
        {
            // The touch detection bit is the 8th bit (bit 7) of the byte.
            byte value = *(byte*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
            return ((value >> 7) & 0x01); // Extract bit 7
        }

        public unsafe override void WriteValueIntoState(float value, void* statePtr)
        {
            // When writing, set or clear the 8th bit (bit 7).
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;
            if (value >= 0.5f)
            {
                *ptr |= 0x80; // Set bit 7
            }
            else
            {
                *ptr &= unchecked((byte)~0x80); // Clear bit 7
            }
        }

        protected override FourCC CalculateOptimizedControlDataType()
        {
            if (m_StateBlock.format == InputStateBlock.FormatBit && m_StateBlock.sizeInBits == 1 && m_StateBlock.bitOffset == 7)
            {
                return InputStateBlock.FormatBit;
            }
            return InputStateBlock.FormatInvalid;
        }
    }


    [InputControlLayout(displayName = "Short")]
    public class ShortControl : InputControl<short>
    {
        public ShortControl()
        {
            m_StateBlock.format = InputStateBlock.FormatShort;
        }

        public unsafe override short ReadUnprocessedValueFromState(void* statePtr)
        {
            return *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
        }

        public unsafe override void WriteValueIntoState(short value, void* statePtr)
        {
            *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset) = (short)value;
        }

        protected override FourCC CalculateOptimizedControlDataType()
        {
            if (m_StateBlock.format == InputStateBlock.FormatShort && m_StateBlock.sizeInBits == 16 && m_StateBlock.bitOffset == 0)
            {
                return InputStateBlock.FormatShort;
            }
            return InputStateBlock.FormatInvalid;
        }
    }

/*
    [InputControlLayout(displayName = "Touch X Axis", stateType = typeof(ShortControl))]
    public class DS5_TouchXAxisControl : ShortControl
    {
        public DS5_TouchXAxisControl()
        {
            // The X coordinate is 12 bits, starting at bit 0 of the 1st byte (offset 33)
            // in the DualSenseHIDInputReport.
            // The Input System handles bit packing, so we just need to specify the size.
            m_StateBlock.format = InputStateBlock.FormatShort; // Use short to read 16 bits
            m_StateBlock.sizeInBits = 12; // Only 12 bits are relevant for the value
            m_StateBlock.bitOffset = 0; // Starts at bit 0 of its byte
        }

        public unsafe override short ReadUnprocessedValueFromState(void* statePtr)
        {
            // Read the 16-bit short value from the state pointer.
            // The Input System's bitfield handling should take care of the 12-bit extraction.
            short rawValue = *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset);

            // Mask to get only the lower 12 bits, as the value is 12-bit.
            // This is a safeguard, as Input System's bitOffset/sizeInBits should handle it.
            return (short)(rawValue & 0x0FFF); // 0x0FFF is 12 bits set to 1
        }

        public unsafe override void WriteValueIntoState(short value, void* statePtr)
        {
            // When writing, ensure the value fits within 12 bits.
            short maskedValue = (short)(value & 0x0FFF);
            *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset) = maskedValue;
        }

        protected override FourCC CalculateOptimizedControlDataType()
        {
            if (m_StateBlock.format == InputStateBlock.FormatShort && m_StateBlock.sizeInBits == 12 && m_StateBlock.bitOffset == 0)
            {
                return InputStateBlock.FormatShort;
            }
            return InputStateBlock.FormatInvalid;
        }
    }


    [InputControlLayout(displayName = "Touch Y Axis", stateType = typeof(ShortControl))]
    public class DS5_TouchYAxisControl : ShortControl
    {
        public DS5_TouchYAxisControl()
        {
            // The Y coordinate is 12 bits. The state block properties are
            // configured to instruct the Input System how to read the data.
            m_StateBlock.format = InputStateBlock.FormatShort;
            m_StateBlock.sizeInBits = 12;
            m_StateBlock.bitOffset = 4;
        }

        public unsafe override short ReadUnprocessedValueFromState(void* statePtr)
        {
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

            short value = (short)(*ptr >> 4);
            value |= (short)(*(ptr + 1) << 4);

            return (short)(value & 0x0FFF);
        }

        public unsafe override void WriteValueIntoState(short value, void* statePtr)
        {
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

            *ptr &= unchecked((byte)~(0x3 << 4));
            *ptr |= (byte)((value & 0x3) << 4);

            *(ptr + 1) = (byte)(value >> 4);
        }
    }*/

    [InputControlLayout(displayName = "DualSense Touch Axis", stateType = typeof(ShortControl))]
    public class DS5_TouchAxisControl : ShortControl
    {
        public DS5_TouchAxisControl()
        {
            m_StateBlock.format = InputStateBlock.FormatShort;
            m_StateBlock.sizeInBits = 12;
        }

        public unsafe override short ReadUnprocessedValueFromState(void* statePtr)
        {
            if (m_StateBlock.bitOffset == 0)
            {
                // Read the 16-bit short value from the state pointer.
                // The Input System's bitfield handling should take care of the 12-bit extraction.
                short rawValue = *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset);

                // Mask to get only the lower 12 bits, as the value is 12-bit.
                // This is a safeguard, as Input System's bitOffset/sizeInBits should handle it.
                return (short)(rawValue & 0x0FFF); // 0x0FFF is 12 bits set to 1
            }
            else
            if (m_StateBlock.bitOffset == 4)
            {
                byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

                short value = (short)(*ptr >> 4);
                value |= (short)(*(ptr + 1) << 4);

                return (short)(value & 0x0FFF);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public unsafe override void WriteValueIntoState(short value, void* statePtr)
        {
            if (m_StateBlock.bitOffset == 0)
            {
                // When writing, ensure the value fits within 12 bits.
                short maskedValue = (short)(value & 0x0FFF);
                *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset) = maskedValue;
                return;
            }
            else if (m_StateBlock.bitOffset == 4)
            {
                byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

                *ptr &= unchecked((byte)~(0x3 << 4));
                *ptr |= (byte)((value & 0x3) << 4);

                *(ptr + 1) = (byte)(value >> 4);
                return;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }


    [InputControlLayout(displayName = "DualSense Touch Point", stateType = typeof(Vector2))]
    public class DS5_TouchPointControl : InputControl<Vector2>
    {
        [InputControl(name = "x", layout = "DS5_TouchAxisControl", bit = 0, sizeInBits = 12, offset = 0)]
        public DS5_TouchAxisControl x { get; protected set; }

        [InputControl(name = "y", layout = "DS5_TouchAxisControl", bit = 4, sizeInBits = 12, offset = 1)]
        public DS5_TouchAxisControl y { get; protected set; }

        public DS5_TouchPointControl()
        {
            m_StateBlock.format = InputStateBlock.FormatVector2;
        }

        protected override void FinishSetup()
        {
            x = GetChildControl<DS5_TouchAxisControl>("x");
            y = GetChildControl<DS5_TouchAxisControl>("y");
            base.FinishSetup();
        }

        public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
        {
            return new Vector2(x.ReadUnprocessedValueFromState(statePtr), y.ReadUnprocessedValueFromState(statePtr));
        }

        public unsafe override void WriteValueIntoState(Vector2 value, void* statePtr)
        {
            x.WriteValueIntoState((short)value.x, statePtr);
            y.WriteValueIntoState((short)value.y, statePtr);
        }

        public unsafe override float EvaluateMagnitude(void* statePtr)
        {
            return ReadValueFromStateWithCaching(statePtr).magnitude;
        }
    }


    [InputControlLayout(displayName = "Touchpad")]
    public class DS5_TouchpadControl : InputControl<Vector2>
    {

        [InputControl(name = "isTouching", displayName = "Is Touching", layout = "DS5_IsTouchingControl", bit = 7, sizeInBits = 1, offset = 0)]
        public DS5_IsTouchingControl isTouching { get; private set; }
        [InputControl(name = "index", displayName = "Index", layout = "DS5_TouchIndexControl", bit = 0, sizeInBits = 7, offset = 0)]
        public DS5_TouchIndexControl index { get; private set; }

        [InputControl(name = "touchPoint", displayName = "Touch Point", layout = "DS5_TouchPointControl", offset = 1)]
        [InputControl(name = "touchPoint/x", layout = "DS5_TouchAxisControl", bit = 0, sizeInBits = 12, offset = 0)]
        [InputControl(name = "touchPoint/y", layout = "DS5_TouchAxisControl", bit = 4, sizeInBits = 12, offset = 1)]
        public DS5_TouchPointControl touchPoint { get; private set; }


        public override Type valueType => throw new NotImplementedException();

        public override int valueSizeInBytes => throw new NotImplementedException();

        public DS5_TouchpadControl()
        {
            m_StateBlock.format = InputStateBlock.FormatVector2;
        }

        protected override void FinishSetup()
        {
            isTouching = GetChildControl<DS5_IsTouchingControl>("isTouching");
            index = GetChildControl<DS5_TouchIndexControl>("index");

            touchPoint = GetChildControl<DS5_TouchPointControl>("touchPoint");
            base.FinishSetup();
        }

        public unsafe override Vector2 ReadUnprocessedValueFromState(void* statePtr)
        {
            return touchPoint.ReadUnprocessedValueFromState(statePtr);
        }

        public override unsafe void WriteValueIntoState(Vector2 value, void* statePtr)
        {
            touchPoint.WriteValueIntoState(value, statePtr);
        }
        
    }
}
