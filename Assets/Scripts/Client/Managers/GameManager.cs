using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client{
    public class GameManager : MonoBehaviour
    {
        // 单例实例
        public static GameManager Instance { get; private set; }

        // 游戏状态
        public enum GameState
        {
            MainMenu,
            Lobby,
            Room,
            Loading,
            Playing,
            GameOver
        }

        // 当前游戏状态
        [SerializeField] private GameState _currentState;
        public GameState CurrentState => _currentState;

        // 事件
        public event Action<GameState, GameState> OnGameStateChanged;

        private void Awake()
        {
            // 单例模式实现
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 设置初始状态
            ChangeState(GameState.MainMenu);
        }

        private void Start()
        {
            // 注册其他管理器的事件
            if (AuthManager.Instance != null)
            {
                AuthManager.Instance.OnLoginResult += HandleLoginResult;
                AuthManager.Instance.OnLogout += HandleLogout;
            }
            
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomJoined += HandleRoomJoined;
                RoomManager.Instance.OnRoomLeft += HandleRoomLeft;
                RoomManager.Instance.OnGameStarted += HandleGameStarted;
            }
            
            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.OnGameOver += HandleGameOver;
            }
        }

        private void OnDestroy()
        {
            // 取消事件注册
            if (AuthManager.Instance != null)
            {
                AuthManager.Instance.OnLoginResult -= HandleLoginResult;
                AuthManager.Instance.OnLogout -= HandleLogout;
            }
            
            if (RoomManager.Instance != null)
            {
                RoomManager.Instance.OnRoomJoined -= HandleRoomJoined;
                RoomManager.Instance.OnRoomLeft -= HandleRoomLeft;
                RoomManager.Instance.OnGameStarted -= HandleGameStarted;
            }
            
            if (GameplayManager.Instance != null)
            {
                GameplayManager.Instance.OnGameOver -= HandleGameOver;
            }
        }

        // 改变游戏状态
        public void ChangeState(GameState newState)
        {
            if (_currentState == newState)
                return;

            GameState oldState = _currentState;
            _currentState = newState;

            // 处理状态变化
            HandleStateChange(oldState, newState);

            // 触发状态变化事件
            OnGameStateChanged?.Invoke(oldState, newState);
        }

        private void HandleStateChange(GameState oldState, GameState newState)
        {
            // 退出旧状态
            switch (oldState)
            {
                case GameState.Playing:
                    // 清理游戏资源
                    if (GameplayManager.Instance != null)
                    {
                        GameplayManager.Instance.CleanupGame();
                    }
                    break;
            }

            // 进入新状态
            switch (newState)
            {
                case GameState.MainMenu:
                    // 加载主菜单场景
                    UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
                    break;
                case GameState.Lobby:
                    // 加载大厅场景
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
                    break;
                case GameState.Room:
                    // 加载房间场景
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
                    break;
                case GameState.Loading:
                    // 加载加载场景
                    UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
                    break;
                case GameState.Playing:
                    // 初始化游戏
                    if (GameplayManager.Instance != null)
                    {
                        GameplayManager.Instance.InitializeGame();
                    }
                    break;
                case GameState.GameOver:
                    // 显示游戏结束界面
                    if (GameplayManager.Instance != null)
                    {
                        GameplayManager.Instance.ShowGameOverUI();
                    }
                    break;
            }
        }

        // 处理登录结果
        private void HandleLoginResult(bool success, string message)
        {
            if (success)
            {
                // 登录成功，切换到大厅
                ChangeState(GameState.Lobby);
            }
        }

        // 处理登出
        private void HandleLogout()
        {
            // 登出，切换到主菜单
            ChangeState(GameState.MainMenu);
        }

        // 处理加入房间
        private void HandleRoomJoined(RoomInfo roomInfo)
        {
            // 加入房间，切换到房间状态
            ChangeState(GameState.Room);
        }

        // 处理离开房间
        private void HandleRoomLeft()
        {
            // 离开房间，切换到大厅
            ChangeState(GameState.Lobby);
        }

        // 处理游戏开始
        private void HandleGameStarted()
        {
            // 游戏开始，切换到加载状态
            ChangeState(GameState.Loading);
            StartCoroutine(DelayedStateChange(GameState.Playing, 2f));
        }

        // 处理游戏结束
        private void HandleGameOver()
        {
            // 游戏结束，切换到游戏结束状态
            ChangeState(GameState.GameOver);
        }

        private IEnumerator DelayedStateChange(GameState newState, float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeState(newState);
        }
    }
}