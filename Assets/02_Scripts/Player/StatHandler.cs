using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatHandler : MonoBehaviour
{
    public static event Action<float, float> OnHealthChanged;

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] public float currentHealth;
    private bool isDead = false;


    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// �׽�Ʈ��, K Ű�� ���� �������� ����
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }
    }

    /// <summary>
    /// �������� �޾� ü���� ���ҽ�Ű��, ü���� 0 ���ϰ� �Ǹ� ���� ó��
    /// </summary>
    /// <param name="damage">�޴� ������ ��</param>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    /// <summary>
    /// ü���� ȸ����Ű��, �ִ� ü���� �ʰ����� �ʵ��� ��
    /// </summary>
    /// <param name="healAmount">ȸ���� ü�� ��</param>
    public void HealHealth(float healAmount)
    {
        // �̹� ���� ���¸� ȸ�� �Ұ�
        if (isDead) return;

        if (healAmount < 0)
        {
            Debug.LogWarning($"{gameObject.name}: Attempted to heal with negative value {healAmount}");
            return;
        }

        if (currentHealth >= maxHealth) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player Died");
        gameObject.SetActive(false); // ������ �ϴ� �÷��̾� ��Ȱ��ȭ
    }
}
