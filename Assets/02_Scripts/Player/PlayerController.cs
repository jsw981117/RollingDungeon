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

    [Header("Stamina")]
    public float maxStamina = 300f;
    public float currentStamina;
    public float staminaRegenRate = 15f; // 초당 스태미나 회복량
    public float doubleJumpCost = 100f;
    public float dashCost = 100f;
    public float dashForce = 30f; // 대시 힘 증가 (기존 10f에서 30f로)
    public float dashCooldown = 0.5f;
    private bool canDash = true;
    private bool isDashing = false;
    public float dashDuration = 0.2f;

    [Header("Item Detection")]
    public TextMeshProUGUI descText; // UI Text 컴포넌트 참조
    public float rayDistance = 5f; // 레이 캐스트 거리
    public Vector3 rayBoxSize = new Vector3(0.5f, 0.5f, 0.5f); // 레이 박스 크기
    public LayerMask itemLayerMask; // 아이템 레이어 마스크
    [SerializeField] private Inventory inventory;

    [Header("Material Settings")]
    public Material playerMaterial; // 플레이어 오브젝트의 머테리얼
    private Color originalColor; // 원래 색상 저장

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

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>(); // 인벤토리 UI 자동 찾기
            if (inventory == null)
            {
                Debug.LogError("InventoryUI가 씬에 없음!");
            }
        }
        currentStamina = maxStamina;

        if (playerMaterial != null)
        {
            originalColor = playerMaterial.color;
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
        if (!isDashing) // 대시 중이 아닐 때만 일반 이동 처리
        {
            Move();
        }
        DetectItemInFront();
        RegenerateStamina();
    }

    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
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
        if (context.performed)
        {
            if (isGrounded)
            {
                // 기본 점프
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
            else if (currentStamina >= doubleJumpCost)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // 기존 y축 속도 초기화
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                ConsumeStamina(doubleJumpCost);

                StartCoroutine(ChangeColorTemporarily(Color.yellow, 0.5f));
            }
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && currentStamina >= dashCost && !isDashing)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        StartCoroutine(ChangeColorTemporarily(Color.yellow, dashDuration)); // 대시 중에 색 변하는 연출

        // 현재 속도 저장 및 초기화
        Vector3 originalVelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        // 대시 방향(플레이어가 바라보는 방향)
        Vector3 dashDirection = transform.forward;

        // 대시 적용 (ForceMode.VelocityChange를 사용하여 질량 무시)
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        // 스태미나 소모
        ConsumeStamina(dashCost);

        // 대시 지속 시간(지속시간 안넣으니까 진짜 아주 매우 짧게 대시됨)
        yield return new WaitForSeconds(dashDuration);

        // 대시 종료
        isDashing = false;

        // 대시 자체 쿨다운 적용
        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    //private IEnumerator DashCooldown()
    //{
    //    canDash = false;
    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true;
    //}

    // 장착 아이템 사용 입력 처리 (E 키)
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed && inventory != null)
        {
            inventory.UseStoredItem();
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
        if (value <= 0) return;

        currentStamina += value;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
    }

    private void RegenerateStamina()
    {
        // 스태미나가 최대가 아니면 회복
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime * 3;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    private IEnumerator ChangeColorTemporarily(Color targetColor, float duration)
    {
        if (playerMaterial != null)
        {
            playerMaterial.color = targetColor; // 색상 변경
        }

        yield return new WaitForSeconds(duration); // 지정된 시간 대기

        if (playerMaterial != null)
        {
            playerMaterial.color = originalColor; // 원래 색상으로 복원
        }
    }
}