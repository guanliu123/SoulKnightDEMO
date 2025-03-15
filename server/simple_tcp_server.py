import socket
import threading
import json
import logging
import time

# 配置日志
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)

# 存储所有连接的客户端
connected_clients = {}

def handle_client(client_socket, client_address, client_id):
    """处理客户端连接"""
    try:
        # 保存连接
        connected_clients[client_id] = client_socket
        logging.info(f"新客户端连接: {client_address}, ID: {client_id}")
        print(f"新客户端连接: {client_address}, ID: {client_id}")
        
        # 发送欢迎消息
        welcome_msg = {
            'c': 0,  # 心跳消息类型
            's': 0,
            'd': "Welcome to SoulKnight Simple TCP Server!"
        }
        send_message(client_socket, welcome_msg)
        
        # 持续接收消息
        buffer = b''
        while True:
            # 接收数据
            chunk = client_socket.recv(4096)
            if not chunk:
                # 连接已关闭
                break
            
            buffer += chunk
            
            # 处理完整消息
            while len(buffer) >= 4:  # 至少包含消息长度字段
                # 解析消息长度
                msg_length = int.from_bytes(buffer[:4], byteorder='big')
                
                # 检查是否收到完整消息
                if len(buffer) >= 4 + msg_length:
                    # 提取消息内容
                    message = buffer[4:4+msg_length].decode('utf-8')
                    buffer = buffer[4+msg_length:]
                    
                    logging.info(f"收到消息: {message}, 来自: {client_id}")
                    print(f"收到消息: {message}, 来自: {client_id}")
                    
                    # 尝试解析JSON消息
                    try:
                        data = json.loads(message)
                        msg_type = data.get('c', None)
                        
                        # 如果是心跳消息，回复相同的消息
                        if msg_type == 0:  # 假设0是心跳消息类型
                            response = {
                                'c': 0,
                                's': 0,
                                'o': data.get('o', None),
                                'd': "Heartbeat received"
                            }
                            send_message(client_socket, response)
                            logging.info(f"发送心跳响应给: {client_id}")
                            print(f"发送心跳响应给: {client_id}")
                    except json.JSONDecodeError:
                        logging.warning(f"收到非JSON格式消息: {message}")
                        print(f"收到非JSON格式消息: {message}")
                else:
                    # 消息不完整，等待更多数据
                    break
    
    except ConnectionResetError:
        logging.info(f"客户端连接重置: {client_id}")
        print(f"客户端连接重置: {client_id}")
    except Exception as e:
        logging.error(f"处理客户端时发生错误: {e}")
        print(f"处理客户端时发生错误: {e}")
    finally:
        # 移除连接
        if client_id in connected_clients:
            del connected_clients[client_id]
            logging.info(f"客户端断开连接: {client_id}")
            print(f"客户端断开连接: {client_id}")
        
        # 关闭套接字
        try:
            client_socket.close()
        except:
            pass

def send_message(client_socket, message):
    """发送消息到客户端"""
    try:
        # 将消息转换为JSON字符串
        message_str = json.dumps(message)
        message_bytes = message_str.encode('utf-8')
        
        # 添加消息长度前缀
        length_prefix = len(message_bytes).to_bytes(4, byteorder='big')
        
        # 发送消息
        client_socket.sendall(length_prefix + message_bytes)
    except Exception as e:
        logging.error(f"发送消息时发生错误: {e}")
        print(f"发送消息时发生错误: {e}")

def start_server():
    """启动TCP服务器"""
    host = "127.0.0.1"
    port = 5555
    
    try:
        # 创建服务器套接字
        server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        server_socket.bind((host, port))
        server_socket.listen(5)
        
        logging.info(f"启动TCP服务器，监听地址: {host}:{port}")
        print(f"启动TCP服务器，监听地址: {host}:{port}")
        
        client_counter = 0
        
        # 接受客户端连接
        while True:
            client_socket, client_address = server_socket.accept()
            client_id = f"client_{client_counter}"
            client_counter += 1
            
            # 为每个客户端创建新线程
            client_thread = threading.Thread(
                target=handle_client,
                args=(client_socket, client_address, client_id)
            )
            client_thread.daemon = True
            client_thread.start()
    
    except KeyboardInterrupt:
        logging.info("服务器被手动停止")
        print("服务器被手动停止")
    except Exception as e:
        logging.error(f"服务器发生错误: {e}")
        print(f"服务器发生错误: {e}")
    finally:
        # 关闭所有连接
        for client_id, client_socket in list(connected_clients.items()):
            try:
                client_socket.close()
            except:
                pass
        
        # 关闭服务器套接字
        try:
            server_socket.close()
        except:
            pass

if __name__ == "__main__":
    start_server()