using UnityEngine;
using System;
using System.Runtime.InteropServices;

public abstract class DS5_Effect : ScriptableObject
{
    public DS5OutputState previousState;
    public string effectName;
    public bool loop = false;
    public float duration; // Duration in seconds

    public abstract void StartEffect(DualSenseController controller);
    public virtual void UpdateEffect(DualSenseController controller) { }
    public virtual void EndEffect(DualSenseController controller) { }
}