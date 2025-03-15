using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class PlayerDataManager : MonoSingletonBase<PlayerDataManager>
    {
        // 单例实例
        public static PlayerDataManager Instance { get; private set; }

        // 玩家数据
        public PlayerData PlayerData { get; private set; }

        // 事件
        public event Action OnPlayerDataUpdated;

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

            // 初始化玩家数据
            PlayerData = new PlayerData();
        }

        // 更新玩家基本数据
        public void UpdatePlayerData(string userId, string username)
        {
            PlayerData.UserId = userId;
            PlayerData.Username = username;
        }

        // 请求玩家数据
        public void RequestPlayerData()
        {
            // 创建请求数据
            JObject requestData = new JObject();
            
            // 发送请求
            NetManager.Instance.Send(RequestId.PLAYER_DATA_REQUEST, requestData, OnPlayerDataResponseReceived);
        }

        private void OnPlayerDataResponseReceived(ISocketPack pack)
        {
            if (pack.s == 0) // 假设0表示成功
            {
                try
                {
                    // 解析玩家数据
                    JObject data = JObject.Parse((string)pack.d);
                    
                    // 更新整数数据
                    JObject intDict = (JObject)data["int_dict"];
                    foreach (var pair in intDict)
                    {
                        PlayerData.SetIntValue(pair.Key, pair.Value.Value<int>());
                    }
                    
                    // 更新对象数据
                    JObject objDict = (JObject)data["obj_dict"];
                    foreach (var category in objDict)
                    {
                        string categoryName = category.Key;
                        JObject items = (JObject)category.Value;
                        
                        foreach (var item in items)
                        {
                            string itemId = item.Key;
                            Dictionary<string, object> itemData = item.Value.ToObject<Dictionary<string, object>>();
                            
                            PlayerData.SetObjValue(categoryName, itemId, itemData);
                        }
                    }
                    
                    Debug.Log("玩家数据已加载");
                    
                    // 触发数据更新事件
                    OnPlayerDataUpdated?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析玩家数据时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError("获取玩家数据失败");
            }
        }

        // 清除玩家数据
        public void ClearPlayerData()
        {
            PlayerData = new PlayerData();
        }
    }

    // 玩家数据类
    [System.Serializable]
    public class PlayerData
    {
        public string UserId { get; set; }
        public string Username { get; set; }

        private Dictionary<string, int> _intValues = new Dictionary<string, int>();
        private Dictionary<string, Dictionary<string, Dictionary<string, object>>> _objValues =
            new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();

        public int GetIntValue(string key, int defaultValue = 0)
        {
            if (_intValues.TryGetValue(key, out int value))
                return value;
            return defaultValue;
        }

        public void SetIntValue(string key, int value)
        {
            _intValues[key] = value;
        }

        public Dictionary<string, object> GetObjValue(string category, string id)
        {
            if (_objValues.TryGetValue(category, out var categoryDict))
            {
                if (categoryDict.TryGetValue(id, out var objData))
                    return objData;
            }
            return null;
        }

        public void SetObjValue(string category, string id, Dictionary<string, object> data)
        {
            if (!_objValues.TryGetValue(category, out var categoryDict))
            {
                categoryDict = new Dictionary<string, Dictionary<string, object>>();
                _objValues[category] = categoryDict;
            }

            categoryDict[id] = data;
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> GetAllObjValues()
        {
            return _objValues;
        }

        public Dictionary<string, int> GetAllIntValues()
        {
            return _intValues;
        }
    }
}