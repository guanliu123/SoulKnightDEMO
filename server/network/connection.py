import socket
import threading
import json
import struct
import logging

class Connection:
    """客户端连接类，负责处理单个客户端的通信"""
    
    def __init__(self, client_socket, address, client_id, message_callback):
        self.client_socket = client_socket
        self.address = address
        self.client_id = client_id
        self.message_callback = message_callback
        self.is_connected = True
        
        # 设置超时
        self.client_socket.settimeout(60)  # 60秒超时
        
        # 启动接收线程
        self.receive_thread = threading.Thread(target=self.receive_loop)
        self.receive_thread.daemon = True
        self.receive_thread.start()
    
    def receive_loop(self):
        """接收消息循环"""
        try:
            while self.is_connected:
                # 接收消息长度
                length_bytes = self.receive_exactly(4)
                if not length_bytes:
                    break
                
                # 解析消息长度
                message_length = struct.unpack('!I', length_bytes)[0]
                
                # 接收消息内容
                message_bytes = self.receive_exactly(message_length)
                if not message_bytes:
                    break
                
                # 解析消息
                message_str = message_bytes.decode('utf-8')
                message = json.loads(message_str)
                
                # 提取消息类型和数据
                msg_type = message.get('type')
                msg_data = message.get('data', {})
                
                # 调用消息回调
                self.message_callback(self.client_id, msg_type, msg_data)
        
        except socket.timeout:
            logging.warning(f"客户端连接超时: {self.address}")
        
        except ConnectionResetError:
            logging.info(f"客户端断开连接: {self.address}")
        
        except Exception as e:
            logging.error(f"接收消息时发生错误: {e}")
        
        finally:
            # 断开连接
            self.disconnect()
    
    def receive_exactly(self, n):
        """精确接收n个字节"""
        data = b''
        while len(data) < n:
            try:
                chunk = self.client_socket.recv(n - len(data))
                if not chunk:  # 连接已关闭
                    return None
                data += chunk
            except socket.timeout:
                logging.warning(f"接收数据超时: {self.address}")
                return None
            except ConnectionResetError:
                return None
        return data
    
    def send(self, msg_type, msg_data):
        """发送消息"""
        if not self.is_connected:
            return False
        
        try:
            # 构建消息
            message = {
                'type': msg_type,
                'data': msg_data
            }
            
            # 序列化消息
            message_str = json.dumps(message)
            message_bytes = message_str.encode('utf-8')
            
            # 构建长度前缀
            length_prefix = struct.pack('!I', len(message_bytes))
            
            # 发送消息
            self.client_socket.sendall(length_prefix + message_bytes)
            return True
        
        except Exception as e:
            logging.error(f"发送消息时发生错误: {e}")
            self.disconnect()
            return False
    
    def disconnect(self):
        """断开连接"""
        if not self.is_connected:
            return
        
        self.is_connected = False
        
        try:
            self.client_socket.close()
        except Exception as e:
            logging.error(f"关闭套接字时发生错误: {e}")
        
        # 通知断开连接
        self.message_callback(self.client_id, 'disconnection', {})