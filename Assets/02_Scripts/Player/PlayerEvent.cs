using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public static event Action<float> OnHealthIncrease;

    public static event Action<float> OnStaminaIncrease;


    public static void TriggerHealthIncrease(float value)
    {
        if (value <= 0) return;
        OnHealthIncrease?.Invoke(value);
    }

    public static void TriggerStaminaIncrease(float value)
    {
        if (value <= 0) return;
        OnStaminaIncrease?.Invoke(value);
    }
}
