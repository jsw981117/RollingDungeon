using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private ItemData storedItem;
    [SerializeField] private Image itemIcon;

    /// <summary>
    /// 인벤토리 활성화 시 이벤트 구독
    /// </summary>
    private void OnEnable()
    {
        PlayerEvent.OnItemPickup += StoreItem;
    }

    /// <summary>
    /// 인벤토리 비활성화 시 이벤트 구독 해제
    /// </summary>
    private void OnDisable()
    {
        PlayerEvent.OnItemPickup -= StoreItem;
    }

    /// <summary>
    /// 아이템을 인벤토리에 저장하고 아이콘 업데이트
    /// </summary>
    /// <param name="itemData">저장할 아이템 데이터</param>
    public void StoreItem(ItemData itemData)
    {
        storedItem = itemData;
        itemIcon.sprite = itemData.icon;
        itemIcon.gameObject.SetActive(true); // 아이콘 표시
    }

    /// <summary>
    /// 인벤토리에 저장된 아이템을 사용하고 인벤토리를 비우기
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
