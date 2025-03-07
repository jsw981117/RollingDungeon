using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    public static event Action<float> OnHealthIncrease;
    public static event Action<float> OnStaminaIncrease;
    public static event Action<ItemData> OnItemPickup;
    public static event Action<ItemData> OnItemUse;


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

    public static void TriggerItemPickup(ItemData itemData)
    {
        OnItemPickup?.Invoke(itemData);
    }

    public static void TriggerItemUse(ItemData itemData)
    {
        if (itemData == null) return;
        OnItemUse?.Invoke(itemData);
    }
}
