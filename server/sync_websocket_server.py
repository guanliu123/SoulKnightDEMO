import socket
import threading
import json
import logging
import time
import random
import base64
import hashlib
import struct

# 配置日志
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s'
)

# 存储所有连接的客户端
clients = {}

def handle_heartbeat(client_socket, data):
    """处理心跳包"""
    response = {
        "c": data["c"],  # 使用相同的命令ID
        "o": data["o"],  # 使用相同的订单ID
        "s": 0,          # 成功状态
        "p": {}          # 空参数
    }
    send_message(client_socket, json.dumps(response))
    logging.info("发送心跳响应")

def handle_login(client_socket, data):
    """处理登录请求"""
    params = data.get("p", {})
    username = params.get("username", "unknown")
    
    logging.info(f"登录请求: 用户名={username}")
    
    # 创建登录响应
    response = {
        "c": data["c"],  # 使用相同的命令ID
        "o": data["o"],  # 使用相同的订单ID
        "s": 0,          # 成功状态
        "d": {
            "userId": f"user_{random.randint(10000, 99999)}",
            "username": username,
            "token": f"token_{int(time.time())}"
        }
    }
    
    send_message(client_socket, json.dumps(response))
    logging.info(f"发送登录响应: {response}")

def handle_message(client_socket, message):
    """处理接收到的消息"""
    try:
        # 解析JSON消息
        data = json.loads(message)
        command_id = data.get("c")
        
        logging.info(f"收到消息: {message}")
        
        # 根据命令ID处理不同类型的消息
        if command_id == 1000:  # 心跳包ID
            handle_heartbeat(client_socket, data)
        elif command_id == 1001:  # 登录请求ID
            handle_login(client_socket, data)
        else:
            # 处理未知命令
            logging.warning(f"未知命令ID: {command_id}")
            response = {
                "c": 9999,  # 错误响应ID
                "o": data.get("o", "0"),
                "s": 1,     # 错误状态
                "d": f"未知命令: {command_id}"
            }
            send_message(client_socket, json.dumps(response))
    except json.JSONDecodeError:
        logging.error(f"无效的JSON格式: {message}")
    except Exception as e:
        logging.error(f"处理消息时出错: {str(e)}", exc_info=True)

def send_message(client_socket, message):
    """发送WebSocket消息"""
    try:
        # 将消息编码为WebSocket帧
        message_bytes = message.encode('utf-8')
        header = bytearray()
        header.append(0x81)  # 文本帧的FIN + opcode
        
        # 设置消息长度
        length = len(message_bytes)
        if length < 126:
            header.append(length)
        elif length < 65536:
            header.append(126)
            header.extend(struct.pack(">H", length))
        else:
            header.append(127)
            header.extend(struct.pack(">Q", length))
        
        # 发送帧头和消息内容
        client_socket.send(header + message_bytes)
        return True
    except Exception as e:
        logging.error(f"发送消息失败: {str(e)}")
        return False

def parse_websocket_frame(data):
    """解析WebSocket帧"""
    if len(data) < 2:
        return None, 0
    
    # 第一个字节: FIN + RSV1-3 + Opcode
    fin = (data[0] & 0x80) != 0
    opcode = data[0] & 0x0F
    
    # 第二个字节: MASK + Payload length
    mask = (data[1] & 0x80) != 0
    payload_length = data[1] & 0x7F
    
    # 确定头部长度
    header_length = 2
    if payload_length == 126:
        if len(data) < 4:
            return None, 0
        payload_length = struct.unpack(">H", data[2:4])[0]
        header_length = 4
    elif payload_length == 127:
        if len(data) < 10:
            return None, 0
        payload_length = struct.unpack(">Q", data[2:10])[0]
        header_length = 10
    
    # 如果有掩码，增加4字节掩码长度
    if mask:
        header_length += 4
        if len(data) < header_length:
            return None, 0
        
        # 获取掩码密钥
        mask_key = data[header_length-4:header_length]
        
        # 确保有足够的数据
        if len(data) < header_length + payload_length:
            return None, 0
        
        # 解码有掩码的数据
        payload = bytearray(data[header_length:header_length+payload_length])
        for i in range(payload_length):
            payload[i] ^= mask_key[i % 4]
        
        return payload, header_length + payload_length
    else:
        # 确保有足够的数据
        if len(data) < header_length + payload_length:
            return None, 0
        
        # 无掩码数据
        return data[header_length:header_length+payload_length], header_length + payload_length

def handle_websocket_handshake(client_socket, data):
    """处理WebSocket握手"""
    try:
        # 将二进制数据转换为字符串
        data_str = data.decode('utf-8')
        
        # 打印收到的握手请求
        logging.info(f"收到WebSocket握手请求:\n{data_str}")
        
        # 解析Sec-WebSocket-Key
        key = ''
        for line in data_str.split('\r\n'):
            if line.startswith('Sec-WebSocket-Key:'):
                key = line.split(':', 1)[1].strip()
                break
        
        if not key:
            logging.error("没有找到Sec-WebSocket-Key")
            return False
        
        # 计算Sec-WebSocket-Accept
        magic_string = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
        accept = base64.b64encode(hashlib.sha1((key + magic_string).encode()).digest()).decode()
        
        # 构建握手响应
        response = (
            "HTTP/1.1 101 Switching Protocols\r\n"
            "Upgrade: websocket\r\n"
            "Connection: Upgrade\r\n"
            f"Sec-WebSocket-Accept: {accept}\r\n"
            "\r\n"
        )
        
        # 发送握手响应
        client_socket.send(response.encode())
        logging.info("WebSocket握手成功")
        return True
    except Exception as e:
        logging.error(f"处理WebSocket握手时出错: {str(e)}")
        return False

def handle_client(client_socket, client_address):
    """处理客户端连接"""
    client_id = id(client_socket)
    clients[client_id] = client_socket
    logging.info(f"客户端连接: {client_address}")
    
    # 接收数据缓冲区
    buffer = bytearray()
    handshake_done = False
    
    try:
        while True:
            # 接收数据
            data = client_socket.recv(4096)
            if not data:
                break
            
            # 将数据添加到缓冲区
            buffer.extend(data)
            
            # 如果还没有完成握手
            if not handshake_done:
                # 尝试处理握手
                if b"\r\n\r\n" in buffer:
                    handshake_done = handle_websocket_handshake(client_socket, buffer)
                    buffer = bytearray()  # 清空缓冲区
                    continue
            
            # 处理WebSocket帧
            while len(buffer) > 0:
                # 尝试解析一个WebSocket帧
                payload, consumed = parse_websocket_frame(buffer)
                
                if payload is None:
                    # 数据不完整，等待更多数据
                    break
                
                # 移除已处理的数据
                buffer = buffer[consumed:]
                
                # 处理有效载荷
                if payload:
                    try:
                        # 尝试将有效载荷解码为文本
                        message = payload.decode('utf-8')
                        logging.info(f"解码后的消息: {message}")
                        
                        # 处理消息
                        handle_message(client_socket, message)
                    except UnicodeDecodeError:
                        # 如果无法解码为UTF-8，打印二进制数据
                        logging.warning(f"收到二进制数据，无法解码为UTF-8: {payload.hex()}")
    except Exception as e:
        logging.error(f"处理客户端连接时出错: {str(e)}", exc_info=True)
    finally:
        # 关闭连接并清理
        if client_id in clients:
            del clients[client_id]
        try:
            client_socket.close()
        except:
            pass
        logging.info(f"客户端 {client_address} 已断开连接")

def start_server(host, port):
    """启动WebSocket服务器"""
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    
    try:
        server_socket.bind((host, port))
        server_socket.listen(5)
        logging.info(f"WebSocket服务器已启动，监听地址: {host}:{port}")
        
        while True:
            client_socket, client_address = server_socket.accept()
            # 为每个客户端创建一个新线程
            client_thread = threading.Thread(
                target=handle_client,
                args=(client_socket, client_address)
            )
            client_thread.daemon = True
            client_thread.start()
    except KeyboardInterrupt:
        logging.info("服务器正在关闭...")
    except Exception as e:
        logging.error(f"服务器出错: {str(e)}", exc_info=True)
    finally:
        server_socket.close()
        logging.info("服务器已关闭")

if __name__ == "__main__":
    import sys
    
    host = "127.0.0.1"
    port = 5555
    
    # 从命令行参数解析地址和端口（如果有）
    if len(sys.argv) >= 2:
        host = sys.argv[1]
    
    if len(sys.argv) >= 3:
        try:
            port = int(sys.argv[2])
        except ValueError:
            logging.error(f"无效的端口号: {sys.argv[2]}")
            sys.exit(1)
    
    start_server(host, port)