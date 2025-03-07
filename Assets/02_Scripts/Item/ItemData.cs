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
    public bool isCollectible = true;  // 인벤토리에 수납 가능한 아이템인지
    public bool isPersistentAfterUse = false;  // 사용 후에도 아이템이 남아있는지
    public bool isEquippable = false;  // 장착 가능한 아이템인지

    [Header("Respawn")]
    public bool shouldRespawn = true;
    public float respawnDelay = 1f;
    public float respawnHeight = 18f;
    public Vector2 spawnAreaXZ = new Vector2(20f, 20f);
    public bool isStarItem = false;  // Star 아이템(항상 리스폰됨)
    public float spawnProbability = 100f;  // 리스폰 시 등장 확률 (0-100)

    [Header("Sfx")]
    public AudioClip collectSound;
}
