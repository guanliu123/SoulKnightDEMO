import time
import logging

class Player:
    """玩家类，表示一个在线玩家"""
    
    def __init__(self, user_id, username, client_id, server_network):
        self.user_id = user_id
        self.username = username
        self.client_id = client_id
        self.server_network = server_network
        
        # 玩家数据
        self.int_dict = {}  # 整数数据，如金币、等级等
        self.obj_dict = {}  # 对象数据，如角色、武器等
        
        # 玩家状态
        self.room_id = None
        self.login_time = time.time()
        self.last_active_time = self.login_time
        
        # 初始化默认数据
        self._init_default_data()
    
    def _init_default_data(self):
        """初始化默认数据"""
        # 默认整数数据
        self.int_dict = {
            "level": 1,
            "exp": 0,
            "coins": 100,
            "gems": 10,
            "energy": 100,
            "max_energy": 100
        }
        
        # 默认对象数据
        self.obj_dict = {
            "characters": {
                "knight": {
                    "unlocked": True,
                    "level": 1,
                    "skins": ["default"]
                }
            },
            "weapons": {},
            "items": {}
        }
    
    def load_data(self, int_dict=None, obj_dict=None):
        """加载玩家数据"""
        if int_dict:
            self.int_dict.update(int_dict)
        
        if obj_dict:
            # 合并对象数据
            for category, items in obj_dict.items():
                if category not in self.obj_dict:
                    self.obj_dict[category] = {}
                
                self.obj_dict[category].update(items)
    
    def get_basic_info(self):
        """获取玩家基本信息"""
        return {
            "user_id": self.user_id,
            "username": self.username,
            "level": self.int_dict.get("level", 1)
        }
    
    def update_activity(self):
        """更新最后活动时间"""
        self.last_active_time = time.time()
    
    def join_room(self, room_id):
        """加入房间"""
        self.room_id = room_id
        self.update_activity()
    
    def leave_room(self):
        """离开房间"""
        self.room_id = None
        self.update_activity()
    
    def send_message(self, msg_type, msg_data):
        """发送消息给玩家"""
        result = self.server_network.send_to_client(self.client_id, msg_type, msg_data)
        self.update_activity()
        return result
    
    def to_dict(self, include_private=False):
        """转换为字典表示"""
        data = {
            'user_id': self.user_id,
            'username': self.username,
            'room_id': self.room_id
        }
        
        if include_private:
            data.update({
                'login_time': self.login_time,
                'last_active_time': self.last_active_time,
                'int_dict': self.int_dict,
                'obj_dict': self.obj_dict
            })
        
        return data