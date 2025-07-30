using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public enum DS5W_ReturnValue : uint
{
    OK = 0,
    E_UNKNOWN = 1,
    E_INSUFFICIENT_BUFFER = 2,
    E_EXTERNAL_WINAPI = 3,
    E_STACK_OVERFLOW = 4,
    E_INVALID_ARGS = 5,
    E_CURRENTLY_NOT_SUPPORTED = 6,
    E_DEVICE_REMOVED = 7,
    E_BT_COM = 8,
    E_IO_TIMEDOUT = 9,
    E_IO_FAILED = 10,
    E_IO_NOT_FOUND = 11,
    E_IO_PENDING = 12,
}

// Enum for device connection type (unchanged)
public enum DeviceConnection : byte
{
    USB = 0,
    BT = 1,
}

// Axis Calibration Data
[StructLayout(LayoutKind.Sequential)]
public struct AxisCalibrationData
{
    public short bias;
    public int sens_numer;
    public int sens_denom;
}

// Device Calibration Data
[StructLayout(LayoutKind.Sequential)]
public struct DeviceCalibrationData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public AxisCalibrationData[] accelerometer;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public AxisCalibrationData[] gyroscope;

    public DeviceCalibrationData(bool init = true)
    {
        if (init)
        {
            accelerometer = new AxisCalibrationData[3];
            gyroscope = new AxisCalibrationData[3];
        }
        else
        {
            accelerometer = null;
            gyroscope = null;
        }
    }
}

// Struckt for storing device enum info while device discovery (updated)
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct DeviceEnumInfo
{
    // Flattening the _internal struct for C#
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string path;

    public DeviceConnection connection;

    public uint uniqueID; // UINT32 in C++
}

// OVERLAPPED struct for async I/O (Windows specific)
[StructLayout(LayoutKind.Sequential)]
public struct OVERLAPPED
{
    public UIntPtr Internal;
    public UIntPtr InternalHigh;
    public uint Offset;
    public uint OffsetHigh;
    public IntPtr hEvent;
}

// Device context (updated)
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public unsafe struct DeviceContext // Mark as unsafe
{
    // Flattening the _internal struct for C#
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string devicePath;

    public uint uniqueID; // UINT32 in C++

    public IntPtr deviceHandle; // HANDLE in C++

    public OVERLAPPED olRead;
    public OVERLAPPED olWrite;

    public DeviceConnection connectionType;

    public DeviceCalibrationData calibrationData;

    public uint timestamp; // unsigned int in C++

    [MarshalAs(UnmanagedType.U1)]
    public bool connected;

    // Changed to fixed byte arrays
    public fixed byte hidInBuffer[DualSenseConstants.DS_MAX_INPUT_REPORT_SIZE];
    public fixed byte hidOutBuffer[DualSenseConstants.DS_MAX_OUTPUT_REPORT_SIZE];

    // Constructor to ensure arrays and structs are initialized
    public DeviceContext(bool init = true)
    {
        devicePath = null;
        uniqueID = 0;
        deviceHandle = IntPtr.Zero;
        olRead = new OVERLAPPED();
        olWrite = new OVERLAPPED();
        connectionType = DeviceConnection.USB;
        calibrationData = new DeviceCalibrationData(init);
        timestamp = 0;
        connected = false;
        // Fixed buffers don't need 'new' allocation
        // They are part of the struct's memory directly
    }
}

// Analog stick (unchanged)
[StructLayout(LayoutKind.Sequential)]
public struct AnalogStick
{
    public sbyte x; // char in C++ is signed
    public sbyte y; // char in C++ is signed
}

// 3 Component vector (updated to int)
[StructLayout(LayoutKind.Sequential)]
public struct Vector3I
{
    public int x;
    public int y;
    public int z;
}

// RGB Color (unchanged)
[StructLayout(LayoutKind.Sequential)]
public struct ColorRGB
{
    public byte r;
    public byte g;
    public byte b;
}

// Touchpad state (updated)
[StructLayout(LayoutKind.Sequential)]
public struct Touch
{
    public uint x;
    public uint y;

    [MarshalAs(UnmanagedType.U1)]
    public bool down;

    public byte id;
}

// Battery information (updated field name)
[StructLayout(LayoutKind.Sequential)]
public struct Battery
{
    [MarshalAs(UnmanagedType.U1)]
    public bool charging; // Renamed from 'chargin'

    [MarshalAs(UnmanagedType.U1)]
    public bool fullyCharged;

    public byte level;
}

// State of the mic led (unchanged)
public enum MicLed : byte
{
    OFF = 0x00,
    ON = 0x01,
    PULSE = 0x02,
}

// Type of trigger effect (updated with ReleaseAll)
public enum TriggerEffectType : byte
{
    NoResitance = 0x00,
    ContinuousResitance = 0x01,
    SectionResitance = 0x02,
    ReleaseAll = 0x05, // New
    EffectEx = 0x26,
    Calibrate = 0xFC,
}

// Trigger effect (Union handling - updated size and EffectEx pad)
[StructLayout(LayoutKind.Explicit)]
public unsafe struct TriggerEffect // Marked as unsafe
{
    [FieldOffset(0)]
    public TriggerEffectType effectType;

    // Raw data for the union - changed to fixed buffer, size 10
    [FieldOffset(1)]
    public fixed byte _u1_raw[10]; // Changed size from 6 to 10

    // For type == ContinuousResitance
    [FieldOffset(1)]
    public byte Continuous_StartPosition;
    [FieldOffset(2)]
    public byte Continuous_Force;
    // _pad[8] is implicitly handled by the layout and size of the struct

    // For type == SectionResitance
    [FieldOffset(1)]
    public byte Section_StartPosition;
    [FieldOffset(2)]
    public byte Section_EndPosition;
    // _pad[8] is implicitly handled

    // For type == EffectEx
    [FieldOffset(1)]
    public byte EffectEx_StartPosition;
    [FieldOffset(2)]
    [MarshalAs(UnmanagedType.U1)]
    public bool EffectEx_KeepEffect;
    [FieldOffset(3)]
    public byte EffectEx_BeginForce;
    [FieldOffset(4)]
    public byte EffectEx_MiddleForce;
    [FieldOffset(5)]
    public byte EffectEx_EndForce;
    [FieldOffset(6)]
    public byte EffectEx_Frequency;
    // _pad[4] is implicitly handled

    public TriggerEffect(TriggerEffectType type)
    {
        effectType = type;
        // _u1_raw is a fixed buffer, no need to allocate with 'new'
        Continuous_StartPosition = 0;
        Continuous_Force = 0;
        Section_StartPosition = 0;
        Section_EndPosition = 0;
        EffectEx_StartPosition = 0;
        EffectEx_KeepEffect = false;
        EffectEx_BeginForce = 0;
        EffectEx_MiddleForce = 0;
        EffectEx_EndForce = 0;
        EffectEx_Frequency = 0;

        // Initialize fixed buffers to zero
        for (int i = 0; i < 10; i++)
        {
            _u1_raw[i] = 0;
        }
    }
}

// Led brightness (unchanged)
public enum LedBrightness : byte
{
    LOW = 0x02,
    MEDIUM = 0x01,
    HIGH = 0x00,
}

// Player leds values (unchanged)
[StructLayout(LayoutKind.Sequential)]
public struct PlayerLeds
{
    public byte bitmask;

    [MarshalAs(UnmanagedType.U1)]
    public bool playerLedFade;

    public LedBrightness brightness;
}

// Input state of the controller (updated)
//[StructLayout(LayoutKind.Sequential)]
//public struct DS5InputState
//{
//    public AnalogStick leftStick;
//    public AnalogStick rightStick;
//
//    public uint buttonMap; // New combined button map
//
//    public byte leftTrigger;
//    public byte rightTrigger;
//
//    public Vector3I accelerometer; // Changed to Vector3I (int components)
//    public Vector3I gyroscope;     // Changed to Vector3I (int components)
//
//    public Touch touchPoint1;
//    public Touch touchPoint2;
//
//    public uint currentTime; // New
//    public uint deltaTime;   // New
//
//    public Battery battery;
//
//    [MarshalAs(UnmanagedType.U1)]
//    public bool headPhoneConnected;
//
//    public byte leftTriggerFeedback;
//    public byte rightTriggerFeedback;
//
//    // Helper properties for button states (updated to use buttonMap)
//    public bool DPadLeft => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_DPAD_LEFT) != 0;
//    public bool DPadDown => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_DPAD_DOWN) != 0;
//    public bool DPadRight => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_DPAD_RIGHT) != 0;
//    public bool DPadUp => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_DPAD_UP) != 0;
//
//    public bool Square => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_SQUARE) != 0;
//    public bool Cross => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_CROSS) != 0;
//    public bool Circle => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_CIRCLE) != 0;
//    public bool Triangle => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_TRIANGLE) != 0;
//
//    public bool LeftBumper => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_BUMPER_LEFT) != 0;
//    public bool RightBumper => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_BUMPER_RIGHT) != 0;
//    public bool LeftTriggerButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_TRIGGER_LEFT) != 0;
//    public bool RightTriggerButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_TRIGGER_RIGHT) != 0;
//
//    public bool ShareButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_SELECT) != 0;
//    public bool OptionsButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_MENU) != 0;
//    public bool LeftStickClick => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_STICK_LEFT) != 0;
//    public bool RightStickClick => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_STICK_RIGHT) != 0;
//
//    public bool PlayStationButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_PLAYSTATION_LOGO) != 0;
//    public bool TouchpadClick => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_PAD_BUTTON) != 0;
//    public bool MicButton => (buttonMap & DualSenseConstants.DS5W_ISTATE_BTN_MIC_BUTTON) != 0;
//}

[StructLayout(LayoutKind.Explicit, Size = DualSenseConstants.DS_MAX_INPUT_REPORT_SIZE)]
public struct DS5InputState : IInputStateTypeInfo
{
    // This is the identifier for your custom data format.
    // Make sure it's 1-4 characters long. 'DS5I' (DualSense 5 Input) is a good choice.
    public FourCC format => new FourCC('D', 'S', 'V', 'W'); // Ensure this is 1-4 chars!

    // ====================================================================================
    // Mapped Controls (Matching _DS5InputState from ds5w.h)
    // ====================================================================================

    // Offset: 0
    [InputControl(name = "leftStick", layout = "Stick", format = "VC2B", offset = 0, displayName = "Left Stick")]
    [FieldOffset(0)] public Vector2 leftStick; // C++: AnalogStick leftStick (char x, char y) -> 2 bytes. Mapped to Vector2.

    // Offset: 2
    [InputControl(name = "rightStick", layout = "Stick", format = "VC2B", offset = 2, displayName = "Right Stick")]
    [FieldOffset(2)] public Vector2 rightStick; // C++: AnalogStick rightStick (char x, char y) -> 2 bytes. Mapped to Vector2.

    // Offset: 4
    // Button Map (unsigned int) - contains all digital button states as bitflags
    [InputControl(name = "buttons", layout = "Button", format = "UINT", displayName = "Buttons (Raw Bitmask)")]
    [FieldOffset(4)] public uint buttonMap; // C++: unsigned int buttonMap -> 4 bytes

    // Individual Buttons from buttonMap using bit offsets
    // D-Pad
    [InputControl(name = "dpad", layout = "Dpad", format = "BIT", bit = 0, sizeInBits = 4, displayName = "D-Pad")]
    [InputControl(name = "dpad/left", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_DPAD_LEFT, format = "BIT", displayName = "D-Pad Left")]
    [InputControl(name = "dpad/down", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_DPAD_DOWN, format = "BIT", displayName = "D-Pad Down")]
    [InputControl(name = "dpad/right", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_DPAD_RIGHT, format = "BIT", displayName = "D-Pad Right")]
    [InputControl(name = "dpad/up", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_DPAD_UP, format = "BIT", displayName = "D-Pad Up")]

    // Face Buttons
    [InputControl(name = "squareButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_SQUARE, format = "BIT", displayName = "Square")]
    [InputControl(name = "crossButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_CROSS, format = "BIT", displayName = "Cross")]
    [InputControl(name = "circleButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_CIRCLE, format = "BIT", displayName = "Circle")]
    [InputControl(name = "triangleButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_TRIANGLE, format = "BIT", displayName = "Triangle")]

    // Shoulder/Trigger Buttons (as digital press)
    [InputControl(name = "leftBumper", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_BUMPER_LEFT, format = "BIT", displayName = "Left Bumper")]
    [InputControl(name = "rightBumper", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_BUMPER_RIGHT, format = "BIT", displayName = "Right Bumper")]
    [InputControl(name = "leftTriggerButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_TRIGGER_LEFT, format = "BIT", displayName = "Left Trigger Button")]
    [InputControl(name = "rightTriggerButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_TRIGGER_RIGHT, format = "BIT", displayName = "Right Trigger Button")]

    // Menu Buttons
    [InputControl(name = "share", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_SELECT, format = "BIT", displayName = "Share")]
    [InputControl(name = "options", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_MENU, format = "BIT", displayName = "Options")]

    // Stick Clicks
    [InputControl(name = "leftStickClick", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_STICK_LEFT, format = "BIT", displayName = "Left Stick Click")]
    [InputControl(name = "rightStickClick", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_STICK_RIGHT, format = "BIT", displayName = "Right Stick Click")]

    // Extra Buttons
    [InputControl(name = "playStationButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_PLAYSTATION_LOGO, format = "BIT", displayName = "PS Button")]
    [InputControl(name = "touchpadButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_PAD_BUTTON, format = "BIT", displayName = "Touchpad Click")]
    [InputControl(name = "micButton", layout = "Button", bit = DualSenseConstants.DS5W_ISTATE_BTN_MIC_BUTTON, format = "BIT", displayName = "Mic Button")]


    // Offset: 8
    [InputControl(name = "leftTrigger", layout = "Axis", format = "BYTE", offset = 8, parameters = "normalize=true,min=0,max=255", displayName = "Left Trigger (Analog)")]
    [FieldOffset(8)] public byte leftTrigger; // C++: unsigned char leftTrigger -> 1 byte

    // Offset: 9
    [InputControl(name = "rightTrigger", layout = "Axis", format = "BYTE", offset = 9, parameters = "normalize=true,min=0,max=255", displayName = "Right Trigger (Analog)")]
    [FieldOffset(9)] public byte rightTrigger; // C++: unsigned char rightTrigger -> 1 byte

    // Offset: 10
    // Accelerometer (Vector3 of int)
    // Note: The C++ Vector3 uses `int` (4 bytes each). Total 12 bytes.
    [InputControl(name = "accelerometer", layout = "Vector3", format = "VC3S", offset = 10, displayName = "Accelerometer")] // VC3S for Vector3 of signed shorts, but C++ is INT. Need to verify interpretation. If Input System reads as short, data will be wrong.
    [FieldOffset(10)] public Vector3 accelerometer; // C++: Vector3 accelerometer (int x, int y, int z) -> 12 bytes

    // Offset: 22
    // Gyroscope (Vector3 of int)
    [InputControl(name = "gyroscope", layout = "Vector3", format = "VC3S", offset = 22, displayName = "Gyroscope")] // VC3S for Vector3 of signed shorts. See above comment.
    [FieldOffset(22)] public Vector3 gyroscope; // C++: Vector3 gyroscope (int x, int y, int z) -> 12 bytes

    // Offset: 34 (TouchPoint1)
    // Touch 1 (x, y, down, id)
    [InputControl(name = "touch/touch0", layout = "Touch", displayName = "Touch 0")]
    [InputControl(name = "touch/touch0/x", layout = "Axis", format = "UINT", offset = 34, parameters = "normalize=false,min=0,max=1920", displayName = "Touch 0 X")]
    [InputControl(name = "touch/touch0/y", layout = "Axis", format = "UINT", offset = 38, parameters = "normalize=false,min=0,max=1080", displayName = "Touch 0 Y")]
    [InputControl(name = "touch/touch0/pressure", layout = "Button", format = "BYTE", offset = 42, parameters = "normalize=true,min=0,max=255", displayName = "Touch 0 Pressure")] // Assuming 'down' relates to pressure. C++ is 'bool down'.
    [InputControl(name = "touch/touch0/id", layout = "Axis", format = "BYTE", offset = 43, displayName = "Touch 0 ID")] // Touch ID
    [FieldOffset(34)] public uint touch0X; // C++: unsigned int x
    [FieldOffset(38)] public uint touch0Y; // C++: unsigned int y
    [FieldOffset(42)] public byte touch0Pressure; // C++: bool down (treating as 0/1 byte pressure)
    [FieldOffset(43)] public byte touch0Id;     // C++: unsigned char id

    // Offset: 44 (TouchPoint2)
    // Touch 2 (x, y, down, id)
    [InputControl(name = "touch/touch1", layout = "Touch", displayName = "Touch 1")]
    [InputControl(name = "touch/touch1/x", layout = "Axis", format = "UINT", offset = 44, parameters = "normalize=false,min=0,max=1920", displayName = "Touch 1 X")]
    [InputControl(name = "touch/touch1/y", layout = "Axis", format = "UINT", offset = 48, parameters = "normalize=false,min=0,max=1080", displayName = "Touch 1 Y")]
    [InputControl(name = "touch/touch1/pressure", layout = "Button", format = "BYTE", offset = 52, parameters = "normalize=true,min=0,max=255", displayName = "Touch 1 Pressure")] // Assuming 'down' relates to pressure
    [InputControl(name = "touch/touch1/id", layout = "Axis", format = "BYTE", offset = 53, displayName = "Touch 1 ID")] // Touch ID
    [FieldOffset(44)] public uint touch1X; // C++: unsigned int x
    [FieldOffset(48)] public uint touch1Y; // C++: unsigned int y
    [FieldOffset(52)] public byte touch1Pressure; // C++: bool down (treating as 0/1 byte pressure)
    [FieldOffset(53)] public byte touch1Id;     // C++: unsigned char id

    // Offset: 54
    [InputControl(name = "time", layout = "Axis", format = "UINT", offset = 54, parameters = "normalize=false", displayName = "Controller Timestamp")]
    [FieldOffset(54)] public uint currentTime; // C++: unsigned int currentTime -> 4 bytes

    // Offset: 58
    [InputControl(name = "deltaTime", layout = "Axis", format = "UINT", offset = 58, parameters = "normalize=false", displayName = "Time Since Last Input")]
    [FieldOffset(58)] public uint deltaTime; // C++: unsigned int deltaTime -> 4 bytes

    // Offset: 62 (Battery Struct)
    [InputControl(name = "battery/charging", layout = "Button", format = "BIT", offset = 62, bit = 0, displayName = "Battery Charging")]
    [InputControl(name = "battery/fullyCharged", layout = "Button", format = "BIT", offset = 63, bit = 0, displayName = "Battery Fully Charged")]
    [InputControl(name = "battery/level", layout = "Axis", format = "BYTE", offset = 64, parameters = "normalize=true,min=0,max=100", displayName = "Battery Level")]
    [FieldOffset(62)] public byte batteryCharging; // C++: bool charging (1 byte)
    [FieldOffset(63)] public byte batteryFullyCharged; // C++: bool fullyCharged (1 byte)
    [FieldOffset(64)] public byte batteryLevel; // C++: unsigned char level (1 byte)

    // Offset: 65
    [InputControl(name = "headphoneConnected", layout = "Button", format = "BIT", offset = 65, bit = 0, displayName = "Headphone Connected")]
    [FieldOffset(65)] public byte headPhoneConnected; // C++: bool headPhoneConnected (1 byte)

    // Offset: 66
    [InputControl(name = "leftTriggerFeedback", layout = "Axis", format = "BYTE", offset = 66, parameters = "normalize=true,min=0,max=255", displayName = "Left Trigger Feedback")]
    [FieldOffset(66)] public byte leftTriggerFeedback; // C++: unsigned char leftTriggerFeedback (1 byte)

    // Offset: 67
    [InputControl(name = "rightTriggerFeedback", layout = "Axis", format = "BYTE", offset = 67, parameters = "normalize=true,min=0,max=255", displayName = "Right Trigger Feedback")]
    [FieldOffset(67)] public byte rightTriggerFeedback; // C++: unsigned char rightTriggerFeedback (1 byte)
}

// Output state of the controller (updated)
[StructLayout(LayoutKind.Sequential)]
public struct DS5OutputState
{
    public byte leftRumble;
    public byte rightRumble;

    public byte rumbleStrength; // New field

    public MicLed microphoneLed;

    [MarshalAs(UnmanagedType.U1)]
    public bool disableLeds;

    public PlayerLeds playerLeds;
    public ColorRGB lightbar;
    public TriggerEffect leftTriggerEffect;
    public TriggerEffect rightTriggerEffect;

    // Constructor to initialize default values
    public DS5OutputState(bool initializeTriggerEffects = true)
    {
        leftRumble = 0;
        rightRumble = 0;
        rumbleStrength = 0; // Initialize new field
        microphoneLed = MicLed.OFF;
        disableLeds = false;
        playerLeds = new PlayerLeds { bitmask = 0, playerLedFade = false, brightness = LedBrightness.HIGH };
        lightbar = new ColorRGB { r = 0, g = 0, b = 0 };
        leftTriggerEffect = initializeTriggerEffects ? new TriggerEffect(TriggerEffectType.NoResitance) : default;
        rightTriggerEffect = initializeTriggerEffects ? new TriggerEffect(TriggerEffectType.NoResitance) : default;
    }
}