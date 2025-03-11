using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image stamina;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color lowStaminaColor = new Color(1f, 0f, 0f);
    [SerializeField] private float lowStaminaThreshold = 100f;

    private float currentStaminaValue;
    private float maxStaminaValue;

    /// <summary>
    /// �̺�Ʈ�� �����ϰ� �ʱ� ���¹̳� ���� �����մϴ�.
    /// </summary>
    private void OnEnable()
    {
        PlayerEvent.OnStaminaChanged += UpdateStaminaUI;

        // �ʱ� ���� ������ ���� �÷��̾��� ���� ���¹̳� �� ��������
        if (PlayerManager.Instance.PlayerController != null)
        {
            var playerController = PlayerManager.Instance.PlayerController;
            currentStaminaValue = playerController.currentStamina;
            maxStaminaValue = playerController.maxStamina;
            UpdateStaminaUI(currentStaminaValue, maxStaminaValue);
        }
    }

    /// <summary>
    /// �̺�Ʈ ������ �����մϴ�.
    /// </summary>
    private void OnDisable()
    {
        // �̺�Ʈ ���� ����
        PlayerEvent.OnStaminaChanged -= UpdateStaminaUI;
    }

    /// <summary>
    /// ���¹̳� UI�� ������Ʈ�մϴ�.
    /// </summary>
    /// <param name="currentStamina">���� ���¹̳� ��</param>
    /// <param name="maxStamina">�ִ� ���¹̳� ��</param>
    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        currentStaminaValue = currentStamina;
        maxStaminaValue = maxStamina;

        float scale = currentStamina / maxStamina;
        stamina.transform.localScale = new Vector3(scale, 1, 1);

        stamina.color = (currentStamina <= lowStaminaThreshold) ? lowStaminaColor : normalColor;
    }
}