using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpBoost = 15f;

    private int playerLayer;

    /// <summary>
    /// 플레이어 레이어를 초기화합니다.
    /// </summary>
    private void Awake()
    {
        playerLayer = LayerMask.NameToLayer("Player");
    }

    /// <summary>
    /// 플레이어가 점프 패드와 충돌하면 플레이어를 점프시킵니다.(기본 점프보다 더 높게)
    /// </summary>
    /// <param name="collision">충돌 정보</param>
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