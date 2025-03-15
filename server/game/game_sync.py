import logging
import random
import time
import threading
from .map_generator import MapGenerator

class GameSync:
    """游戏同步管理器，负责处理游戏状态同步"""
    
    def __init__(self, room_manager):
        self.room_manager = room_manager
        self.map_generator = MapGenerator()
        self.game_states = {}  # room_id -> game_state
        self.sync_threads = {}  # room_id -> thread
        self.sync_interval = 0.05  # 同步间隔，单位秒
    
    def start_game(self, room_id):
        """开始游戏"""
        room = self.room_manager.get_room(room_id)
        if not room:
            logging.error(f"开始游戏失败: 房间 {room_id} 不存在")
            return False
        
        # 生成随机种子
        seed = random.randint(1, 1000000)
        
        # 生成地图
        map_data = self.map_generator.generate_map(seed)
        
        # 创建游戏状态
        game_state = {
            "seed": seed,
            "map_data": map_data,
            "players": {},
            "current_frame": 0,
            "frame_inputs": {},  # frame_id -> {client_id: input_data}
            "npc_actions": {},   # frame_id -> [{npc_id, action, params}]
            "start_time": time.time(),
            "is_running": True
        }
        
        # 初始化玩家状态
        for player_id in room["players"]:
            game_state["players"][player_id] = {
                "position": map_data["start_position"].copy(),
                "health": 100,
                "mana": 100,
                "weapons": [],
                "items": []
            }
        
        # 保存游戏状态
        self.game_states[room_id] = game_state
        
        # 启动同步线程
        sync_thread = threading.Thread(target=self.sync_loop, args=(room_id,))
        sync_thread.daemon = True
        sync_thread.start()
        self.sync_threads[room_id] = sync_thread
        
        # 向房间内所有玩家发送游戏开始消息
        from network.message_types import MSG_GAME_START
        self.room_manager.broadcast_to_room(room_id, MSG_GAME_START, {
            "seed": seed,
            "map_data": map_data
        })
        
        logging.info(f"房间 {room_id} 游戏开始")
        return True
    
    def end_game(self, room_id):
        """结束游戏"""
        if room_id not in self.game_states:
            logging.error(f"结束游戏失败: 房间 {room_id} 没有正在进行的游戏")
            return False
        
        # 标记游戏结束
        self.game_states[room_id]["is_running"] = False
        
        # 向房间内所有玩家发送游戏结束消息
        from network.message_types import MSG_GAME_END
        self.room_manager.broadcast_to_room(room_id, MSG_GAME_END, {})
        
        # 清理游戏状态
        del self.game_states[room_id]
        if room_id in self.sync_threads:
            del self.sync_threads[room_id]
        
        logging.info(f"房间 {room_id} 游戏结束")
        return True
    
    def process_player_input(self, room_id, client_id, frame_id, input_data, npc_actions=None):
        """处理玩家输入"""
        if room_id not in self.game_states:
            logging.error(f"处理玩家输入失败: 房间 {room_id} 没有正在进行的游戏")
            return False
        
        game_state = self.game_states[room_id]
        
        # 保存玩家输入
        if frame_id not in game_state["frame_inputs"]:
            game_state["frame_inputs"][frame_id] = {}
        
        game_state["frame_inputs"][frame_id][client_id] = input_data
        
        # 如果是房主，还需要保存NPC行为
        room = self.room_manager.get_room(room_id)
        if room and room["host_id"] == client_id and npc_actions:
            if frame_id not in game_state["npc_actions"]:
                game_state["npc_actions"][frame_id] = []
            
            game_state["npc_actions"][frame_id].extend(npc_actions)
        
        return True
    
    def sync_loop(self, room_id):
        """同步循环"""
        try:
            while room_id in self.game_states and self.game_states[room_id]["is_running"]:
                self.sync_game_state(room_id)
                time.sleep(self.sync_interval)
        except Exception as e:
            logging.error(f"游戏同步错误 (房间 {room_id}): {e}")
            self.end_game(room_id)
    
    def sync_game_state(self, room_id):
        """同步游戏状态"""
        if room_id not in self.game_states:
            return
        
        game_state = self.game_states[room_id]
        current_frame = game_state["current_frame"]
        
        # 收集当前帧的所有输入
        frame_inputs = game_state["frame_inputs"].get(current_frame, {})
        npc_actions = game_state["npc_actions"].get(current_frame, [])
        
        # 向房间内所有玩家广播当前帧的输入和NPC行为
        from network.message_types import MSG_GAME_PLAYER_INPUT
        self.room_manager.broadcast_to_room(room_id, MSG_GAME_PLAYER_INPUT, {
            "frame_id": current_frame,
            "frame_data": {
                "inputs": frame_inputs,
                "npc_actions": npc_actions
            }
        })
        
        # 更新帧计数
        game_state["current_frame"] += 1
        
        # 清理旧帧数据（保留最近100帧）
        old_frames = [f for f in game_state["frame_inputs"].keys() if f < current_frame - 100]
        for frame in old_frames:
            if frame in game_state["frame_inputs"]:
                del game_state["frame_inputs"][frame]
            if frame in game_state["npc_actions"]:
                del game_state["npc_actions"][frame]