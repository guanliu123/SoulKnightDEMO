import logging
import json
import asyncio
from typing import Dict, Any

class Player:
    def __init__(self, client_id, player_id=None, username=None):
        self.client_id = client_id
        self.player_id = player_id
        self.username = username
        self.room_id = None
        self.is_ready = False
        self.character_id = None
        self.position = (0, 0)
        self.health = 100
        self.last_activity = 0  # 最后活动时间戳

class PlayerManager:
    def __init__(self, server_network, db_manager):
        self.server_network = server_network
        self.db_manager = db_manager
        self.players = {}  # {client_id: Player}
        
        # 注册消息处理器
        self.register_handlers()
    
    def register_handlers(self):
        """注册消息处理器"""
        self.server_network.register_handler(1001, self.handle_login)  # 登录请求
        self.server_network.register_handler(1002, self.handle_logout)  # 登出请求
        self.server_network.register_handler(1003, self.handle_player_update)  # 玩家更新
        self.server_network.register_handler(1000, self.handle_heartbeat)  # 心跳包
    
    async def handle_login(self, client_id, data):
        """处理登录请求"""
        try:
            params = data.get("p", {})
            username = params.get("username", "")
            password = params.get("password", "")
            
            logging.info(f"玩家登录请求: {username}")
            
            # 这里应该有数据库验证逻辑
            # 简化处理，直接创建玩家
            player_id = f"player_{client_id}"
            
            # 创建玩家对象
            player = Player(client_id, player_id, username)
            self.players[client_id] = player
            
            # 发送登录成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0,  # 成功状态
                "d": {
                    "playerId": player_id,
                    "username": username,
                    "token": f"token_{client_id}"
                }
            }
            
            await self.server_network.send_to_client(client_id, response)
            logging.info(f"玩家 {username} 登录成功")
            
        except Exception as e:
            logging.error(f"处理登录请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"登录失败: {str(e)}")
    
    async def handle_logout(self, client_id, data):
        """处理登出请求"""
        if client_id in self.players:
            player = self.players[client_id]
            logging.info(f"玩家 {player.username} 登出")
            
            # 如果玩家在房间中，需要处理离开房间逻辑
            if player.room_id:
                # 这里应该调用RoomManager的方法处理玩家离开房间
                pass
            
            # 从玩家列表中移除
            del self.players[client_id]
            
            # 发送登出成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0  # 成功状态
            }
            
            await self.server_network.send_to_client(client_id, response)
    
    async def handle_player_update(self, client_id, data):
        """处理玩家更新请求"""
        if client_id in self.players:
            player = self.players[client_id]
            params = data.get("p", {})
            
            # 更新玩家信息
            if "position" in params:
                player.position = params["position"]
            
            if "health" in params:
                player.health = params["health"]
            
            if "characterId" in params:
                player.character_id = params["characterId"]
            
            # 发送更新成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0  # 成功状态
            }
            
            await self.server_network.send_to_client(client_id, response)
    
    async def handle_heartbeat(self, client_id, data):
        """处理心跳包"""
        # 更新玩家最后活动时间
        if client_id in self.players:
            player = self.players[client_id]
            player.last_activity = data.get("t", 0)  # 时间戳
        
        # 发送心跳响应
        response = {
            "c": data["c"],
            "o": data["o"],
            "s": 0,  # 成功状态
            "p": {}  # 空参数
        }
        
        await self.server_network.send_to_client(client_id, response)
