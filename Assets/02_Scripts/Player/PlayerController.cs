using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody rb;
    private Vector2 moveInput;
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private bool isGrounded;

    private int groundLayer;
    private int trapLayer;

    public float rotateSpeed = 10.0f; // 회전 속도
    private Transform camTransform; // 메인 카메라의 Transform
    private StatHandler statHandler;

    [Header("Item Detection")]
    public TextMeshProUGUI descText; // UI Text 컴포넌트 참조
    public float rayDistance = 5f; // 레이 캐스트 거리
    public Vector3 rayBoxSize = new Vector3(0.5f, 0.5f, 0.5f); // 레이 박스 크기
    public LayerMask itemLayerMask; // 아이템 레이어 마스크

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground"); // "Ground" 레이어 값 가져오기
        camTransform = Camera.main.transform; // 메인 카메라의 Transform을 가져옴
        statHandler = GetComponent<StatHandler>();
        if (statHandler == null)
        {
            Debug.LogError("StatHandler가 Player에 없음!");
        }
    }

    void OnEnable()
    {
        PlayerEvent.OnHealthIncrease += Heal;
        PlayerEvent.OnStaminaIncrease += IncreaseStamina;
    }

    void OnDisable()
    {
        PlayerEvent.OnHealthIncrease -= Heal;
        PlayerEvent.OnStaminaIncrease -= IncreaseStamina;
    }

    void Update()
    {
        Move();
        DetectItemInFront();
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
        // rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 adjustedMoveDirection = camForward.normalized * moveDirection.z + camRight.normalized * moveDirection.x;

        rb.velocity = new Vector3(adjustedMoveDirection.x * moveSpeed, rb.velocity.y, adjustedMoveDirection.z * moveSpeed);
    }

    void DetectItemInFront()
    {
        // 플레이어 정면 방향으로 BoxCast 수행
        RaycastHit[] hits = Physics.BoxCastAll(
            transform.position,           // 시작 위치
            rayBoxSize / 2,               // 박스의 반크기 
            transform.forward,            // 방향
            Quaternion.identity,          // 회전 없음
            rayDistance,                  // 거리
            itemLayerMask                 // 아이템 레이어 마스크
        );

        // 디버그 시각화
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.red);

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                // 부딪힌 오브젝트가 IItem 인터페이스를 구현하는지 확인
                IItem item = hit.collider.GetComponent<IItem>();
                if (item != null)
                {
                    // 아이템 이름을 UI 텍스트에 표시
                    if (descText != null)
                    {
                        descText.text = item.GetItemName();
                    }
                    return; // 첫 번째 감지된 아이템만 표시
                }
            }
        }
        else
        {
            // 아이템이 감지되지 않았으면 텍스트 초기화
            if (descText != null)
            {
                descText.text = "";
            }
        }
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

    public void Heal(float value)
    {
        statHandler.HealHealth(value);
    }

    public void IncreaseStamina(float value)
    {
        Debug.Log($"Stamina increased by {value}");
    }
}
