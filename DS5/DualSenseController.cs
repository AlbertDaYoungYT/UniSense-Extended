using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


// TODO: Create a playerID parameter
// TODO: Make null checks to insure it doesn't kill itself


// High-level wrapper for DualSense controller interaction
public class DualSenseController : IDisposable
{
    private DeviceContext _deviceContext;
    public bool IsConnected { get; private set; }
    public DeviceConnection ConnectionType { get; private set; }
    public string DevicePath { get; private set; }

    public uint UniqueID { get; private set; }
    public int PlayerID { get; private set; }

    public DS5InputState CurrentInputState { get; private set; }
    public DS5OutputState CurrentOutputState { get; private set; }

    public List<DS5_Effect> _effectQueue;
    private DS5_Effect _currentEffect;

    // Events for callbacks (using null-conditional operator for safe invocation)
    public event Action<DualSenseController, uint> OnDisconnect;
    public event Action<DualSenseController, DeviceConnection, uint> OnConnect;
    public event Action<DualSenseController, DS5W_ReturnValue> OnControllerError;

    public event Action<DualSenseController, DS5InputState> OnInputUpdate;
    public event Action<DualSenseController, DS5OutputState> OnOutputUpdate;

    // Default constructor (delegates to the one with playerID)
    public DualSenseController() : this(0) { }

    public DualSenseController(int playerID)
    {
        _deviceContext = new DeviceContext(true); // Initialize hidBuffer, calibrationData etc.
        CurrentInputState = new DS5InputState();
        CurrentOutputState = new DS5OutputState(true); // Initialize trigger effects
        IsConnected = false;
        PlayerID = playerID;
        _effectQueue = new List<DS5_Effect>(); // Initialize the list

        Debug.Log("DualSense Controller initialized.");
    }

    public static List<DeviceEnumInfo> GetConnectedDevices()
    {
        List<DeviceEnumInfo> devices = new List<DeviceEnumInfo>();
        uint requiredLength = 0;
        const int MAX_CONTROLLERS_GUESS = 8;
        DeviceEnumInfo[] infos = new DeviceEnumInfo[MAX_CONTROLLERS_GUESS];
        GCHandle gcHandle = GCHandle.Alloc(infos, GCHandleType.Pinned);
        IntPtr bufferPtr = gcHandle.AddrOfPinnedObject();

        DS5W_ReturnValue result;
        try
        {
            // First call to get the required buffer size
            result = DualSenseNative.enumDevices(bufferPtr, (uint)MAX_CONTROLLERS_GUESS, out requiredLength, true);

            if (result == DS5W_ReturnValue.E_INSUFFICIENT_BUFFER)
            {
                gcHandle.Free();
                infos = new DeviceEnumInfo[requiredLength];
                gcHandle = GCHandle.Alloc(infos, GCHandleType.Pinned);
                bufferPtr = gcHandle.AddrOfPinnedObject();
                result = DualSenseNative.enumDevices(bufferPtr, requiredLength, out requiredLength, true);
            }

            if (result == DS5W_ReturnValue.OK)
            {
                if (requiredLength == 0)
                {
                    Debug.LogWarning("No DualSense controllers found.");
                    return devices;
                }

                DeviceEnumInfo firstDeviceInfo = Marshal.PtrToStructure<DeviceEnumInfo>(bufferPtr); // This is a local variable

                DeviceContext tempDeviceContext; // Use a temporary variable for the out parameter
                result = DualSenseNative.initDeviceContext(ref firstDeviceInfo, out tempDeviceContext);
                if (result == DS5W_ReturnValue.OK) // Check if initialization was successful
                {
                    // This method is static, so it cannot set instance properties like IsConnected, ConnectionType, etc.
                    // It should only return the list of devices.
                    // The actual initialization of a specific controller instance happens in InitializeFirstController().
                    // For now, we just add the enumerated device info to the list.
                    devices.Add(firstDeviceInfo); // Assuming a conversion method or direct use
                }
                else
                {
                    Debug.LogError($"Failed to initialize device context: {result}");
                }
            }
            else
            {
                Debug.LogError($"Failed to enumerate devices: {result}");
            }
        }
        finally
        {
            if (gcHandle.IsAllocated)
            {
                gcHandle.Free();
            }
        }
        return devices;
    }

    // Attempts to find and initialize the first available DualSense controller.
    public bool InitializeFirstController()
    {
        uint requiredLength = 0;
        const int MAX_CONTROLLERS_GUESS = 8;
        DeviceEnumInfo[] infos = new DeviceEnumInfo[MAX_CONTROLLERS_GUESS];
        GCHandle gcHandle = GCHandle.Alloc(infos, GCHandleType.Pinned);
        IntPtr bufferPtr = gcHandle.AddrOfPinnedObject();

        DS5W_ReturnValue result;

        try
        {
            result = DualSenseNative.enumDevices(bufferPtr, (uint)MAX_CONTROLLERS_GUESS, out requiredLength, true);

            if (result == DS5W_ReturnValue.E_INSUFFICIENT_BUFFER)
            {
                gcHandle.Free();
                infos = new DeviceEnumInfo[requiredLength];
                gcHandle = GCHandle.Alloc(infos, GCHandleType.Pinned);
                bufferPtr = gcHandle.AddrOfPinnedObject();
                result = DualSenseNative.enumDevices(bufferPtr, requiredLength, out requiredLength, true);
            }

            if (result == DS5W_ReturnValue.OK)
            {
                if (requiredLength == 0)
                {
                    Debug.LogWarning("No DualSense controllers found.");
                    OnControllerError?.Invoke(this, result); // Safe invoke
                    return false;
                }

                DeviceEnumInfo firstDeviceInfo = Marshal.PtrToStructure<DeviceEnumInfo>(bufferPtr);

                result = DualSenseNative.initDeviceContext(ref firstDeviceInfo, out _deviceContext);
                if (result == DS5W_ReturnValue.OK)
                {
                    IsConnected = _deviceContext.connected;
                    ConnectionType = _deviceContext.connectionType;
                    DevicePath = _deviceContext.devicePath;
                    UniqueID = _deviceContext.uniqueID;

                    // Set initial player LED (1 << PlayerID) to light up the corresponding LED
                    DS5OutputState initialState = new DS5OutputState(true);
                    initialState.playerLeds.bitmask = (byte)(1 << PlayerID); // Corrected LED setting
                    SetOutputState(initialState);

                    Debug.Log($"DualSense Controller Initialized: {DevicePath}, Connection: {ConnectionType}, ID: {UniqueID}, Player: {PlayerID}");
                    OnConnect?.Invoke(this, ConnectionType, UniqueID); // Safe invoke
                    return true;
                }
                else
                {
                    Debug.LogError($"Failed to initialize device context: {result}");
                    OnControllerError?.Invoke(this, result); // Safe invoke
                }
            }
            else
            {
                Debug.LogError($"Failed to enumerate devices: {result}");
                OnControllerError?.Invoke(this, result); // Safe invoke
            }
        }
        finally
        {
            if (gcHandle.IsAllocated)
            {
                gcHandle.Free();
            }
        }

        IsConnected = false;
        return false;
    }

    public bool Reconnect()
    {
        if (!IsConnected)
        {
            DS5W_ReturnValue result = DualSenseNative.reconnectDevice(ref _deviceContext);
            if (result == DS5W_ReturnValue.OK)
            {
                IsConnected = _deviceContext.connected;
                Debug.Log("DualSense Controller reconnected.");
                OnConnect?.Invoke(this, _deviceContext.connectionType, _deviceContext.uniqueID); // Safe invoke
                return true;
            }
            else
            {
                Debug.LogError($"Failed to reconnect device: {result}");
                OnControllerError?.Invoke(this, result); // Safe invoke
                return false;
            }
        }
        return true;
    }

    public bool GetInputStateBlocking()
    {
        if (!IsConnected) return false;

        DS5InputState inputState;
        DS5W_ReturnValue result = DualSenseNative.getDeviceInputState(ref _deviceContext, out inputState);
        if (result == DS5W_ReturnValue.OK)
        {
            CurrentInputState = inputState;
            return true;
        }
        else if (result == DS5W_ReturnValue.E_DEVICE_REMOVED)
        {
            Debug.LogWarning("DualSense Controller disconnected.");
            OnDisconnect?.Invoke(this, UniqueID); // Safe invoke
            IsConnected = false;
            return false;
        }
        else
        {
            Debug.LogError($"Failed to get input state (blocking): {result}");
            OnControllerError?.Invoke(this, result); // Safe invoke
            return false;
        }
    }

    public bool StartInputRequest()
    {
        if (!IsConnected) return false;

        DS5W_ReturnValue result = DualSenseNative.startInputRequest(ref _deviceContext);
        if (result == DS5W_ReturnValue.OK || result == DS5W_ReturnValue.E_IO_PENDING)
        {
            return true;
        }
        else
        {
            Debug.LogError($"Failed to start input request: {result}");
            OnControllerError?.Invoke(this, result); // Safe invoke
            return false;
        }
    }

    public bool AwaitAndGetInputState()
    {
        if (!IsConnected) return false;

        DS5W_ReturnValue result = DualSenseNative.awaitInputRequest(ref _deviceContext);
        if (result == DS5W_ReturnValue.OK)
        {
            DS5InputState inputState;
            // Assuming getHeldInputState correctly populates inputState from _deviceContext.hidInBuffer
            DualSenseNative.getHeldInputState(ref _deviceContext, out inputState);
            CurrentInputState = inputState;
            return true;
        }
        else if (result == DS5W_ReturnValue.E_IO_TIMEDOUT)
        {
            // Debug.Log("Input request timed out. No new data."); // Often not an error, just no new data
            // OnControllerError?.Invoke(this, result); // Don't necessarily treat timeout as an error to invoke
            return false;
        }
        else if (result == DS5W_ReturnValue.E_DEVICE_REMOVED)
        {
            Debug.LogWarning("DualSense Controller disconnected during async input.");
            IsConnected = false;
            OnDisconnect?.Invoke(this, UniqueID); // Safe invoke
            return false;
        }
        else
        {
            Debug.LogError($"Failed to await input request: {result}");
            OnControllerError?.Invoke(this, result); // Safe invoke
            return false;
        }
    }

    public bool SetOutputState(DS5OutputState outputState)
    {
        if (!IsConnected) return false;

        DS5OutputState tempOutputState = outputState;
        DS5W_ReturnValue result = DualSenseNative.setDeviceOutputState(ref _deviceContext, ref tempOutputState);
        if (result == DS5W_ReturnValue.OK)
        {
            CurrentOutputState = tempOutputState;
            return true;
        }
        else if (result == DS5W_ReturnValue.E_DEVICE_REMOVED)
        {
            Debug.LogWarning("DualSense Controller disconnected during output set.");
            IsConnected = false;
            OnDisconnect?.Invoke(this, UniqueID); // Safe invoke
            return false;
        }
        else
        {
            Debug.LogError($"Failed to set output state: {result}");
            OnControllerError?.Invoke(this, result); // Safe invoke
            return false;
        }
    }

    public void Dispose()
    {
        if (_deviceContext.connected) // Only free if it was actually connected
        {
            DualSenseNative.freeDeviceContext(ref _deviceContext);
        }
        IsConnected = false;
        Debug.Log("DualSense Controller context freed.");
        GC.SuppressFinalize(this); // Prevent finalizer from running if Dispose is called manually
    }

    // Finalizer (destructor) to ensure Dispose is called if not done manually
    ~DualSenseController()
    {
        Dispose();
    }

    public void UpdateEffects()
    {
        if (_currentEffect != null)
        {
            _currentEffect.UpdateEffect(this);

            // Check if the effect has finished
            if (!_currentEffect.loop && _currentEffect.duration <= 0f)
            {
                _currentEffect.EndEffect(this);
                _currentEffect = null; // Clear current effect
            }
        }
        // If current effect is null, try to start the next one from the queue
        if (_currentEffect == null && _effectQueue != null && _effectQueue.Count > 0)
        {
            _currentEffect = _effectQueue[0];
            _effectQueue.RemoveAt(0);
            _currentEffect.StartEffect(this);
        }
    }

    public void AddEffect(DS5_Effect effect)
    {
        if (_effectQueue == null)
        {
            _effectQueue = new List<DS5_Effect>();
        }
        _effectQueue.Add(effect);
    }

    public void ClearEffects()
    {
        if (_currentEffect != null)
        {
            _currentEffect.EndEffect(this);
            _currentEffect = null;
        }
        if (_effectQueue != null)
        {
            _effectQueue.Clear();
        }
    }

    public void StopCurrentEffect()
    {
        if (_currentEffect != null)
        {
            _currentEffect.EndEffect(this);
            _currentEffect = null;
        }
    }
}