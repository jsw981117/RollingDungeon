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
        itemIcon.gameObject.SetActive(true); // 아이콘 표시
    }

    public void UseStoredItem()
    {
        if (storedItem == null) return;

        // 인벤토리 아이템 사용 전용 이벤트 발생 (맵 아이템과 구분)
        PlayerEvent.TriggerInventoryItemUse(storedItem);

        // 아이템 효과 적용을 위한 이벤트
        if (storedItem.isEquippable)
        {
            // IItem 인터페이스가 구현된 임시 객체 생성하여 사용
            GameObject tempObject = new GameObject("TempItemForUse");
            ItemController tempItem = tempObject.AddComponent<ItemController>();
            tempItem.SetItemData(storedItem);
            tempItem.Use();
            Destroy(tempObject);
        }

        // 인벤토리 비우기
        storedItem = null;
        itemIcon.gameObject.SetActive(false);
    }
}
