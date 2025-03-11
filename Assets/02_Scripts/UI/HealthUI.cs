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

    private void OnDestroy()
    {
        StatHandler.OnHealthChanged -= UpdateHealthBar;
    }

    /// <summary>
    /// ü�¹ٸ� ������Ʈ�ϰ� ���� �ð� �� ����
    /// </summary>
    /// <param name="currentHealth">���� ü�� ��</param>
    /// <param name="maxHealth">�ִ� ü�� ��</param>
    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        gameObject.SetActive(true);
        healthBar.fillAmount = currentHealth / maxHealth;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay(3f));
        Debug.Log("ü�¹� Update");
    }

    /// <summary>
    /// ���� �ð� �� ü�¹ٸ� ����
    /// </summary>
    /// <param name="delay">���� �ð�</param>
    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }
}
