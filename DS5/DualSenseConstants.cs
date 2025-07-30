using System;
using System.Runtime.InteropServices;
using Unity.AppUI.Core;
using UnityEngine;

static class DualSenseConstants
{
    public const string DllName = "ds5w_x64.dll"; // Ensure this matches your DLL file name

    // Updated Return Values from DS5W v0.3 header
    public const uint DS5W_OK = 0;
    public const uint DS5W_E_UNKNOWN = 1;
    public const uint DS5W_E_INSUFFICIENT_BUFFER = 2;
    public const uint DS5W_E_EXTERNAL_WINAPI = 3;
    public const uint DS5W_E_STACK_OVERFLOW = 4;
    public const uint DS5W_E_INVALID_ARGS = 5;
    public const uint DS5W_E_CURRENTLY_NOT_SUPPORTED = 6;
    public const uint DS5W_E_DEVICE_REMOVED = 7;
    public const uint DS5W_E_BT_COM = 8;
    public const uint DS5W_E_IO_TIMEDOUT = 9;
    public const uint DS5W_E_IO_FAILED = 10;
    public const uint DS5W_E_IO_NOT_FOUND = 11;
    public const uint DS5W_E_IO_PENDING = 12;

    // Updated Button Constants (now combined into a single unsigned int buttonMap)
    public const uint DS5W_ISTATE_BTN_DPAD_LEFT = 0x01;
    public const uint DS5W_ISTATE_BTN_DPAD_DOWN = 0x02;
    public const uint DS5W_ISTATE_BTN_DPAD_RIGHT = 0x04;
    public const uint DS5W_ISTATE_BTN_DPAD_UP = 0x08;

    public const uint DS5W_ISTATE_BTN_SQUARE = 0x10;
    public const uint DS5W_ISTATE_BTN_CROSS = 0x20;
    public const uint DS5W_ISTATE_BTN_CIRCLE = 0x40;
    public const uint DS5W_ISTATE_BTN_TRIANGLE = 0x80;

    public const uint DS5W_ISTATE_BTN_BUMPER_LEFT = 0x0100;
    public const uint DS5W_ISTATE_BTN_BUMPER_RIGHT = 0x0200;
    public const uint DS5W_ISTATE_BTN_TRIGGER_LEFT = 0x0400;
    public const uint DS5W_ISTATE_BTN_TRIGGER_RIGHT = 0x0800;

    public const uint DS5W_ISTATE_BTN_SELECT = 0x1000; // Share button
    public const uint DS5W_ISTATE_BTN_MENU = 0x2000;   // Options button

    public const uint DS5W_ISTATE_BTN_STICK_LEFT = 0x4000;
    public const uint DS5W_ISTATE_BTN_STICK_RIGHT = 0x8000;

    public const uint DS5W_ISTATE_BTN_PLAYSTATION_LOGO = 0x010000;
    public const uint DS5W_ISTATE_BTN_PAD_BUTTON = 0x020000; // Touchpad click
    public const uint DS5W_ISTATE_BTN_MIC_BUTTON = 0x040000;

    // Output State Player LED Flags (unchanged)
    public const byte DS5W_OSTATE_PLAYER_LED_LEFT = 0x01;
    public const byte DS5W_OSTATE_PLAYER_LED_MIDDLE_LEFT = 0x02;
    public const byte DS5W_OSTATE_PLAYER_LED_MIDDLE = 0x04;
    public const byte DS5W_OSTATE_PLAYER_LED_MIDDLE_RIGHT = 0x08;
    public const byte DS5W_OSTATE_PLAYER_LED_RIGHT = 0x10;

    // Assumed max report sizes (not in header, but typical for DualSense USB)
    public const int DS_MAX_INPUT_REPORT_SIZE = 68;
    public const int DS_MAX_OUTPUT_REPORT_SIZE = 48; // Excluding CRC
}