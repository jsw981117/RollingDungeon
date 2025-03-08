using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Data", menuName = "Game/Items/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    public string itemName = "Item";
    public string description = "An item description";
    public Sprite icon;

    [Header("Effect")]
    public bool affectsScore = false;
    public int scoreValue = 0;

    public bool affectsHealth = false;
    public float healthValue = 0f;

    public bool affectsStamina = false;
    public float staminaValue = 0f;

    [Header("Inventory Settings")]
    public bool isEquippable = false;  // 장착 가능한 아이템인지

    [Header("Respawn")]
    public bool shouldRespawn = true;

    [Header("Sfx")]
    public AudioClip collectSound;
}
