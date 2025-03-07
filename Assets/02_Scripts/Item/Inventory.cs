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

        // 아이템 사용 이벤트 발생
        PlayerEvent.TriggerItemUse(storedItem);

        // 인벤토리 비우기
        storedItem = null;
        itemIcon.gameObject.SetActive(false);
    }
}
