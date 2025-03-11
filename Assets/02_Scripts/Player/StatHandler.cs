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
    /// 테스트용, K 키를 눌러 데미지를 받음
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }
    }

    /// <summary>
    /// 데미지를 받아 체력을 감소시키고, 체력이 0 이하가 되면 죽음 처리
    /// </summary>
    /// <param name="damage">받는 데미지 양</param>
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
    /// 체력을 회복시키고, 최대 체력을 초과하지 않도록 함
    /// </summary>
    /// <param name="healAmount">회복할 체력 양</param>
    public void HealHealth(float healAmount)
    {
        // 이미 죽은 상태면 회복 불가
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
        gameObject.SetActive(false); // 죽으면 일단 플레이어 비활성화
    }
}
