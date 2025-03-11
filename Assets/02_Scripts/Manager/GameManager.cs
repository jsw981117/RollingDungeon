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
    /// 현재 점수를 초기화
    /// </summary>
    public void ResetScore()
    {
        currentScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }

    /// <summary>
    /// 점수를 추가하고, 최고 점수를 갱신
    /// </summary>
    /// <param name="amount">추가할 점수</param>
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
    /// 최고 점수를 저장
    /// </summary>
    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 최고 점수를 불러옴
    /// </summary>
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    /// <summary>
    /// 최고 점수를 반환 (UI 등에 표시할 때 사용)
    /// </summary>
    /// <returns>최고 점수</returns>
    public int GetHighScore()
    {
        return highScore;
    }

    /// <summary>
    /// 현재 점수를 반환 (UI 등에 표시할 때 사용)
    /// </summary>
    /// <returns>현재 점수</returns>
    public int GetCurrentScore()
    {
        return currentScore;
    }
}
