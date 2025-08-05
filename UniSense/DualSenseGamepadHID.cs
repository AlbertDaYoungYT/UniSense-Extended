using UniSense.LowLevel;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;

namespace UniSense
{
    public unsafe class DualSenseTouchPoint : InputControl
    {
        // Expose the individual controls that are defined in DualSenseTouchPointState
        public IntegerControl touchId { get; private set; }
        public Vector2Control position { get; private set; }
        public AxisControl pressure { get; private set; }
        public ButtonControl isPressed { get; private set; }

        public override int valueSizeInBytes => sizeof(DualSenseTouchPointState);
        public override System.Type valueType => typeof(DualSenseTouchPointState);

        protected override void FinishSetup()
        {
            // Get references to the controls from the builder based on their names
            // as defined in the InputControl attributes within DualSenseTouchPointState.
            touchId = GetChildControl<IntegerControl>("touchId");
            position = GetChildControl<Vector2Control>("position");
            pressure = GetChildControl<AxisControl>("pressure");
            isPressed = GetChildControl<ButtonControl>("isPressed");

            // If rawX/rawY from DualSenseTouchPointState need custom processing to form Vector2,
            // this is where you might add processors or custom logic.
            // For example:
            // position.SetProcessor(new DualSenseTouchpadPositionProcessor());
            base.FinishSetup();
        }

        public override bool CompareValue(void* firstStatePtr, void* secondStatePtr)
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

        public override object ReadValueFromBufferAsObject(void* buffer, int bufferSize)
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

        public override object ReadValueFromStateAsObject(void* statePtr)
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

        public override void ReadValueFromStateIntoBuffer(void* statePtr, void* bufferPtr, int bufferSize)
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


    [InputControlLayout(
        stateType = typeof(DualSenseHIDInputReport),
        displayName = "DualSense Extended Gamepad")]
    [Preserve]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class DualSenseGamepadHID : DualShockGamepad
    {
        public ButtonControl leftTriggerButton { get; protected set; }
        public ButtonControl rightTriggerButton { get; protected set; }
        public ButtonControl playStationButton { get; protected set; }

        public ButtonControl micMuteButton { get; protected set; }

        public DualSenseTouchPoint touch0 { get; private set; }
        public DualSenseTouchPoint touch1 { get; private set; }



#if UNITY_EDITOR
        static DualSenseGamepadHID()
        {
            Initialize();
        }
#endif

        /// <summary>
        /// Finds the first DualSense connected by the player or <c>null</c> if 
        /// there is no one connected to the system.
        /// </summary>
        /// <returns>A DualSenseGamepadHID instance or <c>null</c>.</returns>
        public static DualSenseGamepadHID FindFirst()
        {
            foreach (var gamepad in all)
            {
                var isDualSenseGamepad = gamepad is DualSenseGamepadHID;
                if (isDualSenseGamepad) return gamepad as DualSenseGamepadHID;
            }

            return null;
        }

        /// <summary>
        /// Finds the DualSense last used/connected by the player or <c>null</c> if 
        /// there is no one connected to the system.
        /// </summary>
        /// <returns>A DualSenseGamepadHID instance or <c>null</c>.</returns>
        public static DualSenseGamepadHID FindCurrent() => Gamepad.current as DualSenseGamepadHID;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            InputSystem.RegisterLayout<DualSenseGamepadHID>(
                matches: new InputDeviceMatcher()
                    .WithInterface("HID")
                    .WithManufacturer("Sony.+Entertainment")
                    .WithCapability("vendorId", 0x54C)
                    .WithCapability("productId", 0xCE6));
        }

        protected override void FinishSetup()
        {
            touch0 = GetChildControl<DualSenseTouchPoint>("touch0");
            touch1 = GetChildControl<DualSenseTouchPoint>("touch1");

            leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
            rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");
            playStationButton = GetChildControl<ButtonControl>("systemButton");

            micMuteButton = GetChildControl<ButtonControl>("micMuteButton");

            base.FinishSetup();
        }

        private bool MotorHasValue => m_LowFrequencyMotorSpeed.HasValue || m_HighFrequenceyMotorSpeed.HasValue;
        private bool LeftTriggerHasValue => m_leftTriggerState.HasValue;
        private bool RightTriggerHasValue => m_rightTriggerState.HasValue;

        public override void PauseHaptics()
        {
            if (!MotorHasValue && !LeftTriggerHasValue && !RightTriggerHasValue)
                return;

            var command = DualSenseHIDOutputReport.Create();
            command.ResetMotorSpeeds();
            command.SetLeftTriggerState(new DualSenseTriggerState());
            command.SetRightTriggerState(new DualSenseTriggerState());

            ExecuteCommand(ref command);
        }

        public override void ResetHaptics()
        {
            if (!MotorHasValue && !LeftTriggerHasValue && !RightTriggerHasValue)
                return;

            var command = DualSenseHIDOutputReport.Create();
            command.ResetMotorSpeeds();
            command.SetLeftTriggerState(new DualSenseTriggerState());
            command.SetRightTriggerState(new DualSenseTriggerState());

            ExecuteCommand(ref command);

            m_HighFrequenceyMotorSpeed = null;
            m_LowFrequencyMotorSpeed = null;
        }

        public void ResetMotorSpeeds() => SetMotorSpeeds(0f, 0f);

        public void ResetLightBarColor() => SetLightBarColor(Color.black);

        public void ResetTriggersState()
        {
            var command = DualSenseHIDOutputReport.Create();
            command.SetRightTriggerState(m_rightTriggerState.Value);
            command.SetLeftTriggerState(m_leftTriggerState.Value);

            ExecuteCommand(ref command);
        }

        public void Reset()
        {
            ResetHaptics();
            ResetMotorSpeeds();
            ResetLightBarColor();
            ResetTriggersState();
        }

        public override void ResumeHaptics()
        {
            if (!MotorHasValue && !LeftTriggerHasValue && !RightTriggerHasValue)
                return;

            var command = DualSenseHIDOutputReport.Create();
            if (MotorHasValue) command.SetMotorSpeeds(m_LowFrequencyMotorSpeed.Value, m_HighFrequenceyMotorSpeed.Value);
            if (LeftTriggerHasValue) command.SetLeftTriggerState(m_leftTriggerState.Value);
            if (RightTriggerHasValue) command.SetRightTriggerState(m_rightTriggerState.Value);

            ExecuteCommand(ref command);
        }

        public override void SetLightBarColor(Color color)
        {
            var command = DualSenseHIDOutputReport.Create();
            command.SetLightBarColor(color);

            ExecuteCommand(ref command);
        }

        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            var command = DualSenseHIDOutputReport.Create();
            command.SetMotorSpeeds(lowFrequency, highFrequency);

            ExecuteCommand(ref command);

            m_LowFrequencyMotorSpeed = lowFrequency;
            m_HighFrequenceyMotorSpeed = highFrequency;
        }

        public void SetGamepadState(DualSenseGamepadState state)
        {
            var command = DualSenseHIDOutputReport.Create();

            if (state.LightBarColor.HasValue)
            {
                var lightBarColor = state.LightBarColor.Value;
                command.SetLightBarColor(lightBarColor);
            }

            if (state.Motor.HasValue)
            {
                var motor = state.Motor.Value;
                command.SetMotorSpeeds(motor.LowFrequencyMotorSpeed, motor.HighFrequenceyMotorSpeed);
                m_LowFrequencyMotorSpeed = motor.LowFrequencyMotorSpeed;
                m_HighFrequenceyMotorSpeed = motor.HighFrequenceyMotorSpeed;
            }

            if (state.MicLed.HasValue)
            {
                var micLed = state.MicLed.Value;
                command.SetMicLedState(micLed);
            }

            if (state.RightTrigger.HasValue)
            {
                var rightTriggerState = state.RightTrigger.Value;
                command.SetRightTriggerState(rightTriggerState);
                m_rightTriggerState = rightTriggerState;
            }

            if (state.LeftTrigger.HasValue)
            {
                var leftTriggerState = state.LeftTrigger.Value;
                command.SetLeftTriggerState(leftTriggerState);
                m_leftTriggerState = leftTriggerState;
            }

            if (state.PlayerLedBrightness.HasValue)
            {
                var playerLedBrightness = state.PlayerLedBrightness.Value;
                command.SetPlayerLedBrightness(playerLedBrightness);
            }

            if (state.PlayerLed.HasValue)
            {
                var playerLed = state.PlayerLed.Value;
                command.SetPlayerLedState(playerLed);
            }

            ExecuteCommand(ref command);
        }

        private float? m_LowFrequencyMotorSpeed;
        private float? m_HighFrequenceyMotorSpeed;
        private DualSenseTriggerState? m_rightTriggerState;
        private DualSenseTriggerState? m_leftTriggerState;
    }
}