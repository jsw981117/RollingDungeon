using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 moveInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isGrounded;

    // Ground 레이어 값 가져오기
    private int groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground"); // "Ground" 레이어 값 가져오기
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ground 레이어와 충돌했는지 확인
        if (collision.gameObject.layer == groundLayer)
        {
            isGrounded = true;
        }
    }
}
