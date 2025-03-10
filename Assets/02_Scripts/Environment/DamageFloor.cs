using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFloor : MonoBehaviour
{
    [SerializeField] private float damagePerSecond = 10f; // �ʴ� ������
    [SerializeField] private string playerLayerName = "Player"; // �÷��̾� ���̾� �̸�
    [SerializeField] private StatHandler playerStatHandler;
    private bool isPlayerOnFloor = false;
    private bool isDamageCoroutineRunning = false; // ������ �ڷ�ƾ�� ���� ������ ����

    private void OnCollisionEnter(Collision collision)
    {
        // �÷��̾� ���̾�� �浹�ߴ��� Ȯ��
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = true;

            // ������ �ڷ�ƾ�� ���� ���� �ƴϸ� ����
            if (!isDamageCoroutineRunning)
            {
                StartCoroutine(ApplyDamageOverTime());
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // �÷��̾� ���̾�� �浹�� �������� Ȯ��
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            isPlayerOnFloor = false;
        }
    }

    private IEnumerator ApplyDamageOverTime()
    {
        isDamageCoroutineRunning = true; // �ڷ�ƾ ���� ������ ǥ��

        while (isPlayerOnFloor)
        {
            if (playerStatHandler != null)
            {
                playerStatHandler.TakeDamage(damagePerSecond); // ������ ����
            }

            // 1�� ���
            yield return new WaitForSeconds(1f);
        }

        isDamageCoroutineRunning = false; // �ڷ�ƾ ����
    }
}
