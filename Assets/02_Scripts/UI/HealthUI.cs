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

    private void OnDestroy()
    {
        StatHandler.OnHealthChanged -= UpdateHealthBar;
    }

    /// <summary>
    /// 체력바를 업데이트하고 일정 시간 후 숨김
    /// </summary>
    /// <param name="currentHealth">현재 체력 값</param>
    /// <param name="maxHealth">최대 체력 값</param>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        gameObject.SetActive(true);
        healthBar.fillAmount = currentHealth / maxHealth;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        Debug.Log("체력바 Update");
    }

    /// <summary>
    /// 일정 시간 후 체력바를 숨김
    /// </summary>
    /// <param name="delay">지연 시간</param>
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
