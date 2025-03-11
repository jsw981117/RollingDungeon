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
    /// �÷��̾ ������ �÷ξ�� �浹�� �� �������� �����մϴ�.
    /// </summary>
    /// <param name="collision">�浹 ����</param>
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
    /// �÷��̾ ������ �÷ξ�� ��� �� ������ ������ �����մϴ�.
    /// </summary>
    /// <param name="collision">�浹 ����</param>
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = false;
        }
    }

    /// <summary>
    /// �÷��̾ ������ �÷ξ� ���� �ִ� ���� ���������� �������� �����մϴ�.
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
