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

    public float rotateSpeed = 10.0f; // ȸ�� �ӵ�
    private Transform camTransform; // ���� ī�޶��� Transform
    private StatHandler statHandler;

    [Header("Stamina")]
    public float maxStamina = 300f;
    public float currentStamina;
    public float staminaRegenRate = 15f; // �ʴ� ���¹̳� ȸ����
    public float doubleJumpCost = 100f;
    public float dashCost = 100f;
    public float dashForce = 30f; // ��� �� ���� (���� 10f���� 30f��)
    public float dashCooldown = 0.5f;
    private bool canDash = true;
    private bool isDashing = false;
    public float dashDuration = 0.2f;

    [Header("Item Detection")]
    public TextMeshProUGUI descText; // UI Text ������Ʈ ����
    public float rayDistance = 5f; // ���� ĳ��Ʈ �Ÿ�
    public Vector3 rayBoxSize = new Vector3(0.5f, 0.5f, 0.5f); // ���� �ڽ� ũ��
    public LayerMask itemLayerMask; // ������ ���̾� ����ũ
    [SerializeField] private Inventory inventory;

    [Header("Material Settings")]
    public Material playerMaterial; // �÷��̾� ������Ʈ�� ���׸���
    private Color originalColor; // ���� ���� ����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.NameToLayer("Ground"); // "Ground" ���̾� �� ��������
        camTransform = Camera.main.transform; // ���� ī�޶��� Transform�� ������
        statHandler = GetComponent<StatHandler>();
        if (statHandler == null)
        {
            Debug.LogError("StatHandler�� Player�� ����!");
        }

        if (inventory == null)
        {
            inventory = FindObjectOfType<Inventory>(); // �κ��丮 UI �ڵ� ã��
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

    void Update()
    {
        if (!isDashing) // ��� ���� �ƴ� ���� �Ϲ� �̵� ó��
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
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 adjustedMoveDirection = camForward.normalized * moveDirection.z + camRight.normalized * moveDirection.x;

        rb.velocity = new Vector3(adjustedMoveDirection.x * moveSpeed, rb.velocity.y, adjustedMoveDirection.z * moveSpeed);
    }

    void DetectItemInFront()
    {
        // �÷��̾� ���� �������� BoxCast ����
        RaycastHit[] hits = Physics.BoxCastAll(
            transform.position,           // ���� ��ġ
            rayBoxSize / 2,               // �ڽ��� ��ũ�� 
            transform.forward,            // ����
            Quaternion.identity,          // ȸ�� ����
            rayDistance,                  // �Ÿ�
            itemLayerMask                 // ������ ���̾� ����ũ
        );

        if (hits.Length > 0)
        {
            foreach (RaycastHit hit in hits)
            {
                // �ε��� ������Ʈ�� IItem �������̽��� �����ϴ��� Ȯ��
                IItem item = hit.collider.GetComponent<IItem>();
                if (item != null)
                {
                    // ������ �̸��� UI �ؽ�Ʈ�� ǥ��
                    if (descText != null)
                    {
                        descText.text = item.GetItemName();
                    }
                    return; // ù ��° ������ �����۸� ǥ��
                }
            }
        }
        else
        {
            // �������� �������� �ʾ����� �ؽ�Ʈ �ʱ�ȭ
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

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        StartCoroutine(ChangeColorTemporarily(Color.yellow, dashDuration)); // ��� �߿� �� ���ϴ� ����

        // ���� �ӵ� ���� �� �ʱ�ȭ
        Vector3 originalVelocity = rb.velocity;
        rb.velocity = Vector3.zero;

        // ��� ����(�÷��̾ �ٶ󺸴� ����)
        Vector3 dashDirection = transform.forward;

        // ��� ���� (ForceMode.VelocityChange�� ����Ͽ� ���� ����)
        rb.AddForce(dashDirection * dashForce, ForceMode.VelocityChange);

        // ���¹̳� �Ҹ�
        ConsumeStamina(dashCost);

        // ��� ���� �ð�(���ӽð� �ȳ����ϱ� ��¥ ���� �ſ� ª�� ��õ�)
        yield return new WaitForSeconds(dashDuration);

        // ��� ����
        isDashing = false;

        // ��� ��ü ��ٿ� ����
        yield return new WaitForSeconds(dashCooldown - dashDuration);
        canDash = true;
    }

    //private IEnumerator DashCooldown()
    //{
    //    canDash = false;
    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true;
    //}

    // ���� ������ ��� �Է� ó�� (E Ű)
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
        // ���¹̳��� �ִ밡 �ƴϸ� ȸ��
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
            playerMaterial.color = targetColor; // ���� ����
        }

        yield return new WaitForSeconds(duration); // ������ �ð� ���

        if (playerMaterial != null)
        {
            playerMaterial.color = originalColor; // ���� �������� ����
        }
    }
}