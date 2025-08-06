using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

namespace UniSense.Utils
{

    [InputControlLayout(displayName = "Touch Index", stateType = typeof(IntegerControl))]
    public class TouchIndexControl : IntegerControl
    {
        public TouchIndexControl()
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


    [InputControlLayout(displayName = "Short", stateType = typeof(short))]
    public class ShortControl : IntegerControl
    {
        public ShortControl()
        {
            m_StateBlock.format = InputStateBlock.FormatShort;
        }

        public unsafe override int ReadUnprocessedValueFromState(void* statePtr)
        {
            return *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
        }

        public unsafe override void WriteValueIntoState(int value, void* statePtr)
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

        public unsafe override int ReadUnprocessedValueFromState(void* statePtr)
        {
            // Read the 16-bit short value from the state pointer.
            // The Input System's bitfield handling should take care of the 12-bit extraction.
            short rawValue = *(short*)((byte*)statePtr + (int)m_StateBlock.byteOffset);

            // Mask to get only the lower 12 bits, as the value is 12-bit.
            // This is a safeguard, as Input System's bitOffset/sizeInBits should handle it.
            return rawValue & 0x0FFF; // 0x0FFF is 12 bits set to 1
        }

        public unsafe override void WriteValueIntoState(int value, void* statePtr)
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

        public unsafe override int ReadUnprocessedValueFromState(void* statePtr)
        {
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

            int value = (*ptr >> 4);
            value |= (*(ptr + 1) << 4);

            return value & 0x0FFF;
        }

        public unsafe override void WriteValueIntoState(int value, void* statePtr)
        {
            byte* ptr = (byte*)statePtr + (int)m_StateBlock.byteOffset;

            *ptr &= unchecked((byte)~(0x3 << 4));
            *ptr |= (byte)((value & 0x3) << 4);

            *(ptr + 1) = (byte)(value >> 4);
        }
    }
}
