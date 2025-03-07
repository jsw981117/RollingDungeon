using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour, IItem
{
    [SerializeField] protected ItemData itemData;

    private BoxCollider itemCollider;
    private int playerLayer;
    private bool isRespawning = false;
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
    }

    void OnEnable()
    {
        PlayerEvent.OnItemUse += HandleItemUse;
    }

    void OnDisable()
    {
        PlayerEvent.OnItemUse -= HandleItemUse;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (itemData.isEquippable)
            {
                // �̺�Ʈ�� �κ��丮�� ����
                PlayerEvent.TriggerItemPickup(itemData);

                // ������ �����ʿ� ������ ��û
                if (itemData.shouldRespawn && ItemSpawner.Instance != null)
                {
                    ItemSpawner.Instance.QueueItemRespawn(gameObject, itemData);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else
            {
                // ��� ��� (������ ������)
                Use();
            }
        }
    }

    private void HandleItemUse(ItemData usedItemData)
    {
        // ���� ������ ���������� Ȯ��
        if (usedItemData == itemData)
        {
            // ������ ��� ȿ�� ����
            Use();
        }
    }

    public void Use()
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

        // ȿ���� ���
        PlayCollectSound();

        // ������ �����ʿ� ������ ��û
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

    // IItem�� �ʿ��� �߰� �޼����
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
