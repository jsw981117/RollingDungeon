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

    public float rotateSpeed = 10.0f; // 회전 속도
    private Transform camTransform; // 메인 카메라의 Transform
    public StatHandler statHandler { get; private set; }

    [Header("Stamina")]
    public float maxStamina = 300f;
    public float currentStamina;
    public float staminaRegenRate = 15f;
    public float doubleJumpCost = 100f;
    public float dashCost = 100f;
    public float dashForce = 30f;
    public float dashCooldown = 0.5f;
    private bool canDash = true;
    private bool isDashing = false;
    public float dashDuration = 0.2f;

    [Header("Item Detection")]
    public TextMeshProUGUI descText;
    public float rayDistance = 5f;
    public Vector3 rayBoxSize = new Vector3(0.5f, 0.5f, 0.5f);
    public LayerMask itemLayerMask;
    [SerializeField] private Inventory inventory;

    [Header("Material Settings")]
    public Material playerMaterial;
    private Color originalColor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground");
        camTransform = Camera.main.transform;
        statHandler = GetComponent<StatHandler>();
        if (statHandler == null)
        {
            Debug.LogError("StatHandler가 Player에 없음!");
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
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

    /// <summary>
    /// 이동 및 아이템 감지, 스태미나 회복
    /// </summary>
    void Update()
    {
        if (!isDashing)
        {
            Move();
        }
        DetectItemInFront();
        RegenerateStamina();
        rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);
    }

    /// <summary>
    /// 이동 방향에 따라 캐릭터를 회전
    /// </summary>
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

    /// <summary>
    /// 플레이어 이동
    /// </summary>
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

    /// <summary>
    /// 플레이어 앞의 아이템을 감지
    /// </summary>
    void DetectItemInFront()
    {
        RaycastHit[] hits = Physics.BoxCastAll(
            transform.position,
            rayBoxSize / 2,
            transform.forward,
            Quaternion.identity,
            rayDistance,
            itemLayerMask
        );

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                IItem item = hit.collider.GetComponent<IItem>();
                if (item != null)
                {
                    if (descText != null)
                    {
                        descText.text = item.GetItemName();
                    }
                    return;
                }
            }
        }
        else
        {
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

    /// <summary>
    /// 대시
    /// </summary>
    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        StartCoroutine(ChangeColorTemporarily(Color.yellow, dashDuration)); // 대시 중에 색 변하는 연출

        Vector3 originalVelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        Vector3 dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);
        ConsumeStamina(dashCost);

        // 대시 지속 시간(지속시간 안넣으니까 진짜 아주 매우 짧게 대시됨)
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    /// <summary>
    /// 장착 아이템 사용 입력 처리 (E 키)
    /// </summary>
    /// <param name="context">입력 컨텍스트</param>
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

    /// <summary>
    /// 체력을 회복
    /// </summary>
    /// <param name="value">회복할 체력 값</param>
    public void Heal(float value)
    {
        statHandler.HealHealth(value);
    }

    /// <summary>
    /// 스태미나 회복(아이템으로 순간적으로 회복)
    /// </summary>
    /// <param name="value">증가할 스태미나 값</param>
    public void IncreaseStamina(float value)
    {
        if (value <= 0) return;

        currentStamina += value;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        PlayerEvent.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    /// <summary>
    /// 스태미나 소모
    /// </summary>
    /// <param name="amount">소모할 스태미나 양</param>
    private void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        PlayerEvent.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    /// <summary>
    /// 스태미나 지속 회복
    /// </summary>
    private void RegenerateStamina()
    {
        // 이전 스태미나 값 저장
        float previousStamina = currentStamina;

        // 스태미나가 최대가 아니면 회복
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime * 3;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            if (currentStamina != previousStamina)
            {
                PlayerEvent.TriggerStaminaChanged(currentStamina, maxStamina);
            }
        }
    }

    /// <summary>
    /// 일시적으로 플레이어의 색상을 변경(멀티 점프, 대시 시 연출)
    /// </summary>
    /// <param name="targetColor">변경할 색상</param>
    /// <param name="duration">변경 지속 시간</param>
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