import logging
import json
import time
import random
from network.message_handler import MessageHandler

# 登录相关命令ID
LOGIN_COMMAND_ID = 1
LOGIN_OVER_COMMAND_ID = 2

async def handle_login(client_id, message_data):
    """处理登录请求"""
    logging.info(f"处理登录请求: {message_data}")
    
    # 获取消息处理器实例
    message_handler = MessageHandler.get_instance()
    
    # 解析登录参数
    params = message_data.get("p", {})
    username = params.get("username", "unknown")
    # TODO 检查数据库 
    
    # 创建用户数据
    user_id = f"user_{random.randint(10000, 99999)}"
    server_time = int(time.time() * 1000)  # 毫秒时间戳
    
    # 创建登录响应数据
    login_data = {
        "gameUid": user_id,
        "username": username,
        "serverTime": server_time,
        "isRegister": False,  # 或根据实际情况设置
        "token": f"token_{int(time.time())}"
    }
    
    # 发送登录响应
    await message_handler.send_response(
        client_id=client_id,
        command_id=LOGIN_COMMAND_ID,
        order_id=message_data.get("o", "0"),
        status=0,
        data=json.dumps(login_data)  # 注意：客户端期望的是JSON字符串
    )
    
    logging.info(f"发送登录响应: {login_data}")

async def handle_login_over(client_id, message_data):
    """处理登录完成请求"""
    logging.info(f"处理登录完成请求: {message_data}")
    
    # 获取消息处理器实例
    message_handler = MessageHandler.get_instance()
    
    # 发送登录完成响应
    await message_handler.send_response(
        client_id=client_id,
        command_id=LOGIN_OVER_COMMAND_ID,
        order_id=message_data.get("o", "0"),
        status=0,
        data=None
    )
    
    logging.info("发送登录完成响应")

def register_handlers():
    """注册登录相关处理函数"""
    message_handler = MessageHandler.get_instance()
    message_handler.register_handler(LOGIN_COMMAND_ID, handle_login)
    message_handler.register_handler(LOGIN_OVER_COMMAND_ID, handle_login_over)