using System;
using System.Runtime.InteropServices;
using UnityEngine;

// Static class for P/Invoke declarations (updated signatures and new functions)
public static class DualSenseNative
{
    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern DS5W_ReturnValue enumDevices(
        IntPtr ptrBuffer,
        uint inArrLength,
        out uint requiredLength,
        [MarshalAs(UnmanagedType.U1)] bool pointerToArray); // pointerToArray is now explicit

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern DS5W_ReturnValue enumUnknownDevices(
        IntPtr ptrBuffer,
        uint inArrLength,
        [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] uint[] knownDeviceIDs, // Array of uint
        uint numKnownDevices,
        out uint requiredLength,
        [MarshalAs(UnmanagedType.U1)] bool pointerToArray);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
    public static extern DS5W_ReturnValue initDeviceContext(
        ref DeviceEnumInfo ptrEnumInfo,
        out DeviceContext ptrContext);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void freeDeviceContext(
        ref DeviceContext ptrContext);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void shutdownDevice(
        ref DeviceContext ptrContext); // New function

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern DS5W_ReturnValue reconnectDevice(
        ref DeviceContext ptrContext);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern DS5W_ReturnValue getDeviceInputState(
        ref DeviceContext ptrContext,
        out DS5InputState ptrInputState);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern DS5W_ReturnValue setDeviceOutputState(
        ref DeviceContext ptrContext,
        ref DS5OutputState ptrOutputState);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern DS5W_ReturnValue startInputRequest(
        ref DeviceContext ptrContext);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern DS5W_ReturnValue awaitInputRequest(
        ref DeviceContext ptrContext);

    [DllImport(DualSenseConstants.DllName, CallingConvention = CallingConvention.Cdecl)]
    public static extern void getHeldInputState(
        ref DeviceContext ptrContext,
        out DS5InputState ptrInputState);
}