UniSense-Extended: DualSense plugin for Unity InputSystem
====

This plugin enables Unity apps to fully utilize the DualSense 5 controller. This is currently limited to Windows only as far as i've tested it.

But i have plans on trying to get a working version for Linux, and having the DualSense Edge controller mapped aswell.


This project is incomplete state for now. For example:
- the documentation including this README is work in progress.
- The code is yet to be optimized.


## Roadmap

[ ] Getting stable support for all DualSense Inputs.
[ ] Getting stable support for sending data to the DualSense.
[ ] Getting stable support for Linux.
[ ] Working on more complex communication with the controller (eg. Audio and Hardware specific data)


## Current Features

- All buttons and analog sticks
- Gyroscope
- Accelerometer
- Player LED
- Lightbar
- Adaptive Triggers (Basic support)
- Rumble (Basic support)
- Multitouch Touchpad (Basic support)

## Requirements

- Unity 2019.1 or above (Tested on Unity 6.1)
- Input System [v1.0.1](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/changelog/CHANGELOG.html#101---2020-11-20) or above

## Installation

You can install this package using Unity Package Manager (UPM).

1. On Unity Editor, open UPM window by going to Window > Package Manager
2. In this window top left corner, click on the **+** button and select **Add package from git URL...**
3. Enter this url ```https://github.com/AlbertDaYoungYT/UniSense-Extended.git```


## Special thanks

- [nullkal/UniSense](https://github.com/nullkal/UniSense) (Original Repository)
- [Ohjurot/DualSense-Windows](https://github.com/Ohjurot/DualSense-Windows)
- [DualSense Wiki](https://controllers.fandom.com/wiki/Sony_DualSense)