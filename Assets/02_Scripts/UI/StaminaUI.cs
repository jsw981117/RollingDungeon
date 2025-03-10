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

    private void OnEnable()
    {
        // 이벤트 구독
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

    private void OnDisable()
    {
        // 이벤트 구독 해제
        PlayerEvent.OnStaminaChanged -= UpdateStaminaUI;
    }

    private void UpdateStaminaUI(float currentStamina, float maxStamina)
    {
        // 값 저장
        currentStaminaValue = currentStamina;
        maxStaminaValue = maxStamina;

        // UI 업데이트
        float scale = currentStamina / maxStamina;
        stamina.transform.localScale = new Vector3(scale, 1, 1);

        // 스태미나가 낮을 때 색상 변경
        stamina.color = (currentStamina <= lowStaminaThreshold) ? lowStaminaColor : normalColor;
    }
}