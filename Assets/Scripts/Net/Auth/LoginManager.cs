
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    // 单例实例
    public static LoginManager Instance { get; private set; }
    
    // 登录状态
    private bool _isLoggedIn = false;
    private string _username = "";
    private string _userId = "";
    
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
    
    // 登录请求
    public void Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            OnLoginResult?.Invoke(false, "用户名和密码不能为空");
            return;
        }
        
        // 创建登录数据
        JObject loginData = new JObject();
        loginData["username"] = username;
        loginData["password"] = password;
        LogTool.Log("发送登录请求: " + username);
        
        // 发送登录请求
        NetManager.Instance.Send(RequestId.LOGIN, loginData, OnLoginResponseReceived);
        
    }
    
    // 注册请求
    public void Register(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            OnRegisterResult?.Invoke(false, "用户名和密码不能为空");
            return;
        }
        
        // 创建注册数据
        JObject registerData = new JObject();
        registerData["username"] = username;
        registerData["password"] = password;
        
        // 发送注册请求
        NetManager.Instance.Send(RequestId.REGISTER_REQUEST, registerData, OnRegisterResponseReceived);
        
        LogTool.Log("发送注册请求: " + username);
    }
    
    // 登出请求
    public void Logout()
    {
        if (!_isLoggedIn)
            return;
        
        // 发送登出请求
        NetManager.Instance.Send(RequestId.LOGOUT, null);
        
        // 清除登录状态
        ClearLoginState();
        OnLogout?.Invoke();
        
        LogTool.Log("用户登出");
    }
    
    // 处理登录响应
    private void OnLoginResponseReceived(ISocketPack pack)
    {
        LogTool.Log("收到登录响应: " + JsonTool.SerializeObject(pack));
        
        // 修改这里，使用正确的响应状态枚举
        if (pack.s == 0) // LoginResponseStatus.LOGIN_OK
        {
            // 登录成功
            JObject data = JObject.Parse((string)pack.d);
            
            _isLoggedIn = true;
            _userId = data["gameUid"].ToString();
            _username = data["username"]?.ToString() ?? "";
            
            OnLoginResult?.Invoke(true, "登录成功");
            LogTool.Log("登录成功: " + _username);
        }
        else
        {
            // 登录失败
            string errorMessage = "登录失败";
            // 修改这里，使用正确的响应状态枚举
            switch (pack.s)
            {
                case 101: // INVALID_LOGIN_INFO
                    errorMessage = "用户名或密码错误";
                    break;
                case 103: // USER_BANNED
                    errorMessage = "账号已被封禁";
                    break;
                case 100: // SERVER_MAINTAIN
                    errorMessage = "服务器维护中";
                    break;
                default:
                    errorMessage = $"登录失败，错误码: {pack.s}";
                    break;
            }
            
            OnLoginResult?.Invoke(false, errorMessage);
            LogTool.Log("登录失败: " + errorMessage);
        }
    }
    
    // 处理注册响应
    private void OnRegisterResponseReceived(ISocketPack pack)
    {
        LogTool.Log("收到注册响应: " + JsonTool.SerializeObject(pack));
        
        if (pack.s == 0) // 假设0表示成功
        {
            OnRegisterResult?.Invoke(true, "注册成功");
            LogTool.Log("注册成功");
        }
        else
        {
            string errorMessage = "注册失败";
            if (pack.d != null)
            {
                try
                {
                    JObject data = JObject.Parse((string)pack.d);
                    errorMessage = data["message"]?.ToString() ?? errorMessage;
                }
                catch { }
            }
            
            OnRegisterResult?.Invoke(false, errorMessage);
            LogTool.Log("注册失败: " + errorMessage);
        }
    }
    
    // 清除登录状态
    private void ClearLoginState()
    {
        _isLoggedIn = false;
        _userId = "";
        _username = "";
    }
    
    // 获取登录状态
    public bool IsLoggedIn()
    {
        return _isLoggedIn;
    }
    
    // 获取用户ID
    public string GetUserId()
    {
        return _userId;
    }
    
    // 获取用户名
    public string GetUsername()
    {
        return _username;
    }
}