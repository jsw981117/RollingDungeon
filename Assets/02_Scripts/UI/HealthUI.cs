using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    private Coroutine hideCoroutine; // ü�¹� ����� �ڷ�ƾ

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
        gameObject.SetActive(true);
        healthBar.fillAmount = currentHealth / maxHealth;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine); // ���� �ڷ�ƾ�� ���� ���̸� ����
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        Debug.Log("ü�¹� Update");
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 3�� ���
        gameObject.SetActive(false); // ü�¹� �����
    }
}
