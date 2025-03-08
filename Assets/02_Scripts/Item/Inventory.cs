using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private ItemData storedItem;
    [SerializeField] private Image itemIcon;

    private void OnEnable()
    {
        PlayerEvent.OnItemPickup += StoreItem;
    }

    private void OnDisable()
    {
        PlayerEvent.OnItemPickup -= StoreItem;
    }

    public void StoreItem(ItemData itemData)
    {
        storedItem = itemData;
        itemIcon.sprite = itemData.icon;
        itemIcon.gameObject.SetActive(true); // ������ ǥ��
    }

    public void UseStoredItem()
    {
        if (storedItem == null) return;

        // �κ��丮 ������ ��� ���� �̺�Ʈ �߻� (�� �����۰� ����)
        PlayerEvent.TriggerInventoryItemUse(storedItem);

        // ������ ȿ�� ������ ���� �̺�Ʈ
        if (storedItem.isEquippable)
        {
            // IItem �������̽��� ������ �ӽ� ��ü �����Ͽ� ���
            GameObject tempObject = new GameObject("TempItemForUse");
            ItemController tempItem = tempObject.AddComponent<ItemController>();
            tempItem.SetItemData(storedItem);
            tempItem.Use();
            Destroy(tempObject);
        }

        // �κ��丮 ����
        storedItem = null;
        itemIcon.gameObject.SetActive(false);
    }
}
