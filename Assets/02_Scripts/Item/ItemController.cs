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
        PlayerEvent.OnInventoryItemUse += HandleInventoryItemUse;
    }

    void OnDisable()
    {
        PlayerEvent.OnInventoryItemUse -= HandleInventoryItemUse;
    }

    /// <summary>
    /// �÷��̾�� �浹 �� ������ ��� �Ǵ� �κ��丮�� ����
    /// </summary>
    /// <param name="other">�浹�� �ݶ��̴�</param>
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

    /// <summary>
    /// �κ��丮���� ���� ������ ó��
    /// </summary>
    /// <param name="usedItemData">���� ������ ������</param>
    private void HandleInventoryItemUse(ItemData usedItemData)
    {
        // �ƹ� ó���� ���� ���� - �ʿ� �ִ� �������� �κ��丮 ��뿡 ������� ����
        // ���� ������ �������̶� ������ ������
    }

    /// <summary>
    /// �ʿ��� ���� ��� �� ȣ��(������ ������)
    /// </summary>
    public void UseDirectly()
    {
        ApplyItemEffects();
        PlayCollectSound();
        RequestRespawn();
    }

    /// <summary>
    /// IItem �������̽� ���� - �κ��丮���� ��� �� ȣ��
    /// </summary>
    public void Use()
    {
        ApplyItemEffects();
    }

    /// <summary>
    /// ������ ȿ�� ���� �޼���
    /// </summary>
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

    /// <summary>
    /// �ʿ��� ������ ���� �� ������ ��û
    /// </summary>
    private void RequestRespawn()
    {
        if (itemData.shouldRespawn && ItemSpawner.Instance != null)
        {
            ItemSpawner.Instance.QueueItemRespawn(gameObject, itemData);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ������ ���� ���� ���
    /// </summary>
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
