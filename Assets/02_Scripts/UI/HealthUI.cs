using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthBar;
    private Coroutine hideCoroutine; // 체력바 숨기는 코루틴

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
            StopCoroutine(hideCoroutine); // 기존 코루틴이 실행 중이면 중지
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        Debug.Log("체력바 Update");
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 3초 대기
        gameObject.SetActive(false); // 체력바 숨기기
    }
}
