import logging
import json
import asyncio
import time
import uuid
from typing import Dict, List, Any

class Room:
    def __init__(self, room_id, name, max_players=4, owner_id=None):
        self.room_id = room_id
        self.name = name
        self.max_players = max_players
        self.owner_id = owner_id
        self.players = []  # 房间内的玩家ID列表
        self.is_game_started = False
        self.create_time = time.time()
    
    def add_player(self, player_id):
        """添加玩家到房间"""
        if len(self.players) < self.max_players and player_id not in self.players:
            self.players.append(player_id)
            return True
        return False
    
    def remove_player(self, player_id):
        """从房间移除玩家"""
        if player_id in self.players:
            self.players.remove(player_id)
            
            # 如果房主离开，选择新房主
            if player_id == self.owner_id and self.players:
                self.owner_id = self.players[0]
            
            return True
        return False
    
    def to_dict(self):
        """转换为字典格式"""
        return {
            "roomId": self.room_id,
            "name": self.name,
            "maxPlayers": self.max_players,
            "currentPlayers": len(self.players),
            "ownerId": self.owner_id,
            "isGameStarted": self.is_game_started
        }

class RoomManager:
    def __init__(self, server_network, player_manager):
        self.server_network = server_network
        self.player_manager = player_manager
        self.rooms = {}  # {room_id: Room}
        
        # 注册消息处理器
        self.register_handlers()
    
    def register_handlers(self):
        """注册消息处理器"""
        self.server_network.register_handler(2001, self.handle_create_room)  # 创建房间
        self.server_network.register_handler(2002, self.handle_join_room)    # 加入房间
        self.server_network.register_handler(2003, self.handle_leave_room)   # 离开房间
        self.server_network.register_handler(2004, self.handle_list_rooms)   # 列出房间
        self.server_network.register_handler(2005, self.handle_start_game)   # 开始游戏
    
    async def handle_create_room(self, client_id, data):
        """处理创建房间请求"""
        try:
            if client_id not in self.player_manager.players:
                await self.server_network.send_error(client_id, data["o"], "玩家未登录")
                return
            
            params = data.get("p", {})
            room_name = params.get("name", f"房间 {len(self.rooms) + 1}")
            max_players = params.get("maxPlayers", 4)
            
            player = self.player_manager.players[client_id]
            
            # 检查玩家是否已经在房间中
            if player.room_id:
                await self.server_network.send_error(client_id, data["o"], "玩家已经在房间中")
                return
            
            # 创建新房间
            room_id = str(uuid.uuid4())
            room = Room(room_id, room_name, max_players, player.player_id)
            self.rooms[room_id] = room
            
            # 将玩家加入房间
            room.add_player(player.player_id)
            player.room_id = room_id
            
            # 发送创建成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0,  # 成功状态
                "d": room.to_dict()
            }
            
            await self.server_network.send_to_client(client_id, response)
            logging.info(f"玩家 {player.username} 创建了房间 {room_name}")
            
        except Exception as e:
            logging.error(f"处理创建房间请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"创建房间失败: {str(e)}")
    
    async def handle_join_room(self, client_id, data):
        """处理加入房间请求"""
        try:
            if client_id not in self.player_manager.players:
                await self.server_network.send_error(client_id, data["o"], "玩家未登录")
                return
            
            params = data.get("p", {})
            room_id = params.get("roomId")
            
            if not room_id or room_id not in self.rooms:
                await self.server_network.send_error(client_id, data["o"], "房间不存在")
                return
            
            player = self.player_manager.players[client_id]
            
            # 检查玩家是否已经在房间中
            if player.room_id:
                if player.room_id == room_id:
                    await self.server_network.send_error(client_id, data["o"], "玩家已经在该房间中")
                else:
                    await self.server_network.send_error(client_id, data["o"], "玩家已经在其他房间中")
                return
            
            room = self.rooms[room_id]
            
            # 检查房间是否已满
            if len(room.players) >= room.max_players:
                await self.server_network.send_error(client_id, data["o"], "房间已满")
                return
            
            # 检查游戏是否已经开始
            if room.is_game_started:
                await self.server_network.send_error(client_id, data["o"], "游戏已经开始")
                return
            
            # 将玩家加入房间
            room.add_player(player.player_id)
            player.room_id = room_id
            
            # 发送加入成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0,  # 成功状态
                "d": room.to_dict()
            }
            
            await self.server_network.send_to_client(client_id, response)
            
            # 通知房间内其他玩家
            for player_id in room.players:
                if player_id != player.player_id:
                    for pid, p in self.player_manager.players.items():
                        if p.player_id == player_id:
                            notify = {
                                "c": 2010,  # 玩家加入房间通知
                                "s": 0,
                                "d": {
                                    "roomId": room_id,
                                    "playerId": player.player_id,
                                    "username": player.username
                                }
                            }
                            await self.server_network.send_to_client(pid, notify)
            
            logging.info(f"玩家 {player.username} 加入了房间 {room.name}")
            
        except Exception as e:
            logging.error(f"处理加入房间请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"加入房间失败: {str(e)}")
    
    async def handle_leave_room(self, client_id, data):
        """处理离开房间请求"""
        try:
            if client_id not in self.player_manager.players:
                await self.server_network.send_error(client_id, data["o"], "玩家未登录")
                return
            
            player = self.player_manager.players[client_id]
            
            # 检查玩家是否在房间中
            if not player.room_id or player.room_id not in self.rooms:
                await self.server_network.send_error(client_id, data["o"], "玩家不在房间中")
                return
            
            room_id = player.room_id
            room = self.rooms[room_id]
            
            # 将玩家从房间移除
            room.remove_player(player.player_id)
            old_room_id = player.room_id
            player.room_id = None
            
            # 发送离开成功响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0  # 成功状态
            }
            
            await self.server_network.send_to_client(client_id, response)
            
            # 通知房间内其他玩家
            for player_id in room.players:
                for pid, p in self.player_manager.players.items():
                    if p.player_id == player_id:
                        notify = {
                            "c": 2011,  # 玩家离开房间通知
                            "s": 0,
                            "d": {
                                "roomId": room_id,
                                "playerId": player.player_id,
                                "username": player.username
                            }
                        }
                        await self.server_network.send_to_client(pid, notify)
            
            # 如果房间空了，删除房间
            if not room.players:
                del self.rooms[room_id]
                logging.info(f"房间 {room.name} 已被删除")
            
            logging.info(f"玩家 {player.username} 离开了房间 {room.name}")
            
        except Exception as e:
            logging.error(f"处理离开房间请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"离开房间失败: {str(e)}")
    
    async def handle_list_rooms(self, client_id, data):
        """处理列出房间请求"""
        try:
            # 获取所有未开始游戏的房间
            available_rooms = [room.to_dict() for room_id, room in self.rooms.items() 
                              if not room.is_game_started and len(room.players) < room.max_players]
            
            # 发送房间列表响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0,  # 成功状态
                "d": {
                    "rooms": available_rooms
                }
            }
            
            await self.server_network.send_to_client(client_id, response)
            
        except Exception as e:
            logging.error(f"处理列出房间请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"获取房间列表失败: {str(e)}")
    
    async def handle_start_game(self, client_id, data):
        """处理开始游戏请求"""
        try:
            if client_id not in self.player_manager.players:
                await self.server_network.send_error(client_id, data["o"], "玩家未登录")
                return
            
            player = self.player_manager.players[client_id]
            
            # 检查玩家是否在房间中
            if not player.room_id or player.room_id not in self.rooms:
                await self.server_network.send_error(client_id, data["o"], "玩家不在房间中")
                return
            
            room = self.rooms[player.room_id]
            
            # 检查是否是房主
            if room.owner_id != player.player_id:
                await self.server_network.send_error(client_id, data["o"], "只有房主可以开始游戏")
                return
            
            # 检查房间人数
            if len(room.players) < 1:  # 可以根据需要调整最小人数
                await self.server_network.send_error(client_id, data["o"], "房间人数不足，无法开始游戏")
                return
            
            # 标记游戏已开始
            room.is_game_started = True
            
            # 发送开始游戏响应
            response = {
                "c": data["c"],
                "o": data["o"],
                "s": 0,  # 成功状态
                "d": {
                    "roomId": room.room_id,
                    "players": []
                }
            }
            
            # 收集房间内所有玩家信息
            for player_id in room.players:
                for pid, p in self.player_manager.players.items():
                    if p.player_id == player_id:
                        response["d"]["players"].append({
                            "playerId": p.player_id,
                            "username": p.username,
                            "characterId": p.character_id
                        })
            
            # 通知房间内所有玩家游戏开始
            for player_id in room.players:
                for pid, p in self.player_manager.players.items():
                    if p.player_id == player_id:
                        await self.server_network.send_to_client(pid, response)
            
            logging.info(f"房间 {room.name} 开始游戏，玩家数: {len(room.players)}")
            
        except Exception as e:
            logging.error(f"处理开始游戏请求时出错: {str(e)}", exc_info=True)
            await self.server_network.send_error(client_id, data["o"], f"开始游戏失败: {str(e)}")
    
    def get_room_by_player(self, player_id):
        """根据玩家ID获取房间"""
        for room_id, room in self.rooms.items():
            if player_id in room.players:
                return room
        return None
    
    async def broadcast_to_room(self, room_id, message, exclude_player_id=None):
        """向房间内所有玩家广播消息"""
        if room_id not in self.rooms:
            return
        
        room = self.rooms[room_id]
        for player_id in room.players:
            if exclude_player_id and player_id == exclude_player_id:
                continue
                
            for pid, player in self.player_manager.players.items():
                if player.player_id == player_id:
                    await self.server_network.send_to_client(pid, message)