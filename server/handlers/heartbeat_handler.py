import logging
import asyncio
from network.message_handler import MessageHandler

# 心跳包命令ID
HEARTBEAT_COMMAND_ID = 1000

async def handle_heartbeat(client_id, message_data):
    """处理心跳包"""
    logging.debug(f"处理心跳包: {message_data}")
    
    # 获取消息处理器实例
    message_handler = MessageHandler.get_instance()
    
    # 发送心跳响应
    await message_handler.send_response(
        client_id=client_id,
        command_id=HEARTBEAT_COMMAND_ID,
        order_id=message_data.get("o", "0"),
        status=0,
        data=None
    )
    
    logging.debug("发送心跳响应")

def register_handlers():
    """注册心跳包处理函数"""
    message_handler = MessageHandler.get_instance()
    message_handler.register_handler(HEARTBEAT_COMMAND_ID, handle_heartbeat)