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
                // 이벤트로 인벤토리에 저장
                PlayerEvent.TriggerItemPickup(itemData);

                // 아이템 스포너에 리스폰 요청
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
                // 즉시 사용 (비장착 아이템)
                Use();
            }
        }
    }

    private void HandleItemUse(ItemData usedItemData)
    {
        // 같은 종류의 아이템인지 확인
        if (usedItemData == itemData)
        {
            // 아이템 사용 효과 적용
            Use();
        }
    }

    public void Use()
    {
        // 점수 처리
        if (itemData.affectsScore && GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(itemData.scoreValue);
        }

        // 체력 처리
        if (itemData.affectsHealth)
        {
            PlayerEvent.TriggerHealthIncrease(itemData.healthValue);
        }

        // 스태미나 처리
        if (itemData.affectsStamina)
        {
            PlayerEvent.TriggerStaminaIncrease(itemData.staminaValue);
        }

        // 효과음 재생
        PlayCollectSound();

        // 아이템 스포너에 리스폰 요청
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

    // IItem에 필요한 추가 메서드들
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
