import time
import random
import logging
from ..game.map_generator import MapGenerator

class Room:
    """游戏房间类，管理单个房间的状态和玩家"""
    
    def __init__(self, room_id, name, host_id, max_players=4):
        self.room_id = room_id
        self.name = name
        self.host_id = host_id
        self.max_players = max_players
        self.players = [host_id]  # 房间内的玩家ID列表
        self.status = "waiting"  # waiting, playing
        self.create_time = time.time()
        
        # 游戏相关
        self.game_seed = None
        self.map_data = None
        self.current_frame = 0
        self.frame_inputs = {}  # frame_id -> {client_id: input_data}
        self.npc_actions = {}   # frame_id -> [{npc_id, action, params}]
        
        # 加入请求和邀请
        self.join_requests = {}  # client_id -> {username, request_time}
        self.invitations = {}   # username -> {inviter_id, invite_time}
        
        # 地图生成器
        self.map_generator = MapGenerator()
    
    def add_player(self, client_id):
        """添加玩家到房间"""
        if client_id not in self.players and len(self.players) < self.max_players:
            self.players.append(client_id)
            return True
        return False
    
    def remove_player(self, client_id):
        """从房间移除玩家"""
        if client_id in self.players:
            self.players.remove(client_id)
            
            # 如果移除的是房主且房间还有其他玩家，选择新房主
            if client_id == self.host_id and self.players:
                self.host_id = self.players[0]
            
            return True
        return False
    
    def is_full(self):
        """检查房间是否已满"""
        return len(self.players) >= self.max_players
    
    def is_empty(self):
        """检查房间是否为空"""
        return len(self.players) == 0
    
    def add_join_request(self, client_id, username):
        """添加加入请求"""
        self.join_requests[client_id] = {
            "username": username,
            "request_time": time.time()
        }
    
    def remove_join_request(self, client_id):
        """移除加入请求"""
        if client_id in self.join_requests:
            del self.join_requests[client_id]
    
    def add_invitation(self, username, inviter_id):
        """添加邀请"""
        self.invitations[username] = {
            "inviter_id": inviter_id,
            "invite_time": time.time()
        }
    
    def remove_invitation(self, username):
        """移除邀请"""
        if username in self.invitations:
            del self.invitations[username]
    
    def start_game(self):
        """开始游戏"""
        if self.status != "waiting":
            return False
        
        # 生成随机种子
        self.game_seed = random.randint(1, 1000000)
        
        # 生成地图
        self.map_data = self.map_generator.generate_map(self.game_seed)
        
        # 重置游戏状态
        self.current_frame = 0
        self.frame_inputs = {}
        self.npc_actions = {}
        
        # 更新房间状态
        self.status = "playing"
        
        logging.info(f"房间 {self.room_id} 开始游戏，种子: {self.game_seed}")
        return True
    
    def end_game(self):
        """结束游戏"""
        if self.status != "playing":
            return False
        
        # 更新房间状态
        self.status = "waiting"
        
        logging.info(f"房间 {self.room_id} 结束游戏")
        return True
    
    def reset_game(self):
        """重置游戏状态"""
        self.game_seed = None
        self.map_data = None
        self.current_frame = 0
        self.frame_inputs = {}
        self.npc_actions = {}
    
    def add_player_input(self, client_id, frame_id, input_data):
        """添加玩家输入"""
        if frame_id not in self.frame_inputs:
            self.frame_inputs[frame_id] = {}
        
        self.frame_inputs[frame_id][client_id] = input_data
    
    def add_npc_action(self, frame_id, npc_id, action, params):
        """添加NPC行为"""
        if frame_id not in self.npc_actions:
            self.npc_actions[frame_id] = []
        
        self.npc_actions[frame_id].append({
            "npc_id": npc_id,
            "action": action,
            "params": params
        })
    
    def get_frame_data(self, frame_id):
        """获取帧数据"""
        return {
            "inputs": self.frame_inputs.get(frame_id, {}),
            "npc_actions": self.npc_actions.get(frame_id, [])
        }
    
    def to_dict(self, include_details=False):
        """转换为字典表示"""
        room_dict = {
            "room_id": self.room_id,
            "name": self.name,
            "host_id": self.host_id,
            "max_players": self.max_players,
            "player_count": len(self.players),
            "status": self.status,
            "create_time": self.create_time
        }
        
        if include_details:
            room_dict.update({
                "players": self.players,
                "join_requests": self.join_requests,
                "invitations": self.invitations
            })
            
            if self.status == "playing":
                room_dict.update({
                    "game_seed": self.game_seed,
                    "current_frame": self.current_frame
                })
        
        return room_dict