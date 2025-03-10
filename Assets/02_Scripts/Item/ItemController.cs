using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItem
{
    [SerializeField] protected ItemData itemData;

    private BoxCollider itemCollider;
    private int playerLayer;
    private AudioSource audioSource;

    void Start()
    {
        if (itemData == null)
        {
            Debug.LogError($"{gameObject.name}: ItemData is not assigned!");
            return;
        }

        playerLayer = LayerMask.NameToLayer("Player");
        itemCollider = GetComponent<BoxCollider>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null && itemData.collectSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        transform.rotation = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);
    }

    void OnEnable()
    {
        // �κ��丮 ��� �̺�Ʈ�� ���� (�ʿ����� ���� ����)
        PlayerEvent.OnInventoryItemUse += HandleInventoryItemUse;
    }

    void OnDisable()
    {
        // �̺�Ʈ ���� ����
        PlayerEvent.OnInventoryItemUse -= HandleInventoryItemUse;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (itemData.isEquippable)
            {
                // �̺�Ʈ�� �κ��丮�� ����
                PlayerEvent.TriggerItemPickup(itemData);

                // �ʿ��� ����
                RequestRespawn();
            }
            else
            {
                // ��� ��� (������ ������)
                UseDirectly();
            }
        }
    }

    // �κ��丮���� ���� ������ ó��
    private void HandleInventoryItemUse(ItemData usedItemData)
    {
        // �ƹ� ó���� ���� ���� - �ʿ� �ִ� �������� �κ��丮 ��뿡 ������� ����
        // ���� ������ �������̶� ������ ������
    }

    // �ʿ��� ���� ��� �� ȣ��(������ ������)
    public void UseDirectly()
    {
        // ������ ȿ�� ����
        ApplyItemEffects();

        // ȿ���� ���
        PlayCollectSound();

        // �ʿ��� ������ ���� �� ������ ��û
        RequestRespawn();
    }

    // IItem �������̽� ���� - �κ��丮���� ��� �� ȣ���
    public void Use()
    {
        // ������ ȿ�� ����
        ApplyItemEffects();

        // �κ��丮 �������� �ʿ� ������ ���� �����Ƿ� ������ ��û ���� ����
    }

    // ������ ȿ�� ���� �޼��� (���� ����)
    private void ApplyItemEffects()
    {
        // ���� ó��
        if (itemData.affectsScore && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(itemData.scoreValue);
        }

        // ü�� ó��
        if (itemData.affectsHealth)
        {
            PlayerEvent.TriggerHealthIncrease(itemData.healthValue);
        }

        // ���¹̳� ó��
        if (itemData.affectsStamina)
        {
            PlayerEvent.TriggerStaminaIncrease(itemData.staminaValue);
        }
    }

    private void RequestRespawn()
    {
        // �ʿ��� ������ ���� �� ������ ��û
        if (itemData.shouldRespawn && ItemSpawner.Instance != null)
        {
            ItemSpawner.Instance.QueueItemRespawn(gameObject, itemData);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void PlayCollectSound()
    {
        if (audioSource != null && itemData.collectSound != null)
        {
            audioSource.clip = itemData.collectSound;
            audioSource.Play();
        }
    }

    public string GetItemName()
    {
        return itemData != null ? itemData.itemName : "Unknown Item";
    }

    public string GetDescription()
    {
        return itemData != null ? itemData.description : "";
    }

    public Sprite GetIcon()
    {
        return itemData != null ? itemData.icon : null;
    }

    public void SetItemData(ItemData newItemData)
    {
        itemData = newItemData;
    }
}
