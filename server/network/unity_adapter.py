import logging
from .message_types import *

class UnityAdapter:
    """Unity客户端适配器，处理Unity客户端特定的消息"""
    
    def __init__(self, server_network):
        self.server_network = server_network
        self.unity_clients = set()  # 存储Unity客户端ID
        
        # 注册消息处理函数
        self.server_network.register_handler(MSG_UNITY_CLIENT_INFO, self.handle_unity_client_info)
        self.server_network.register_handler('connection', self.handle_connection)
        self.server_network.register_handler('disconnection', self.handle_disconnection)
    
    def handle_connection(self, client_id, _):
        """处理客户端连接"""
        # 新客户端连接时，发送欢迎消息
        self.server_network.send_to_client(client_id, MSG_SYSTEM_INFO, {
            "message": "欢迎连接到元气骑士多人游戏服务器",
            "server_version": "1.0.0"
        })
    
    def handle_disconnection(self, client_id, _):
        """处理客户端断开连接"""
        # 如果是Unity客户端，从列表中移除
        if client_id in self.unity_clients:
            self.unity_clients.remove(client_id)
            logging.info(f"Unity客户端断开连接 (ID: {client_id})")
    
    def handle_unity_client_info(self, client_id, data):
        """处理Unity客户端信息"""
        unity_version = data.get('unity_version', 'Unknown')
        device_info = data.get('device_info', {})
        
        # 将客户端标记为Unity客户端
        self.unity_clients.add(client_id)
        
        logging.info(f"Unity客户端连接 (ID: {client_id}), 版本: {unity_version}")
        logging.debug(f"设备信息: {device_info}")
        
        # 可以在这里进行版本兼容性检查
        # 如果版本不兼容，可以发送错误消息并断开连接
        
        # 发送确认消息
        self.server_network.send_to_client(client_id, MSG_SYSTEM_INFO, {
            "message": "Unity客户端信息已接收",
            "compatible": True
        })
    
    def is_unity_client(self, client_id):
        """检查客户端是否为Unity客户端"""
        return client_id in self.unity_clients