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
        // 인벤토리 사용 이벤트만 구독 (맵에서의 사용과 구분)
        PlayerEvent.OnInventoryItemUse += HandleInventoryItemUse;
    }

    void OnDisable()
    {
        // 이벤트 구독 해제
        PlayerEvent.OnInventoryItemUse -= HandleInventoryItemUse;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            if (itemData.isEquippable)
            {
                // 이벤트로 인벤토리에 저장
                PlayerEvent.TriggerItemPickup(itemData);

                // 맵에서 제거
                RequestRespawn();
            }
            else
            {
                // 즉시 사용 (비장착 아이템)
                UseDirectly();
            }
        }
    }

    // 인벤토리에서 사용된 아이템 처리
    private void HandleInventoryItemUse(ItemData usedItemData)
    {
        // 아무 처리도 하지 않음 - 맵에 있는 아이템은 인벤토리 사용에 영향받지 않음
        // 같은 종류의 아이템이라도 별도로 관리됨
    }

    // 맵에서 직접 사용 시 호출(비장착 아이템)
    public void UseDirectly()
    {
        // 아이템 효과 적용
        ApplyItemEffects();

        // 효과음 재생
        PlayCollectSound();

        // 맵에서 아이템 제거 및 리스폰 요청
        RequestRespawn();
    }

    // IItem 인터페이스 구현 - 인벤토리에서 사용 시 호출됨
    public void Use()
    {
        // 아이템 효과 적용
        ApplyItemEffects();

        // 인벤토리 아이템은 맵에 영향을 주지 않으므로 리스폰 요청 하지 않음
    }

    // 아이템 효과 적용 메서드 (공통 로직)
    private void ApplyItemEffects()
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
    }

    private void RequestRespawn()
    {
        // 맵에서 아이템 제거 및 리스폰 요청
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
