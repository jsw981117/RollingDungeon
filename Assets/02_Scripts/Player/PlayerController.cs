using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody rb;
    private Vector2 moveInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isGrounded;

    // Ground 레이어 값 가져오기
    private int groundLayer;
    public float rotateSpeed = 10.0f;       // 회전 속도

    float h, v;

    private Transform camTransform;     // 메인 카메라의 Transform

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground"); // "Ground" 레이어 값 가져오기
        camTransform = Camera.main.transform; // 메인 카메라의 Transform을 가져옴
    }

    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f) // 입력이 있을 때만 방향 전환
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

            // 카메라 방향을 기준으로 이동 방향 변환
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;
            camForward.y = 0;  // 수직 방향 제거 (평면 이동)
            camRight.y = 0;

            Vector3 adjustedMoveDirection = camForward.normalized * moveDirection.z + camRight.normalized * moveDirection.x;

            // 이동 방향으로 캐릭터 회전
            Quaternion targetRotation = Quaternion.LookRotation(adjustedMoveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        //rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 adjustedMoveDirection = camForward.normalized * moveDirection.z + camRight.normalized * moveDirection.x;

        rb.velocity = new Vector3(adjustedMoveDirection.x * moveSpeed, rb.velocity.y, adjustedMoveDirection.z * moveSpeed);
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
