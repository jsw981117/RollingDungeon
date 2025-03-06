using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour, IItem
{
    public int scoreValue = 10;

    public void Use()
    {
        // ���� �Ŵ����� ���� ���� ����
        // �̺�Ʈ�� �߻����Ѽ� ���� ���� ó��
        PlayerEvent.TriggerScoreIncrease(scoreValue);

        Debug.Log($"Star item used! {scoreValue} points added.");
        Destroy(gameObject);  // ������ ��� �� ����
    }

    public string GetItemName()
    {
        return "Star";
    }
}
