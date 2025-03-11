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
    /// 이벤트를 구독하고 초기 스태미나 값을 설정합니다.
    /// </summary>
    private void OnEnable()
    {
        PlayerEvent.OnStaminaChanged += UpdateStaminaUI;

        // 초기 상태 설정을 위해 플레이어의 현재 스태미나 값 가져오기
        if (PlayerManager.Instance.PlayerController != null)
        {
            var playerController = PlayerManager.Instance.PlayerController;
            currentStaminaValue = playerController.currentStamina;
            maxStaminaValue = playerController.maxStamina;
            UpdateStaminaUI(currentStaminaValue, maxStaminaValue);
        }
    }

    /// <summary>
    /// 이벤트 구독을 해제합니다.
    /// </summary>
    private void OnDisable()
    {
        // 이벤트 구독 해제
        PlayerEvent.OnStaminaChanged -= UpdateStaminaUI;
    }

    /// <summary>
    /// 스태미나 UI를 업데이트합니다.
    /// </summary>
    /// <param name="currentStamina">현재 스태미나 값</param>
    /// <param name="maxStamina">최대 스태미나 값</param>
    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        currentStaminaValue = currentStamina;
        maxStaminaValue = maxStamina;

        float scale = currentStamina / maxStamina;
        stamina.transform.localScale = new Vector3(scale, 1, 1);

        stamina.color = (currentStamina <= lowStaminaThreshold) ? lowStaminaColor : normalColor;
    }
}