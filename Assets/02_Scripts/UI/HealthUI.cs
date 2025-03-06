using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;

    private void OnEnable()
    {
        StatHandler.OnHealthChanged += UpdateHealthBar;
    }

    private void OnDisable()
    {
        StatHandler.OnHealthChanged -= UpdateHealthBar;
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void Hide()
    {
        healthBar.gameObject.SetActive(false);
    }
}
