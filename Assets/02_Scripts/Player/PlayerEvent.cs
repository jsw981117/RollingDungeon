using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public delegate void ScoreEvent(int value);
    public static event ScoreEvent OnScoreIncrease;

    public delegate void HealthEvent(int value);
    public static event HealthEvent OnHealthIncrease;

    public delegate void StaminaEvent(int value);
    public static event StaminaEvent OnStaminaIncrease;

    public static void TriggerScoreIncrease(int value)
    {
        OnScoreIncrease?.Invoke(value);
    }

    public static void TriggerHealthIncrease(int value)
    {
        OnHealthIncrease?.Invoke(value);
    }

    public static void TriggerStaminaIncrease(int value)
    {
        OnStaminaIncrease?.Invoke(value);
    }
}
