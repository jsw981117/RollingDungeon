using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
    [SerializeField] private float delay = 1f; // �� ��ȯ ������ (�� ����)

    private void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    private IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(delay); // 1�� ���
        SceneManager.LoadScene("MainScene"); // MainScene���� �̵�
    }
}