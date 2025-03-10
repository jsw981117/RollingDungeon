using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
    [SerializeField] private float delay = 1f; // 씬 전환 딜레이 (초 단위)

    private void Start()
    {
        StartCoroutine(LoadMainScene());
    }

    private IEnumerator LoadMainScene()
    {
        yield return new WaitForSeconds(delay); // 1초 대기
        SceneManager.LoadScene("MainScene"); // MainScene으로 이동
    }
}