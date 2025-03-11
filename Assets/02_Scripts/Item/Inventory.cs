using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private ItemData storedItem;
    [SerializeField] private Image itemIcon;

    /// <summary>
    /// �κ��丮 Ȱ��ȭ �� �̺�Ʈ ����
    /// </summary>
    private void OnEnable()
    {
        PlayerEvent.OnItemPickup += StoreItem;
    }

    /// <summary>
    /// �κ��丮 ��Ȱ��ȭ �� �̺�Ʈ ���� ����
    /// </summary>
    private void OnDisable()
    {
        PlayerEvent.OnItemPickup -= StoreItem;
    }

    /// <summary>
    /// �������� �κ��丮�� �����ϰ� ������ ������Ʈ
    /// </summary>
    /// <param name="itemData">������ ������ ������</param>
    public void StoreItem(ItemData itemData)
    {
        storedItem = itemData;
        itemIcon.sprite = itemData.icon;
        itemIcon.gameObject.SetActive(true); // ������ ǥ��
    }

    /// <summary>
    /// �κ��丮�� ����� �������� ����ϰ� �κ��丮�� ����
    /// </summary>
    public void UseStoredItem()
    {
        if (storedItem == null) return;

        PlayerEvent.TriggerInventoryItemUse(storedItem);

        if (storedItem.isEquippable)
        {
            GameObject tempObject = new GameObject("TempItemForUse");
            ItemController tempItem = tempObject.AddComponent<ItemController>();
            tempItem.SetItemData(storedItem);
            tempItem.Use();
            Destroy(tempObject);
        }

        storedItem = null;
        itemIcon.gameObject.SetActive(false);
    }
}
