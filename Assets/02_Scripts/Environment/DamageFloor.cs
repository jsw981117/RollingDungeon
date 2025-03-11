using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f;
    [SerializeField] private string playerLayerName = "Player";
    [SerializeField] private StatHandler playerStatHandler;
    private bool isPlayerOnFloor = false;
    private bool isDamageCoroutineRunning = false;

    /// <summary>
    /// 플레이어가 데미지 플로어와 충돌할 때 데미지를 적용합니다.
    /// </summary>
    /// <param name="collision">충돌 정보</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = true;

            if (!isDamageCoroutineRunning)
            {
                StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    /// <summary>
    /// 플레이어가 데미지 플로어에서 벗어날 때 데미지 적용을 중지합니다.
    /// </summary>
    /// <param name="collision">충돌 정보</param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = false;
        }
    }

    /// <summary>
    /// 플레이어가 데미지 플로어 위에 있는 동안 지속적으로 데미지를 적용합니다.
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator ApplyDamageOverTime()
    {
        isDamageCoroutineRunning = true;

        while (isPlayerOnFloor)
        {
            if (playerStatHandler != null)
            {
                playerStatHandler.TakeDamage(damagePerSecond);
            }

            yield return new WaitForSeconds(1f);
        }

        isDamageCoroutineRunning = false;
    }
}
