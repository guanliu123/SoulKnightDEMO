using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Client
{
    public class RoomManager : MonoSingletonBase<RoomManager>
    {
        // 单例实例
        public static RoomManager Instance { get; private set; }

        // 房间信息
        public RoomInfo CurrentRoom { get; private set; }

        // 事件
        public event Action<RoomInfo> OnRoomJoined;
        public event Action OnRoomLeft;
        public event Action OnGameStarted;

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

        // 创建房间
        public void CreateRoom(string roomName, int maxPlayers = 4)
        {
            // 创建请求数据
            JObject data = new JObject
            {
                ["name"] = roomName,
                ["max_players"] = maxPlayers
            };
            
            // 发送请求
            NetManager.Instance.Send(RequestId.CREATE_ROOM_REQUEST, data, OnCreateRoomResponseReceived);
        }

        private void OnCreateRoomResponseReceived(ISocketPack pack)
        {
            if (pack.s == 0) // 假设0表示成功
            {
                try
                {
                    // 解析房间信息
                    JObject roomData = JObject.Parse((string)pack.d);
                    Dictionary<string, object> roomDict = roomData.ToObject<Dictionary<string, object>>();
                    CurrentRoom = new RoomInfo(roomDict);

                    Debug.Log($"创建房间成功: {CurrentRoom.Name}");

                    // 切换到房间状态
                    GameManager.Instance.ChangeState(GameManager.GameState.Room);

                    // 触发房间加入事件
                    OnRoomJoined?.Invoke(CurrentRoom);
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析房间数据时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"创建房间失败，错误码: {pack.s}");
            }
        }

        // 加入房间
        public void JoinRoom(string roomId)
        {
            // 创建请求数据
            JObject data = new JObject
            {
                ["room_id"] = roomId
            };
            
            // 发送请求
            NetManager.Instance.Send(RequestId.JOIN_ROOM_REQUEST, data, OnJoinRoomResponseReceived);
        }

        private void OnJoinRoomResponseReceived(ISocketPack pack)
        {
            if (pack.s == 0) // 假设0表示成功
            {
                try
                {
                    // 解析房间信息
                    JObject roomData = JObject.Parse((string)pack.d);
                    Dictionary<string, object> roomDict = roomData.ToObject<Dictionary<string, object>>();
                    CurrentRoom = new RoomInfo(roomDict);

                    Debug.Log($"加入房间成功: {CurrentRoom.Name}");

                    // 通知游戏管理器切换状态
                    // GameManager.Instance.OnRoomJoined();

                    // 触发房间加入事件
                    OnRoomJoined?.Invoke(CurrentRoom);
                }
                catch (Exception e)
                {
                    Debug.LogError($"解析房间数据时出错: {e.Message}");
                }
            }
            else
            {
                Debug.LogError($"加入房间失败，错误码: {pack.s}");
            }
        }

        // 离开房间
        public void LeaveRoom()
        {
            // 创建请求数据
            JObject data = new JObject();
            
            // 发送请求
            NetManager.Instance.Send(RequestId.LEAVE_ROOM_REQUEST, data);
            
            CurrentRoom = null;
            OnRoomLeft?.Invoke();
            
            // 通知游戏管理器切换状态
            // GameManager.Instance.OnReturnToLobby();
        }

        // 开始游戏
        public void StartGame()
        {
            if (CurrentRoom == null || !IsRoomHost())
            {
                Debug.LogError("只有房主可以开始游戏");
                return;
            }
            
            // 创建请求数据
            JObject data = new JObject();
            
            // 发送请求
            NetManager.Instance.Send(RequestId.GAME_START_REQUEST, data);
        }

        // 检查是否是房主
        public bool IsRoomHost()
        {
            return CurrentRoom != null && CurrentRoom.HostId == PlayerDataManager.Instance.PlayerData.UserId;
        }
    }

    // 房间信息类
    [System.Serializable]
    public class RoomInfo
    {
        public string RoomId { get; private set; }
        public string Name { get; private set; }
        public string HostId { get; private set; }
        public int MaxPlayers { get; private set; }
        public int PlayerCount { get; private set; }
        public string Status { get; private set; }
        public List<string> Players { get; private set; }

        public RoomInfo(Dictionary<string, object> data)
        {
            UpdateFromData(data);
        }

        public void UpdateFromData(Dictionary<string, object> data)
        {
            RoomId = (string)data["room_id"];
            Name = (string)data["name"];
            HostId = (string)data["host_id"];
            MaxPlayers = Convert.ToInt32(data["max_players"]);
            Status = (string)data["status"];

            if (data.ContainsKey("player_count"))
            {
                PlayerCount = Convert.ToInt32(data["player_count"]);
            }

            if (data.ContainsKey("players") && data["players"] is List<object> playersList)
            {
                Players = new List<string>();
                foreach (var player in playersList)
                {
                    Players.Add((string)player);
                }
                PlayerCount = Players.Count;
            }
        }
    }
}