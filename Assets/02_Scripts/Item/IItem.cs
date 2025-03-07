using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{

    void Use();  // ������ ���
    string GetItemName();  // ������ �̸� ��ȯ
    string GetDescription();  // ������ ���� ��ȯ
    Sprite GetIcon();  // ������ ������ ��ȯ
}
