using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpBoost = 15f;

    private int playerLayer;

    /// <summary>
    /// �÷��̾� ���̾ �ʱ�ȭ�մϴ�.
    /// </summary>
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    /// <summary>
    /// �÷��̾ ���� �е�� �浹�ϸ� �÷��̾ ������ŵ�ϴ�.(�⺻ �������� �� ����)
    /// </summary>
    /// <param name="collision">�浹 ����</param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == playerLayer)
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            playerRb.velocity = new Vector3(playerRb.velocity.x, 0, playerRb.velocity.z);
            playerRb.AddForce(Vector3.up * jumpBoost, ForceMode.Impulse);
        }
    }
}