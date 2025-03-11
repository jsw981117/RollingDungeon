using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int currentScore = 0;
    private int highScore = 0;

    public static event Action<int> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        LoadHighScore();
        ResetScore();
    }

    /// <summary>
    /// ���� ������ �ʱ�ȭ
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// ������ �߰��ϰ�, �ְ� ������ ����
    /// </summary>
    /// <param name="amount">�߰��� ����</param>
    public void AddScore(int amount)
    {
        currentScore += amount;

        OnScoreChanged?.Invoke(currentScore);

        if (currentScore > highScore)
        {
            highScore = currentScore;
            SaveHighScore();
        }
    }

    /// <summary>
    /// �ְ� ������ ����
    /// </summary>
    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �ְ� ������ �ҷ���
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// �ְ� ������ ��ȯ (UI � ǥ���� �� ���)
    /// </summary>
    /// <returns>�ְ� ����</returns>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// ���� ������ ��ȯ (UI � ǥ���� �� ���)
    /// </summary>
    /// <returns>���� ����</returns>
    public int GetCurrentScore()
    {
        return currentScore;
    }
}
