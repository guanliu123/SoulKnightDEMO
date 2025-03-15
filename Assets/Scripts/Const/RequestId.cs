using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RequestId
{
    /**
     * 不受服务器消息队列限制的心跳
     */
    NO_QUEUE_HEART = 0,
    /**
     * 登录
     */
    LOGIN = 1,
    /**
     * 登录完成后通知后端的消息
     */
    LOGIN_OVER = 82,
    
    /**
     * 注册请求
     */
    REGISTER_REQUEST = 102,
    
    /**
     * 登出请求
     */
    LOGOUT = 104,
    
    /**
     * 创建房间请求
     */
    CREATE_ROOM_REQUEST = 200,
    
    /**
     * 加入房间请求
     */
    JOIN_ROOM_REQUEST = 201,
    
    /**
     * 离开房间请求
     */
    LEAVE_ROOM_REQUEST = 202,
    
    /**
     * 开始游戏请求
     */
    GAME_START_REQUEST = 203,
    
    /**
     * 玩家输入请求
     */
    GAME_PLAYER_INPUT = 204,
    
    /**
     * 请求玩家数据
     */
    PLAYER_DATA_REQUEST = 300,
    
    /**
     * 请求地图数据
     */
    MAP_DATA_REQUEST = 301
}
