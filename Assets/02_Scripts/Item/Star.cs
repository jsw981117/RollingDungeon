using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour, IItem
{
    public int scoreValue = 10;

    public void Use()
    {
        // 게임 매니저의 현재 점수 증가
        // 이벤트를 발생시켜서 점수 증가 처리
        PlayerEvent.TriggerScoreIncrease(scoreValue);

        Debug.Log($"Star item used! {scoreValue} points added.");
        Destroy(gameObject);  // 아이템 사용 후 제거
    }

    public string GetItemName()
    {
        return "Star";
    }
}
