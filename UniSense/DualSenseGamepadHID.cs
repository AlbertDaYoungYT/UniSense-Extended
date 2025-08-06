using UniSense.LowLevel;
using UniSense.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace UniSense
{
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

        public IntegerControl touch0Index { get; private set; }
        public ButtonControl touch0Detection { get; private set; }
        public IntegerControl touch0X { get; private set; }
        public IntegerControl touch0Y { get; private set; }

        public IntegerControl touch1Index { get; private set; }
        public ButtonControl touch1Detection { get; private set; }
        public IntegerControl touch1X { get; private set; }
        public IntegerControl touch1Y { get; private set; }



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
            InputSystem.RegisterLayout<TouchIndexControl>();
            InputSystem.RegisterLayout<ShortControl>();
            InputSystem.RegisterLayout<DS5_TouchXAxisControl>();
            InputSystem.RegisterLayout<DS5_TouchYAxisControl>();
            //InputSystem.RegisterLayout<DualSenseTouchPoint>();
            InputSystem.RegisterLayout<DualSenseGamepadHID>(
                matches: new InputDeviceMatcher()
                    .WithInterface("HID")
                    .WithManufacturer("Sony.+Entertainment")
                    .WithCapability("vendorId", 0x54C)
                    .WithCapability("productId", 0xCE6));
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            touch0Index = GetChildControl<IntegerControl>("touch0Index");
            touch0Detection = GetChildControl<ButtonControl>("touch0Detection");
            touch0X = GetChildControl<IntegerControl>("touch0X");
            touch0Y = GetChildControl<IntegerControl>("touch0Y");

            touch1Index = GetChildControl<IntegerControl>("touch1Index");
            touch1Detection = GetChildControl<ButtonControl>("touch1Detection");
            touch1X = GetChildControl<IntegerControl>("touch1X");
            touch1Y = GetChildControl<IntegerControl>("touch1Y");

            leftTriggerButton = GetChildControl<ButtonControl>("leftTriggerButton");
            rightTriggerButton = GetChildControl<ButtonControl>("rightTriggerButton");
            playStationButton = GetChildControl<ButtonControl>("systemButton");

            micMuteButton = GetChildControl<ButtonControl>("micMuteButton");
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