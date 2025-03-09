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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }
    }

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

    public void HealHealth(float healAmount)
    {
        // 이미 죽은 상태면 회복 불가
        if (isDead) return;

        // 회복량이 음수면 리턴
        if (healAmount < 0)
        {
            Debug.LogWarning($"{gameObject.name}: Attempted to heal with negative value {healAmount}");
            return;
        }

        if (currentHealth >= maxHealth) return;

        // 회복 적용
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
