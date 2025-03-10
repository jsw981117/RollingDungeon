using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class FollowCamera : MonoBehaviour
{
    public GameObject Target;               // ī�޶� ����ٴ� Ÿ��

    public float offsetX = 0.0f;            // ī�޶��� x��ǥ
    public float offsetY = 10.0f;           // ī�޶��� y��ǥ
    public float offsetZ = -10.0f;          // ī�޶��� z��ǥ

    public float cameraSpeed = 100.0f;       // ī�޶��� �ӵ�
    public float rotationSpeed = 100.0f;      // ī�޶� ȸ�� �ӵ�

    private float rotationX = 0f;           // X�� ȸ���� (���� ȸ��)
    private float rotationY = 0f;           // Y�� ȸ���� (�¿� ȸ��)

    Vector3 TargetPos;                      // Ÿ���� ��ġ

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ���� ȭ�� �߾ӿ� ����
        Cursor.visible = false;                   // ���콺 Ŀ���� ����
    }

    void LateUpdate()
    {
        HandleRotation(); // ���콺 ȸ�� ó��
        HandleMovement(); // ī�޶� ��ġ ������Ʈ
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        rotationY += mouseX;
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -40f, 80f); // ���� ȸ�� ���� (������ ���� ����)

        // ȸ�� ����
        transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);
    }

    void HandleMovement()
    {
        // Ÿ���� ��ġ�� offset�� �����Ͽ� ī�޶� ��ġ ����
        Vector3 targetPosition = Target.transform.position
            + transform.right * offsetX
            + transform.up * offsetY
            + transform.forward * offsetZ;

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * cameraSpeed);
    }
}
