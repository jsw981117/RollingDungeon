using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FollowCamera : MonoBehaviour
{
    public GameObject Target;               // 카메라가 따라다닐 타겟

    public float offsetX = 0.0f;            // 카메라의 x좌표
    public float offsetY = 10.0f;           // 카메라의 y좌표
    public float offsetZ = -10.0f;          // 카메라의 z좌표

    public float cameraSpeed = 100.0f;       // 카메라의 속도
    public float rotationSpeed = 100.0f;      // 카메라 회전 속도

    private float rotationX = 0f;           // X축 회전값 (상하 회전)
    private float rotationY = 0f;           // Y축 회전값 (좌우 회전)

    Vector3 TargetPos;                      // 타겟의 위치

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서를 화면 중앙에 고정
        Cursor.visible = false;                   // 마우스 커서를 숨김
    }

    void LateUpdate()
    {
        HandleRotation(); // 마우스 회전 처리
        HandleMovement(); // 카메라 위치 업데이트
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -40f, 80f); // 상하 회전 제한 (과도한 기울기 방지)

        // 회전 적용
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    void HandleMovement()
    {
        // 타겟의 위치에 offset을 적용하여 카메라 위치 설정
        Vector3 targetPosition = Target.transform.position
            + transform.right * offsetX
            + transform.up * offsetY
            + transform.forward * offsetZ;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
    }
}
