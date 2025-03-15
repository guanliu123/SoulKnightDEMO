import sys
import os
import logging
import argparse
import signal
import datetime
import time

# 添加当前目录到模块搜索路径
sys.path.append(os.path.dirname(os.path.abspath(__file__)))

from network.server_network import ServerNetwork
from network.message_handler import MessageHandler
from handlers.heartbeat_handler import register_handlers as register_heartbeat_handlers
from handlers.login_handler import register_handlers as register_login_handlers

def setup_logging(log_level, log_prefix="logs/server"):
    """设置日志"""
    numeric_level = getattr(logging, log_level.upper(), None)
    if not isinstance(numeric_level, int):
        raise ValueError(f'无效的日志级别: {log_level}')
    
    # 生成带日期的日志文件名
    today = datetime.datetime.now().strftime("%Y%m%d")
    log_file = f"{log_prefix}_{today}.log"
    
    # 确保日志目录存在
    os.makedirs(os.path.dirname(log_file), exist_ok=True)
    
    logging.basicConfig(
        level=numeric_level,
        format='%(asctime)s [%(levelname)s] %(message)s',
        handlers=[
            logging.StreamHandler(),
            logging.FileHandler(log_file, encoding='utf-8')
        ]
    )

def signal_handler(sig, frame):
    """处理信号"""
    logging.info("接收到终止信号，正在关闭服务器...")
    sys.exit(0)

def main():
    """主函数"""
    # 解析命令行参数
    parser = argparse.ArgumentParser(description='启动游戏服务器')
    parser.add_argument('--host', default='127.0.0.1', help='服务器监听地址')
    parser.add_argument('--port', type=int, default=5555, help='服务器监听端口')
    parser.add_argument('--log-level', default='INFO', choices=['DEBUG', 'INFO', 'WARNING', 'ERROR', 'CRITICAL'],
                        help='日志级别')
    args = parser.parse_args()
    
    # 设置日志
    setup_logging(args.log_level)
    
    # 注册信号处理器
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    # 创建消息处理器
    message_handler = MessageHandler.get_instance()
    
    # 注册消息处理函数
    register_heartbeat_handlers()
    register_login_handlers()
    
    # 创建并启动网络服务器
    server = ServerNetwork(args.host, args.port)
    server.set_message_handler(message_handler)
    
    if server.start():
        logging.info(f"服务器已启动，监听地址: {args.host}:{args.port}")
        
        try:
            # 保持主线程运行（Windows兼容方式）
            while True:
                time.sleep(1)  # 每秒检查一次
        except (KeyboardInterrupt, SystemExit):
            logging.info("正在关闭服务器...")
        finally:
            server.stop()
            logging.info("服务器已关闭")
    else:
        logging.error("服务器启动失败")

if __name__ == "__main__":
    main()