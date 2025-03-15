import logging
import json
from typing import Dict, Callable, Any, Coroutine

class MessageHandler:
    """消息处理器，负责注册和分发消息"""
    
    _instance = None
    
    @classmethod
    def get_instance(cls):
        """获取单例实例"""
        if cls._instance is None:
            cls._instance = MessageHandler()
        return cls._instance
    
    def __init__(self):
        """初始化消息处理器"""
        self.handlers: Dict[int, Callable] = {}
        self.server_network = None
    
    def set_network(self, server_network):
        """设置网络模块引用"""
        self.server_network = server_network
    
    def register_handler(self, command_id: int, handler_func: Callable):
        """注册消息处理函数
        
        Args:
            command_id: 命令ID
            handler_func: 处理函数，接收 (client_id, message_data) 两个参数
        """
        self.handlers[command_id] = handler_func
        logging.info(f"注册消息处理器: 命令ID={command_id}, 处理函数={handler_func.__name__}")
    
    async def handle_message(self, client_id: Any, message_data: dict):
        """处理消息
        
        Args:
            client_id: 客户端ID
            message_data: 消息数据
        """
        command_id = message_data.get("c")
        
        if command_id in self.handlers:
            try:
                handler = self.handlers[command_id]
                await handler(client_id, message_data)
            except Exception as e:
                logging.error(f"处理命令ID={command_id}的消息时出错: {str(e)}", exc_info=True)
                # 发送错误响应
                if self.server_network:
                    await self.server_network.send_error(
                        client_id, 
                        message_data.get("o", "0"), 
                        f"处理消息时出错: {str(e)}"
                    )
        else:
            logging.warning(f"未知命令ID: {command_id}")
            # 发送错误响应
            if self.server_network:
                await self.server_network.send_error(
                    client_id, 
                    message_data.get("o", "0"), 
                    f"未知命令: {command_id}"
                )
    
    async def send_response(self, client_id: Any, command_id: int, order_id: str, 
                           status: int = 0, data: Any = None):
        """发送响应消息
        
        Args:
            client_id: 客户端ID
            command_id: 命令ID
            order_id: 订单ID
            status: 状态码，0表示成功
            data: 响应数据
        """
        if not self.server_network:
            logging.error("未设置网络模块，无法发送响应")
            return
        
        response = {
            "c": command_id,
            "o": order_id,
            "s": status,
            "d": data
        }
        
        await self.server_network.send_to_client(client_id, response)