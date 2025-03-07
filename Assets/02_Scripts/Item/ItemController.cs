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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            Use();
        }
    }

    public void Use()
    {
        if (isRespawning || itemData == null) return;

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

        // 리스폰 처리
        if (itemData.shouldRespawn)
        {
            StartCoroutine(ResetItemPosition());
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

    private IEnumerator ResetItemPosition()
    {
        isRespawning = true;

        // 콜라이더 비활성화
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        // 랜덤 위치로 이동
        float randomX = Random.Range(-itemData.spawnAreaXZ.x, itemData.spawnAreaXZ.x);
        float randomZ = Random.Range(-itemData.spawnAreaXZ.y, itemData.spawnAreaXZ.y);
        transform.position = new Vector3(randomX, itemData.respawnHeight, randomZ);

        // 대기
        yield return new WaitForSeconds(itemData.respawnDelay);

        // 콜라이더 활성화
        if (itemCollider != null)
        {
            itemCollider.enabled = true;
        }

        isRespawning = false;
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
}
