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

    // Ground ���̾� �� ��������
    private int groundLayer;
    public float rotateSpeed = 10.0f;       // ȸ�� �ӵ�

    float h, v;

    private Transform camTransform;     // ���� ī�޶��� Transform

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground"); // "Ground" ���̾� �� ��������
        camTransform = Camera.main.transform; // ���� ī�޶��� Transform�� ������
    }

    void Update()
    {
        Move();
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f) // �Է��� ���� ���� ���� ��ȯ
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

            // ī�޶� ������ �������� �̵� ���� ��ȯ
            Vector3 camForward = camTransform.forward;
            Vector3 camRight = camTransform.right;
            camForward.y = 0;  // ���� ���� ���� (��� �̵�)
            camRight.y = 0;

            Vector3 adjustedMoveDirection = camForward.normalized * moveDirection.z + camRight.normalized * moveDirection.x;

            // �̵� �������� ĳ���� ȸ��
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
        // Ground ���̾�� �浹�ߴ��� Ȯ��
        if (collision.gameObject.layer == groundLayer)
        {
            isGrounded = true;
        }
    }
}
