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

    [Header("Settings")]
    public bool shouldRespawn = true;
    public float respawnDelay = 1f;
    public float respawnHeight = 18f;
    public Vector2 spawnAreaXZ = new Vector2(20f, 20f);

    [Header("Sfx")]
    public AudioClip collectSound;
}
