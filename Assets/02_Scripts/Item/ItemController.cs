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

        // ������ ó��
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

        // �ݶ��̴� ��Ȱ��ȭ
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        // ���� ��ġ�� �̵�
        float randomX = Random.Range(-itemData.spawnAreaXZ.x, itemData.spawnAreaXZ.x);
        float randomZ = Random.Range(-itemData.spawnAreaXZ.y, itemData.spawnAreaXZ.y);
        transform.position = new Vector3(randomX, itemData.respawnHeight, randomZ);

        // ���
        yield return new WaitForSeconds(itemData.respawnDelay);

        // �ݶ��̴� Ȱ��ȭ
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

    // IItem�� �ʿ��� �߰� �޼����
    public string GetDescription()
    {
        return itemData != null ? itemData.description : "";
    }

    public Sprite GetIcon()
    {
        return itemData != null ? itemData.icon : null;
    }
}
