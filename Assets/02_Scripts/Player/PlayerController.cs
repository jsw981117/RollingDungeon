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

    public float rotateSpeed = 10.0f; // ȸ�� �ӵ�
    private Transform camTransform; // ���� ī�޶��� Transform
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
            Debug.LogError("StatHandler�� Player�� ����!");
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>();
            if (inventory == null)
            {
                Debug.LogError("InventoryUI�� ���� ����!");
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
    /// �̵� �� ������ ����, ���¹̳� ȸ��
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
    /// �̵� ���⿡ ���� ĳ���͸� ȸ��
    /// </summary>
    void FixedUpdate()
    {
        if (moveInput.sqrMagnitude > 0.01f)
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

    /// <summary>
    /// �÷��̾� �̵�
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
    /// �÷��̾� ���� �������� ����
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
                // �⺻ ����
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }
            else if (currentStamina >= doubleJumpCost)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); // ���� y�� �ӵ� �ʱ�ȭ
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
    /// ���
    /// </summary>
    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        StartCoroutine(ChangeColorTemporarily(Color.yellow, dashDuration)); // ��� �߿� �� ���ϴ� ����

        Vector3 originalVelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        Vector3 dashDirection = transform.forward;
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);
        ConsumeStamina(dashCost);

        // ��� ���� �ð�(���ӽð� �ȳ����ϱ� ��¥ ���� �ſ� ª�� ��õ�)
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    /// <summary>
    /// ���� ������ ��� �Է� ó�� (E Ű)
    /// </summary>
    /// <param name="context">�Է� ���ؽ�Ʈ</param>
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.performed && inventory != null)
        {
            inventory.UseStoredItem();
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

    /// <summary>
    /// ü���� ȸ��
    /// </summary>
    /// <param name="value">ȸ���� ü�� ��</param>
    public void Heal(float value)
    {
        statHandler.HealHealth(value);
    }

    /// <summary>
    /// ���¹̳� ȸ��(���������� ���������� ȸ��)
    /// </summary>
    /// <param name="value">������ ���¹̳� ��</param>
    public void IncreaseStamina(float value)
    {
        if (value <= 0) return;

        currentStamina += value;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        PlayerEvent.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    /// <summary>
    /// ���¹̳� �Ҹ�
    /// </summary>
    /// <param name="amount">�Ҹ��� ���¹̳� ��</param>
    private void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        PlayerEvent.TriggerStaminaChanged(currentStamina, maxStamina);
    }

    /// <summary>
    /// ���¹̳� ���� ȸ��
    /// </summary>
    private void RegenerateStamina()
    {
        // ���� ���¹̳� �� ����
        float previousStamina = currentStamina;

        // ���¹̳��� �ִ밡 �ƴϸ� ȸ��
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
    /// �Ͻ������� �÷��̾��� ������ ����(��Ƽ ����, ��� �� ����)
    /// </summary>
    /// <param name="targetColor">������ ����</param>
    /// <param name="duration">���� ���� �ð�</param>
    private IEnumerator ChangeColorTemporarily(Color targetColor, float duration)
    {
        if (playerMaterial != null)
        {
            playerMaterial.color = targetColor; // ���� ����
        }

        yield return new WaitForSeconds(duration); // ������ �ð� ���

        if (playerMaterial != null)
        {
            playerMaterial.color = originalColor; // ���� �������� ����
        }
    }
}