using System;
using UnityEngine;


public class RumbleEffect : DS5_Effect
{
    private byte _lowFreq;
    private byte _highFreq;

    public RumbleEffect(byte lowFrequency, byte highFrequency, float duration = 0f, bool loop = false)
    {
        _lowFreq = Math.Clamp(lowFrequency, (byte)0, (byte)255); // Clamp to 0-255
        _highFreq = Math.Clamp(highFrequency, (byte)0, (byte)255);
        this.duration = duration;
        this.loop = loop;
    }

    public override void StartEffect(DualSenseController controller)
    {
        Debug.Log($"Starting Rumble Effect: Low={_lowFreq}, High={_highFreq}");
        DS5OutputState outputState = new DS5OutputState(true);
        outputState.leftRumble = _lowFreq;
        outputState.rightRumble = _highFreq;
        controller.SetOutputState(outputState); // Set rumble state
    }

    public override void UpdateEffect(DualSenseController controller)
    {
        if (!loop)
        {
            duration -= Time.deltaTime;
        }
        // For continuous rumble, no need to call SetRumble every frame unless values change.
        // If you want pulsating or varying rumble, you'd modify _lowFreq/_highFreq here
        // and call controller.SetRumble again.
    }

    public override void EndEffect(DualSenseController controller)
    {
        Debug.Log("Ending Rumble Effect");
        DS5OutputState outputState = new DS5OutputState(true);
        outputState.leftRumble = 0;
        outputState.rightRumble = 0;
        controller.SetOutputState(outputState); // Set rumble state to zero
    }
}