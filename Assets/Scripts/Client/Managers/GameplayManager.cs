using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class GameplayManager : MonoBehaviour
    {
        // 单例实例
        public static GameplayManager Instance { get; private set; }

        // 游戏地图
        public MapData CurrentMap { get; private set; }

        // 事件
        public event Action<MapData> OnMapLoaded;
        public event Action OnGameOver;

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

        // 初始化游戏
        public void InitializeGame()
        {
            Debug.Log("初始化游戏...");
            
            // 请求地图数据
            RequestMapData();
        }

        // 请求地图数据
        private void RequestMapData()
        {
            // 创建请求数据
            JObject data = new JObject();
            
            // 发送请求
            NetManager.Instance.Send(RequestId.MAP_DATA_REQUEST, data, OnMapDataResponseReceived);
        }

        private void OnMapDataResponseReceived(ISocketPack pack)
        {
            if (pack.s == 0) // 假设0表示成功
            {
                try
                {
                    // 解析地图数据
                    JObject mapData = JObject.Parse((string)pack.d);
                    Dictionary<string, object> mapDict = mapData.ToObject<Dictionary<string, object>>();
                    CurrentMap = new MapData(mapDict);

                    Debug.Log($"地图数据已加载: {CurrentMap.MapId}");

                    // 触发地图加载事件
                    OnMapLoaded?.Invoke(CurrentMap);
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析地图数据时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"获取地图数据失败，错误码: {pack.s}");
            }
        }

        // 发送玩家输入
        public void SendPlayerInput(Dictionary<string, object> inputData)
        {
            if (GameManager.Instance.CurrentState != GameManager.GameState.Playing)
                return;
            
            // 创建请求数据
            JObject data = new JObject
            {
                ["frame_id"] = Time.frameCount,
                ["input_data"] = JObject.FromObject(inputData)
            };
            
            // 发送请求
            NetManager.Instance.Send(RequestId.GAME_PLAYER_INPUT, data);
        }

        // 清理游戏
        public void CleanupGame()
        {
            Debug.Log("清理游戏资源...");
            // 清理游戏资源
            CurrentMap = null;
        }

        // 显示游戏结束界面
        public void ShowGameOverUI()
        {
            Debug.Log("游戏结束");
            // 显示游戏结束界面
            
            // 触发游戏结束事件
            OnGameOver?.Invoke();
        }
    }

    // 地图数据类
    [System.Serializable]
    public class MapData
    {
        public string MapId { get; private set; }
        public string MapName { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Dictionary<string, object> MapConfig { get; private set; }
        public List<Dictionary<string, object>> Rooms { get; private set; }
        public List<Dictionary<string, object>> Enemies { get; private set; }
        public List<Dictionary<string, object>> Items { get; private set; }

        public MapData(Dictionary<string, object> data)
        {
            UpdateFromData(data);
        }

        public void UpdateFromData(Dictionary<string, object> data)
        {
            MapId = (string)data["map_id"];
            MapName = (string)data["map_name"];
            Width = Convert.ToInt32(data["width"]);
            Height = Convert.ToInt32(data["height"]);
            
            if (data.ContainsKey("config"))
            {
                MapConfig = ((JObject)data["config"]).ToObject<Dictionary<string, object>>();
            }
            
            if (data.ContainsKey("rooms") && data["rooms"] is JArray roomsArray)
            {
                Rooms = roomsArray.ToObject<List<Dictionary<string, object>>>();
            }
            
            if (data.ContainsKey("enemies") && data["enemies"] is JArray enemiesArray)
            {
                Enemies = enemiesArray.ToObject<List<Dictionary<string, object>>>();
            }
            
            if (data.ContainsKey("items") && data["items"] is JArray itemsArray)
            {
                Items = itemsArray.ToObject<List<Dictionary<string, object>>>();
            }
        }
    }
}