using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f; // 초당 데미지
    [SerializeField] private string playerLayerName = "Player"; // 플레이어 레이어 이름
    [SerializeField] private StatHandler playerStatHandler;
    private bool isPlayerOnFloor = false;
    private bool isDamageCoroutineRunning = false; // 데미지 코루틴이 실행 중인지 여부

    private void OnCollisionEnter(Collision collision)
    {
        // 플레이어 레이어와 충돌했는지 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = true;

            // 데미지 코루틴이 실행 중이 아니면 시작
            if (!isDamageCoroutineRunning)
            {
                StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 플레이어 레이어와 충돌이 끝났는지 확인
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = false;
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        isDamageCoroutineRunning = true; // 코루틴 실행 중임을 표시

        while (isPlayerOnFloor)
        {
            if (playerStatHandler != null)
            {
                playerStatHandler.TakeDamage(damagePerSecond); // 데미지 적용
            }

            // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        isDamageCoroutineRunning = false; // 코루틴 종료
    }
}
