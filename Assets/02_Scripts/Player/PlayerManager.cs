using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PlayerManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("PlayerManager");
                    _instance = obj.AddComponent<PlayerManager>();
                }
            }
            return _instance;
        }
    }

    public GameObject Player { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public StatHandler PlayerStatHandler { get; private set; }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

        InitializePlayer();
    }

    private void InitializePlayer()
    {
        Player = GameObject.FindGameObjectWithTag("Player"); // "Player" 태그로 플레이어 찾기
        if (Player != null)
        {
            PlayerController = Player.GetComponent<PlayerController>();
            PlayerStatHandler = Player.GetComponent<StatHandler>();
        }
        else
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다!");
        }
    }
}
