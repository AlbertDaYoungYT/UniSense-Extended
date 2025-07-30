/*
	DualSenseWindows API
	https://github.com/mattdevv/DualSense-Windows

	MIT License

	Copyright (c) 2022 Matthew Hall
	Copyright (c) 2020 Ludwig Füchsl

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.

*/
#pragma once

#if defined(DS5W_BUILD_DLL)
#define DS5W_API __declspec(dllexport)
#elif defined(DS5W_BUILD_LIB)
#define DS5W_API
#elif defined(DS5W_USE_LIB)
#define DS5W_API
#else
#define DS5W_API __declspec(dllimport)
#endif

#define DS5W_SUCCESS(expr) ((expr) == _DS5W_ReturnValue::OK)
#define DS5W_FAILED(expr) ((expr) != _DS5W_ReturnValue::OK)

#define DS5W_OK							_DS5W_ReturnValue::OK
#define DS5W_E_UNKNOWN					_DS5W_ReturnValue::E_UNKNOWN
#define DS5W_E_INSUFFICIENT_BUFFER		_DS5W_ReturnValue::E_INSUFFICIENT_BUFFER
#define DS5W_E_EXTERNAL_WINAPI			_DS5W_ReturnValue::E_EXTERNAL_WINAPI
#define DS5W_E_STACK_OVERFLOW			_DS5W_ReturnValue::E_STACK_OVERFLOW
#define DS5W_E_INVALID_ARGS				_DS5W_ReturnValue::E_INVALID_ARGS
#define DS5W_E_CURRENTLY_NOT_SUPPORTED	_DS5W_ReturnValue::E_CURRENTLY_NOT_SUPPORTED
#define DS5W_E_DEVICE_REMOVED			_DS5W_ReturnValue::E_DEVICE_REMOVED
#define DS5W_E_BT_COM					_DS5W_ReturnValue::E_BT_COM
#define DS5W_E_IO_TIMEDOUT				_DS5W_ReturnValue::E_IO_TIMEDOUT
#define DS5W_E_IO_FAILED				_DS5W_ReturnValue::E_IO_FAILED
#define DS5W_E_IO_NOT_FOUND				_DS5W_ReturnValue::E_IO_NOT_FOUND
#define DS5W_E_IO_PENDING				_DS5W_ReturnValue::E_IO_PENDING

/// <summary>
/// Enum for error codes
/// </summary>
typedef enum class _DS5W_ReturnValue : unsigned int {
	/// <summary>
	/// Operation completed without an error
	/// </summary>
	OK = 0,

	/// <summary>
	/// Operation encountered an unknown error
	/// </summary>
	E_UNKNOWN = 1,

	/// <summary>
	/// The user supplied buffer is to small
	/// </summary>
	E_INSUFFICIENT_BUFFER = 2,

	/// <summary>
	/// External unexpected winapi error (please report as issue if you get this error!)
	/// </summary>
	E_EXTERNAL_WINAPI = 3,

	/// <summary>
	/// Not enought memory on the stack
	/// </summary>
	E_STACK_OVERFLOW = 4,

	/// <summary>
	/// Invalid arguments
	/// </summary>
	E_INVALID_ARGS = 5,

	/// <summary>
	/// This feature is currently not supported
	/// </summary>
	E_CURRENTLY_NOT_SUPPORTED = 6,

	/// <summary>
	/// Device was disconnected
	/// </summary>
	E_DEVICE_REMOVED = 7,

	/// <summary>
	/// Bluetooth communication error
	/// </summary>
	E_BT_COM = 8,

	/// <summary>
	/// IO timeout
	/// </summary>
	E_IO_TIMEDOUT = 9,

	/// <summary>
	/// IO failed
	/// </summary>
	E_IO_FAILED = 10,

	/// <summary>
	/// Overlapped IO request was not found
	/// </summary>
	E_IO_NOT_FOUND = 11,

	/// <summary>
	/// IO did not complete because it is running in the background
	/// </summary>
	E_IO_PENDING = 12

} DS5W_ReturnValue, DS5W_RV;

// DPAD buttons
#define DS5W_ISTATE_BTN_DPAD_LEFT 0x01
#define DS5W_ISTATE_BTN_DPAD_DOWN 0x02
#define DS5W_ISTATE_BTN_DPAD_RIGHT 0x04
#define DS5W_ISTATE_BTN_DPAD_UP 0x08

// Face buttons
#define DS5W_ISTATE_BTN_SQUARE 0x10
#define DS5W_ISTATE_BTN_CROSS 0x20
#define DS5W_ISTATE_BTN_CIRCLE 0x40
#define DS5W_ISTATE_BTN_TRIANGLE 0x80

// Shoulder buttons
#define DS5W_ISTATE_BTN_BUMPER_LEFT 0x0100
#define DS5W_ISTATE_BTN_BUMPER_RIGHT 0x0200
#define DS5W_ISTATE_BTN_TRIGGER_LEFT 0x0400
#define DS5W_ISTATE_BTN_TRIGGER_RIGHT 0x0800

// Menu buttons
#define DS5W_ISTATE_BTN_SELECT 0x1000
#define DS5W_ISTATE_BTN_MENU 0x2000

// Stick buttons
#define DS5W_ISTATE_BTN_STICK_LEFT 0x4000
#define DS5W_ISTATE_BTN_STICK_RIGHT 0x8000

// Extra buttons
#define DS5W_ISTATE_BTN_PLAYSTATION_LOGO 0x010000
#define DS5W_ISTATE_BTN_PAD_BUTTON 0x020000
#define DS5W_ISTATE_BTN_MIC_BUTTON 0x040000

#define DS5W_OSTATE_PLAYER_LED_LEFT 0x01
#define DS5W_OSTATE_PLAYER_LED_MIDDLE_LEFT 0x02
#define DS5W_OSTATE_PLAYER_LED_MIDDLE 0x04
#define DS5W_OSTATE_PLAYER_LED_MIDDLE_RIGHT 0x08
#define DS5W_OSTATE_PLAYER_LED_RIGHT 0x10

/// <summary>
/// Enum for return values
/// </summary>
typedef enum class _DS5W_ReturnValue : unsigned int {
	/// <summary>
	/// Operation completed without an error
	/// </summary>
	OK = 0,

	/// <summary>
	/// Operation encountered an unknown error
	/// </summary>
	E_UNKNOWN = 1,

	/// <summary>
	/// The user supplied buffer is to small
	/// </summary>
	E_INSUFFICIENT_BUFFER = 2,

	/// <summary>
	/// External unexpected winapi error (please report as issue if you get this error!)
	/// </summary>
	E_EXTERNAL_WINAPI = 3,

	/// <summary>
	/// Not enought memory on the stack
	/// </summary>
	E_STACK_OVERFLOW = 4,

	/// <summary>
	/// Invalid arguments
	/// </summary>
	E_INVALID_ARGS = 5,

	/// <summary>
	/// This feature is currently not supported
	/// </summary>
	E_CURRENTLY_NOT_SUPPORTED = 6,

	/// <summary>
	/// Device was disconnected
	/// </summary>
	E_DEVICE_REMOVED = 7,

	/// <summary>
	/// Bluetooth communication error
	/// </summary>
	E_BT_COM = 8,

	/// <summary>
	/// IO timeout
	/// </summary>
	E_IO_TIMEOUT = 9,

	/// <summary>
	/// IO fail
	/// </summary>
	E_IO_FAILED = 10,

} DS5W_ReturnValue, DS5W_RV;

namespace DS5W {
	/// <summary>
	/// Storage for calibration values used to parse raw motion data
	/// </summary>
	typedef struct _AxisCalibrationData {
		short bias;
		int sens_numer;
		int sens_denom;
	} AxisCalibrationData;

	typedef struct _DeviceCalibrationData {
		/// <summary>
			/// Values to calibrate controller's accelerometer and gyroscope
			/// </summary>
		AxisCalibrationData accelerometer[3];

		/// <summary>
		/// Values to calibrate controller's gyroscope
		/// </summary>
		AxisCalibrationData gyroscope[3];
	} DeviceCalibrationData;

	/// <summary>
	/// Enum for device connection type
	/// </summary>
	typedef enum class _DeviceConnection : unsigned char {
		/// <summary>
		/// Controler is connected via USB
		/// </summary>
		USB = 0,

		/// <summary>
		/// Controler is connected via bluetooth
		/// </summary>
		BT = 1,
	} DeviceConnection;

	/// <summary>
	/// Struckt for storing device enum info while device discovery
	/// </summary>
	typedef struct _DeviceEnumInfo {
		/// <summary>
		/// Encapsulate data in struct to (at least try) prevent user from modifing the context
		/// </summary>
		struct {
			/// <summary>
			/// Path to the discovered device
			/// </summary>
			wchar_t path[260];

			/// <summary>
			/// Connection type of the discoverd device
			/// </summary>
			DeviceConnection connection;

			/// <summary>
			/// Unique device identifier
			/// 32-bit hash of device interface's path
			/// </summary>
			UINT32 uniqueID;
		} _internal;
	} DeviceEnumInfo;

	/// <summary>
	/// Device context
	/// </summary>
	typedef struct _DeviceContext {
		/// <summary>
		/// Encapsulate data in struct to (at least try) prevent user from modifing the context
		/// </summary>
		struct {
			/// <summary>
			/// Path to the device
			/// </summary>
			wchar_t devicePath[260];

			/// <summary>
			/// Unique device identifier
			/// 32-bit hash of device interface's path
			/// </summary>
			UINT32 uniqueID;

			/// <summary>
			/// Handle to the open device
			/// </summary>
			HANDLE deviceHandle;

			/// <summary>
			/// Synchronization struct for async input
			/// </summary>
			OVERLAPPED olRead;

			/// <summary>
			/// Synchronization struct for async output
			/// </summary>
			OVERLAPPED olWrite;

			/// <summary>
			/// Connection of the device
			/// </summary>
			DeviceConnection connectionType;

			/// <summary>
			/// Collection of values required to parse controller's motion data
			/// </summary>
			DeviceCalibrationData calibrationData;

			/// <summary>
			/// Time when last input report was received, measured in 0.33 microseconds
			/// </summary>
			unsigned int timestamp;

			/// <summary>
			/// Current state of connection
			/// </summary>
			bool connected;

			/// <summary>
			/// HID Input buffer
			/// </summary>
			unsigned char hidInBuffer[DS_MAX_INPUT_REPORT_SIZE];

			/// <summary>
			/// HID Output buffer
			/// </summary>
			unsigned char hidOutBuffer[DS_MAX_OUTPUT_REPORT_SIZE];
		}_internal;
	} DeviceContext;

	/// <summary>
	/// Analog stick
	/// </summary>
	typedef struct _AnalogStick {
		/// <summary>
		/// X Position of stick (0 = Center)
		/// </summary>
		char x;

		/// <summary>
		/// Y Posistion of stick (0 = Center)
		/// </summary>
		char y;
	} AnalogStick;

	/// <summary>
	/// 3 Component vector
	/// </summary>
	typedef struct _Vec3 {
		int x;
		int y;
		int z;
	} Vector3, Vec3;

	/// <summary>
	/// RGB Color
	/// </summary>
	typedef struct _Color {
		unsigned char r;
		unsigned char g;
		unsigned char b;
	} Color;

	/// <summary>
	/// Touchpad state
	/// </summary>
	typedef struct _Touch {
		/// <summary>
		/// X positon of finger (~ 0 - 2000)
		/// </summary>
		unsigned int x;

		/// <summary>
		/// Y position of finger (~ 0 - 2048)
		/// </summary>
		unsigned int y;

		/// <summary>
		/// Touch is down
		/// </summary>
		bool down;

		/// <summary>
		/// 7-bit ID for touch
		/// </summary>
		unsigned char id;
	} Touch;

	typedef struct _Battery {
		/// <summary>
		/// Charching state of the battery
		/// </summary>
		bool charging;

		/// <summary>
		/// Indicates that the battery is fully charged
		/// </summary>
		bool fullyCharged;

		/// <summary>
		/// Battery charge level 0x0 to 
		/// </summary>
		unsigned char level;
	}Battery;

	/// <summary>
	/// State of the mic led
	/// </summary>
	typedef enum class _MicLed : unsigned char {
		/// <summary>
		/// Lef is off
		/// </summary>
		OFF = 0x00,

		/// <summary>
		/// Led is on
		/// </summary>
		ON = 0x01,

		/// <summary>
		/// Led is pulsing
		/// </summary>
		PULSE = 0x02,
	} MicLed;

	/// <summary>
	/// Type of trigger effect
	/// </summary>
	typedef enum class _TriggerEffectType : unsigned char {
		/// <summary>
		/// Disable all effects (after trigger is released)
		/// </summary>
		NoResitance = 0x00,

		/// <summary>
		/// Continuous Resitance is applied
		/// </summary>
		ContinuousResitance = 0x01,

		/// <summary>
		/// Seciton resistance is appleyed
		/// </summary>
		SectionResitance = 0x02,

		/// <summary>
		/// Disable all effects and release any active tension
		/// </summary>
		ReleaseAll = 0x05,

		/// <summary>
		/// Extended trigger effect
		/// </summary>
		EffectEx = 0x26,

		/// <summary>
		/// Calibrate triggers
		/// </summary>
		Calibrate = 0xFC,
	} TriggerEffectType;

	/// <summary>
	/// Trigger effect
	/// </summary>
	typedef struct _TriggerEffect {
		/// <summary>
		/// Trigger effect type
		/// </summary>
		TriggerEffectType effectType;

		/// <summary>
		/// Union for effect parameters
		/// </summary>
		union {
			/// <summary>
			/// Union one raw data
			/// </summary>
			unsigned char _u1_raw[10];

			/// <summary>
			/// For type == ContinuousResitance
			/// </summary>
			struct {
				/// <summary>
				/// Start position of resistance
				/// </summary>
				unsigned char startPosition;

				/// <summary>
				/// Force of resistance
				/// </summary>
				unsigned char force;

				/// <summary>
				/// PAD / UNUSED
				/// </summary>
				unsigned char _pad[8];
			} Continuous;

			/// <summary>
			/// For type == SectionResitance
			/// </summary>
			struct {
				/// <summary>
				/// Start position of resistance
				/// </summary>
				unsigned char startPosition;

				/// <summary>
				/// End position of resistance (>= start)
				/// </summary>
				unsigned char endPosition;

				/// <summary>
				/// PAD / UNUSED
				/// </summary>
				unsigned char _pad[8];
			} Section;

			/// <summary>
			/// For type == EffectEx
			/// </summary>
			struct {
				/// <summary>
				/// Position at witch the effect starts
				/// </summary>
				unsigned char startPosition;

				/// <summary>
				/// Wher the effect should keep playing when trigger goes beyond 255
				/// </summary>
				bool keepEffect;

				/// <summary>
				/// Force applied when trigger >= (255 / 2)
				/// </summary>
				unsigned char beginForce;

				/// <summary>
				/// Force applied when trigger <= (255 / 2)
				/// </summary>
				unsigned char middleForce;

				/// <summary>
				/// Force applied when trigger is beyond 255
				/// </summary>
				unsigned char endForce;

				/// <summary>
				/// Vibration frequency of the trigger
				/// </summary>
				unsigned char frequency;

				/// <summary>
				/// PAD / UNUSED
				/// </summary>
				unsigned char _pad[4];
			} EffectEx;
		};
	} TriggerEffect;

	/// <summary>
	/// Led brightness
	/// </summary>
	typedef enum _LedBrightness : unsigned char {
		/// <summary>
		/// Low led brightness
		/// </summary>
		LOW = 0x02,

		/// <summary>
		/// Medium led brightness
		/// </summary>
		MEDIUM = 0x01,

		/// <summary>
		/// High led brightness
		/// </summary>
		HIGH = 0x00,
	} LedBrightness;

	/// <summary>
	/// Player leds values
	/// </summary>
	typedef struct _PlayerLeds {
		/// <summary>
		/// Player indication leds bitflag (You may used them for other features) DS5W_OSTATE_PLAYER_LED_???
		/// </summary>
		unsigned char bitmask;

		/// <summary>
		/// Indicates weather the player leds should fade in
		/// </summary>
		bool playerLedFade;

		/// <summary>
		/// Brightness of the player leds
		/// </summary>
		LedBrightness brightness;
	} PlayerLeds;

	/// <summary>
	/// Input state of the controler
	/// </summary>
	typedef struct _DS5InputState {
		/// <summary>
		/// Position of left stick
		/// </summary>
		AnalogStick leftStick;

		/// <summary>
		/// Posisiton of right stick
		/// </summary>
		AnalogStick rightStick;

		/// <summary>
		/// bitflags of buttons, (face | btnsA | btnsB), final 13 bits are empty
		/// </summary>
		unsigned int buttonMap;

		/// <summary>
		/// Left trigger position
		/// </summary>
		unsigned char leftTrigger;

		/// <summary>
		/// Right trigger position
		/// </summary>
		unsigned char rightTrigger;

		/// <summary>
		/// Accelerometer
		/// </summary>
		Vector3 accelerometer;

		/// <summary>
		/// Gyroscope  (Currently only raw values will be dispayed! Probably needs calibration (Will be done within the lib in the future))
		/// </summary>
		Vector3 gyroscope;

		/// <summary>
		/// First touch point
		/// </summary>
		Touch touchPoint1;

		/// <summary>
		/// Second touch point
		/// </summary>
		Touch touchPoint2;

		/// <summary>
		/// Sensor timestamp in 0.33 microseconds
		/// </summary>
		unsigned int currentTime;

		/// <summary>
		/// Time since last input report. Measured in 0.33 microseconds
		/// </summary>
		unsigned int deltaTime;

		/// <summary>
		/// Battery information
		/// </summary>
		Battery battery;

		/// <summary>
		/// Indicates the connection of headphone
		/// </summary>
		bool headPhoneConnected;

		/// <summary>
		/// EXPERIMAENTAL: Feedback of the left adaptive trigger (only when trigger effect is active)
		/// </summary>
		unsigned char leftTriggerFeedback;

		/// <summary>
		/// EXPERIMAENTAL: Feedback of the right adaptive trigger (only when trigger effect is active)
		/// </summary>
		unsigned char rightTriggerFeedback;
	} DS5InputState;

	typedef struct _DS5OutputState {

		/// <summary>
		/// Left / Hard rumbel motor
		/// </summary>
		unsigned char leftRumble;

		/// <summary>
		/// Right / Soft rumbel motor
		/// </summary>
		unsigned char rightRumble;

		/// <summary>
		/// strength of rumble motors in 12.5% steps
		/// lower nibble (bits 0-3) main rumbles
		/// uppper nibble (bits 4-7) trigger rumbles
		/// </summary>
		unsigned char rumbleStrength;

		/// <summary>
		/// State of the microphone led
		/// </summary>
		MicLed microphoneLed;

		/// <summary>
		/// Diables all leds
		/// </summary>
		bool disableLeds;

		/// <summary>
		/// Player leds
		/// </summary>
		PlayerLeds playerLeds;

		/// <summary>
		/// Color of the lightbar
		/// </summary>
		Color lightbar;

		/// <summary>
		/// Effect of left trigger
		/// </summary>
		TriggerEffect leftTriggerEffect;

		/// <summary>
		/// Effect of right trigger
		/// </summary>
		TriggerEffect rightTriggerEffect;

	} DS5OutputState;

	/// <summary>
	/// Enumerate all ds5 deviced connected to the computer
	/// </summary>
	/// <param name="ptrBuffer">Pointer to begin of array of DeviceEnumInfo objects / DeviceEnumInfo pointers</param>
	/// <param name="inArrLength">Length of imput array</param>
	/// <param name="requiredLength"> pointer to uint witch recives the required total length</param>
	/// <param name="pointerToArray"> DeviceEnumInfo pointer is the pointer to an array of DeviceEnumInfo objects. false: DeviceEnumInfo pointer is a pointer to DeviceEnumInfo pointers to DeviceEnumInfo objects</param>
	/// <returns>DS5W Return value</returns>
	extern "C" DS5W_API DS5W_ReturnValue enumDevices(void* ptrBuffer, unsigned int inArrLength, unsigned int* requiredLength, bool pointerToArray = true);

	/// <summary>
	/// Enumerate all ds5 deviced that are not in the list of known devices
	/// Devices are 'known' if their unique ID is in the array passed to this function
	/// </summary>
	/// <param name="ptrBuffer">Pointer to begin of array of unused DeviceEnumInfo objects / DeviceEnumInfo pointers</param>
	/// <param name="inArrLength">Length of input array</param>
	/// <param name="knownDeviceIDs"> pointer to array of known device IDs</param>
	/// <param name="numKnownDevices"> length of knownDeviceIDs array</param>
	/// <param name="requiredLength"> pointer to uint witch recives the required total length</param>
	/// <param name="pointerToArray"> DeviceEnumInfo pointer is the pointer to an array of DeviceEnumInfo objects. false: DeviceEnumInfo pointer is a pointer to DeviceEnumInfo pointers to DeviceEnumInfo objects</param>
	/// <returns>DS5W Return value</returns>
	extern "C" DS5W_API DS5W_ReturnValue enumUnknownDevices(void* ptrBuffer, unsigned int inArrLength, unsigned int* knownDeviceIDs, unsigned int numKnownDevices, unsigned int* requiredLength, bool pointerToArray = true);

	/// <summary>
	/// Initializes a DeviceContext from its enum infos
	/// </summary>
	/// <param name="ptrEnumInfo">Pointer to enum object to create device from</param>
	/// <param name="ptrContext">Pointer to context to create to</param>
	/// <returns>If creation was successfull</returns>
	extern "C" DS5W_API DS5W_ReturnValue initDeviceContext(DS5W::DeviceEnumInfo* ptrEnumInfo, DS5W::DeviceContext* ptrContext);

	/// <summary>
	/// Stop device functions and free all links in Windows
	/// This context will not be able to be reconnected
	/// </summary>
	/// <param name="ptrContext">Pointer to context</param>
	extern "C" DS5W_API void freeDeviceContext(DS5W::DeviceContext* ptrContext);

	/// <summary>
	/// Stop device functions and disconnect device from windows
	/// This context is able to be reconnected
	/// </summary>
	/// <param name="ptrContext">Context to shutdown</param>
	extern "C" DS5W_API void shutdownDevice(DS5W::DeviceContext * ptrContext);

	/// <summary>
	/// Try to reconnect a disconnected device
	/// </summary>
	/// <param name="ptrContext">Context to reconnect on</param>
	/// <returns>Result</returns>
	extern "C" DS5W_API DS5W_ReturnValue reconnectDevice(DS5W::DeviceContext* ptrContext);

	/// <summary>
	/// Get device input state
	/// Blocks thread until state is read or an error occurs
	/// </summary>
	/// <param name="ptrContext">Pointer to context</param>
	/// <param name="ptrInputState">Pointer to input state</param>
	/// <returns>Result of call</returns>
	extern "C" DS5W_API DS5W_ReturnValue getDeviceInputState(DS5W::DeviceContext* ptrContext, DS5W::DS5InputState* ptrInputState);

	/// <summary>
	/// Set the device output state
	/// Blocks thread until state is read or an error occurs
	/// </summary>
	/// <param name="ptrContext">Pointer to context</param>
	/// <param name="ptrOutputState">Pointer to output state to be set</param>
	/// <returns>Result of call</returns>
	extern "C" DS5W_API DS5W_ReturnValue setDeviceOutputState(DS5W::DeviceContext* ptrContext, DS5W::DS5OutputState* ptrOutputState);

	/// <summary>
	/// Starts an overlapped IO call to get device input report
	/// </summary>
	extern "C" DS5W_API DS5W_ReturnValue startInputRequest(DS5W::DeviceContext* ptrContext);

	/// <summary>
	/// Waits until overlapped call finishes
	/// Only call this if startInputRequest() returned DS5W_E_IO_PENDING
	/// </summary>
	extern "C" DS5W_API DS5W_ReturnValue awaitInputRequest(DS5W::DeviceContext* ptrContext);

	/// <summary>
	/// Parses and copies the last input report read into an InputState struct
	/// Intended to be used with startInputRequest() after the request is completed
	/// </summary>
	extern "C" DS5W_API void getHeldInputState(DS5W::DeviceContext * ptrContext, DS5W::DS5InputState * ptrInputState);
}
