import logging
import time
from ..network.message_types import *

class Role:
    """
    角色类，表示一个在线玩家
    """
    
    def __init__(self, user_id, username, server_network, client_id, db_manager):
        self.user_id = user_id
        self.username = username
        self.server_network = server_network
        self.client_id = client_id
        self.db_manager = db_manager
        
        # 玩家数据
        self.int_data = {}  # 整数数据，如金币、等级等
        self.obj_data = {}  # 对象数据，如装备、角色等
        
        # 状态信息
        self.login_time = time.time()
        self.last_active_time = time.time()
        self.room_id = None
        
        # 加载玩家数据
        self.load_data()
    
    def load_data(self):
        """加载玩家数据"""
        try:
            # 从数据库加载玩家数据
            player_data = self.db_manager.get_player_data(self.user_id)
            
            if player_data:
                # 加载整数数据
                if 'int_data' in player_data:
                    self.int_data = player_data['int_data']
                
                # 加载对象数据
                if 'obj_data' in player_data:
                    self.obj_data = player_data['obj_data']
            
            # 初始化默认数据
            self._init_default_data()
            
            logging.debug(f"已加载玩家 {self.username} 的数据")
            return True
        
        except Exception as e:
            logging.error(f"加载玩家 {self.username} 数据失败: {e}")
            return False
    
    def _init_default_data(self):
        """初始化默认数据"""
        # 确保基础整数数据存在
        if 'coin' not in self.int_data:
            self.int_data['coin'] = 0
        
        if 'gem' not in self.int_data:
            self.int_data['gem'] = 0
        
        if 'level' not in self.int_data:
            self.int_data['level'] = 1
        
        if 'exp' not in self.int_data:
            self.int_data['exp'] = 0
        
        # 确保基础对象数据类别存在
        for category in ['characters', 'weapons', 'items', 'achievements']:
            if category not in self.obj_data:
                self.obj_data[category] = {}
        
        # 确保至少有一个初始角色
        if not self.obj_data['characters']:
            self.obj_data['characters']['knight'] = {
                'unlocked': True,
                'level': 1,
                'skin': 'default'
            }
    
    def save_data(self):
        """保存玩家数据到数据库"""
        try:
            # 更新最后活动时间
            self.last_active_time = time.time()
            
            # 准备保存的数据
            player_data = {
                'int_data': self.int_data,
                'obj_data': self.obj_data,
                'last_active_time': self.last_active_time
            }
            
            # 保存到数据库
            self.db_manager.save_player_data(self.user_id, player_data)
            
            logging.debug(f"已保存玩家 {self.username} 的数据")
            return True
        
        except Exception as e:
            logging.error(f"保存玩家 {self.username} 数据失败: {e}")
            return False
    
    def send_player_data(self):
        """发送玩家数据到客户端"""
        self.send_obj(MSG_PLAYER_DATA_RESPONSE, {
            'success': True,
            'data': {
                'int_dict': self.int_data,
                'obj_dict': self.obj_data
            }
        })
    
    def update_int_data(self, key, value):
        """更新整数数据"""
        self.int_data[key] = value
        self.last_active_time = time.time()
    
    def update_obj_data(self, category, obj_id, obj_data):
        """更新对象数据"""
        if category not in self.obj_data:
            self.obj_data[category] = {}
        
        self.obj_data[category][obj_id] = obj_data
        self.last_active_time = time.time()
    
    def send_obj(self, msg_type, msg_data):
        """发送消息到客户端"""
        self.server_network.send_to_client(self.client_id, msg_type, msg_data)
        self.last_active_time = time.time()
    
    def disconnect(self):
        """处理玩家断开连接"""
        # 保存玩家数据
        self.save_data()
        
        # 如果在房间中，通知房间管理器
        if self.room_id:
            # 这里需要通过事件系统或回调通知房间管理器
            pass
    
    def join_room(self, room_id):
        """加入房间"""
        self.room_id = room_id
        self.last_active_time = time.time()
    
    def leave_room(self):
        """离开房间"""
        self.room_id = None
        self.last_active_time = time.time()
    
    def to_dict(self, include_private=False):
        """转换为字典表示"""
        data = {
            'user_id': self.user_id,
            'username': self.username,
            'room_id': self.room_id,
            'level': self.int_data.get('level', 1)
        }
        
        if include_private:
            data.update({
                'login_time': self.login_time,
                'last_active_time': self.last_active_time,
                'int_data': self.int_data,
                'obj_data': self.obj_data
            })
        
        return data