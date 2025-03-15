using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class AuthManager : MonoBehaviour
    {
        // 单例实例
        public static AuthManager Instance { get; private set; }
        
        // 事件
        public event Action<bool, string> OnLoginResult;
        public event Action<bool, string> OnRegisterResult;
        public event Action OnLogout;

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
        }

        // 登录
        public void Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                OnLoginResult?.Invoke(false, "用户名和密码不能为空");
                return;
            }
            
            // 创建登录请求数据
            JObject data = new JObject
            {
                ["username"] = username,
                ["password"] = password
            };
            
            // 直接发送登录请求
            NetManager.Instance.Send(RequestId.LOGIN, data);
            
            // 登录响应由 LoginResponse 处理
        }

        // 注册
        public void Register(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                OnRegisterResult?.Invoke(false, "用户名和密码不能为空");
                return;
            }
            
            // 创建注册请求数据
            JObject data = new JObject
            {
                ["username"] = username,
                ["password"] = password
            };
            
            // 直接发送注册请求
            NetManager.Instance.Send(RequestId.REGISTER_REQUEST, data);
            
            // 注册响应由相应的 Response 处理
        }
        
        // 登出
        public void Logout()
        {
            // 创建登出请求数据
            JObject data = new JObject();
            
            // 直接发送登出请求
            NetManager.Instance.Send(RequestId.LOGOUT, data);
            
            // 清除玩家数据
            if (PlayerDataManager.Instance != null)
            {
                PlayerDataManager.Instance.ClearPlayerData();
            }
            
            // 返回主菜单
            GameManager.Instance.ChangeState(GameManager.GameState.MainMenu);
            
            // 触发登出事件
            OnLogout?.Invoke();
        }
    }
}