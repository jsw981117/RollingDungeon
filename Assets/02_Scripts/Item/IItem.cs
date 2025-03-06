using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem
{
    void Use();  // 아이템 사용
    string GetItemName();  // 아이템 이름 반환
}
