import logging
import argparse
import signal
import sys
import time
import os
from network.server_network import ServerNetwork
from database.mysql_manager import MySQLManager
from player.player_manager import PlayerManager
from room.room_manager import RoomManager

def setup_logging(log_level):
    """设置日志"""
    numeric_level = getattr(logging, log_level.upper(), None)
    if not isinstance(numeric_level, int):
        raise ValueError(f'无效的日志级别: {log_level}')
    
    # 确保日志目录存在
    os.makedirs('logs', exist_ok=True)
    
    logging.basicConfig(
        level=numeric_level,
        format='%(asctime)s [%(levelname)s] %(message)s',
        handlers=[
            logging.StreamHandler(),
            logging.FileHandler('logs/server.log', encoding='utf-8')
        ]
    )

def parse_args():
    """解析命令行参数"""
    parser = argparse.ArgumentParser(description='魂斗骑士服务器')
    parser.add_argument('--host', default='0.0.0.0', help='服务器主机地址')
    parser.add_argument('--port', type=int, default=5555, help='服务器端口')
    parser.add_argument('--log-level', default='INFO', choices=['DEBUG', 'INFO', 'WARNING', 'ERROR', 'CRITICAL'], help='日志级别')
    
    # MySQL数据库配置
    parser.add_argument('--db-host', default='localhost', help='MySQL主机地址')
    parser.add_argument('--db-user', default='root', help='MySQL用户名')
    parser.add_argument('--db-password', default='', help='MySQL密码')
    parser.add_argument('--db-name', default='soulknight', help='MySQL数据库名')
    
    return parser.parse_args()

def main():
    """主函数"""
    # 解析命令行参数
    args = parse_args()
    
    # 设置日志
    setup_logging(args.log_level)
    
    # 创建MySQL数据库管理器
    db_manager = MySQLManager(
        host=args.db_host,
        user=args.db_user,
        password=args.db_password,
        database=args.db_name
    )
    
    # 创建服务器网络
    server_network = ServerNetwork(args.host, args.port)
    
    # 创建玩家管理器
    player_manager = PlayerManager(server_network, db_manager)
    
    # 创建房间管理器
    room_manager = RoomManager(server_network, player_manager)
    
    # 注册信号处理
    def signal_handler(sig, frame):
        logging.info("正在关闭服务器...")
        server_network.stop()
        sys.exit(0)
    
    signal.signal(signal.SIGINT, signal_handler)
    signal.signal(signal.SIGTERM, signal_handler)
    
    # 启动服务器
    if server_network.start():
        logging.info(f"服务器已启动，监听地址: {args.host}:{args.port}")
        
        try:
            # 保持主线程运行
            while True:
                time.sleep(1)
        except KeyboardInterrupt:
            logging.info("正在关闭服务器...")
            server_network.stop()
    else:
        logging.error("服务器启动失败")

if __name__ == "__main__":
    main()