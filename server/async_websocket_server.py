import asyncio
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

async def handle_heartbeat(writer, data):
    """处理心跳包"""
    response = {
        "c": data["c"],  # 使用相同的命令ID
        "o": data["o"],  # 使用相同的订单ID
        "s": 0,          # 成功状态
        "p": {}          # 空参数
    }
    await send_message(writer, json.dumps(response))
    logging.info("发送心跳响应")

# 在handle_login函数中修改响应格式
async def handle_login(writer, data):
    """处理登录请求"""
    params = data.get("p", {})
    username = params.get("username", "unknown")
    
    logging.info(f"登录请求: 用户名={username}")
    
    # 创建符合客户端期望的登录响应
    user_id = f"user_{random.randint(10000, 99999)}"
    server_time = int(time.time() * 1000)  # 毫秒时间戳
    
    # 创建登录响应
    response = {
        "c": data["c"],  # 使用相同的命令ID
        "o": data["o"],  # 使用相同的订单ID
        "s": 0,          # 成功状态 (LOGIN_OK)
        "d": json.dumps({
            "gameUid": user_id,
            "username": username,
            "serverTime": server_time,
            "isRegister": False,  # 或根据实际情况设置
            "token": f"token_{int(time.time())}"
        })
    }
    
    await send_message(writer, json.dumps(response))
    logging.info(f"发送登录响应: {response}")

# 添加处理LOGIN_OVER请求的函数
async def handle_login_over(writer, data):
    """处理登录完成请求"""
    logging.info("收到登录完成请求")
    
    # 创建登录完成响应
    response = {
        "c": data["c"],  # 使用相同的命令ID
        "o": data["o"],  # 使用相同的订单ID
        "s": 0,          # 成功状态
        "d": None        # 无需返回数据
    }
    
    await send_message(writer, json.dumps(response))
    logging.info("发送登录完成响应")

# 在handle_message函数中添加对LOGIN_OVER的处理
async def handle_message(writer, message):
    """处理接收到的消息"""
    try:
        # 解析JSON消息
        data = json.loads(message)
        command_id = data.get("c")
        
        logging.info(f"收到消息: {message}")
        
        # 根据命令ID处理不同类型的消息
        if command_id == 1000:  # 心跳包ID
            await handle_heartbeat(writer, data)
        elif command_id == 1:  # 登录请求ID
            await handle_login(writer, data)
        elif command_id == 1002:  # 假设LOGIN_OVER的ID是1002
            await handle_login_over(writer, data)
        else:
            # 处理未知命令
            logging.warning(f"未知命令ID: {command_id}")
            response = {
                "c": 9999,  # 错误响应ID
                "o": data.get("o", "0"),
                "s": 1,     # 错误状态
                "d": f"未知命令: {command_id}"
            }
            await send_message(writer, json.dumps(response))
    except json.JSONDecodeError:
        logging.error(f"无效的JSON格式: {message}")
    except Exception as e:
        logging.error(f"处理消息时出错: {str(e)}", exc_info=True)

async def send_message(writer, message):
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
        writer.write(header + message_bytes)
        await writer.drain()
        return True
    except Exception as e:
        logging.error(f"发送消息失败: {str(e)}")
        return False

def parse_websocket_frame(data):
    """解析WebSocket帧"""
    if len(data) < 2:
        return None, 0, 0
    
    # 第一个字节: FIN + RSV1-3 + Opcode
    fin = (data[0] & 0x80) != 0
    opcode = data[0] & 0x0F
    
    # 记录操作码类型
    opcode_type = "未知"
    if opcode == 0x0:
        opcode_type = "继续帧"
    elif opcode == 0x1:
        opcode_type = "文本帧"
    elif opcode == 0x2:
        opcode_type = "二进制帧"
    elif opcode == 0x8:
        opcode_type = "关闭帧"
    elif opcode == 0x9:
        opcode_type = "Ping帧"
    elif opcode == 0xA:
        opcode_type = "Pong帧"
    
    logging.debug(f"WebSocket帧类型: {opcode_type} (opcode={opcode}), FIN={fin}")
    
    # 第二个字节: MASK + Payload length
    mask = (data[1] & 0x80) != 0
    payload_length = data[1] & 0x7F
    
    # 确定头部长度
    header_length = 2
    if payload_length == 126:
        if len(data) < 4:
            return None, 0, 0
        payload_length = struct.unpack(">H", data[2:4])[0]
        header_length = 4
    elif payload_length == 127:
        if len(data) < 10:
            return None, 0, 0
        payload_length = struct.unpack(">Q", data[2:10])[0]
        header_length = 10
    
    # 如果有掩码，增加4字节掩码长度
    if mask:
        header_length += 4
        if len(data) < header_length:
            return None, 0, 0
        
        # 获取掩码密钥
        mask_key = data[header_length-4:header_length]
        
        # 确保有足够的数据
        if len(data) < header_length + payload_length:
            return None, 0, 0
        
        # 解码有掩码的数据
        payload = bytearray(data[header_length:header_length+payload_length])
        for i in range(payload_length):
            payload[i] ^= mask_key[i % 4]
        
        return payload, header_length + payload_length, opcode
    else:
        # 确保有足够的数据
        if len(data) < header_length + payload_length:
            return None, 0, 0
        
        # 无掩码数据
        return data[header_length:header_length+payload_length], header_length + payload_length, opcode

async def handle_websocket_handshake(reader, writer):
    """处理WebSocket握手"""
    try:
        # 读取HTTP请求
        data = await reader.read(4096)
        
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
        writer.write(response.encode())
        await writer.drain()
        logging.info("WebSocket握手成功")
        return True
    except Exception as e:
        logging.error(f"处理WebSocket握手时出错: {str(e)}")
        return False

async def handle_client(reader, writer):
    """处理客户端连接"""
    addr = writer.get_extra_info('peername')
    client_id = id(writer)
    clients[client_id] = writer
    logging.info(f"客户端连接: {addr}")
    
    # 接收数据缓冲区
    buffer = bytearray()
    handshake_done = False
    
    try:
        # 处理握手
        handshake_done = await handle_websocket_handshake(reader, writer)
        if not handshake_done:
            logging.error("WebSocket握手失败")
            return
        
        # 处理WebSocket消息
        while True:
            # 接收数据
            data = await reader.read(4096)
            if not data:
                break
            
            # 将数据添加到缓冲区
            buffer.extend(data)
            
            # 处理WebSocket帧
            while len(buffer) > 0:
                # 尝试解析一个WebSocket帧
                result = parse_websocket_frame(buffer)
                
                if result[0] is None:
                    # 数据不完整，等待更多数据
                    break
                
                payload, consumed, opcode = result
                
                # 移除已处理的数据
                buffer = buffer[consumed:]
                
                # 处理有效载荷
                if payload:
                    # 根据操作码处理不同类型的帧
                    if opcode == 0x1:  # 文本帧
                        try:
                            # 尝试将有效载荷解码为文本
                            message = payload.decode('utf-8')
                            logging.info(f"解码后的消息: {message}")
                            
                            # 处理消息
                            await handle_message(writer, message)
                        except UnicodeDecodeError:
                            # 如果无法解码为UTF-8，打印二进制数据
                            logging.warning(f"收到文本帧但无法解码为UTF-8: {payload.hex()}")
                    
                    elif opcode == 0x2:  # 二进制帧
                        # 尝试将二进制数据解析为JSON
                        try:
                            message = payload.decode('utf-8')
                            logging.info(f"二进制帧解码为文本: {message}")
                            await handle_message(writer, message)
                        except UnicodeDecodeError:
                            logging.info(f"收到二进制帧: {payload.hex()}")
                            # 这里可以添加二进制数据的处理逻辑
                    
                    elif opcode == 0x8:  # 关闭帧
                        logging.info("收到关闭帧，关闭连接")
                        return
                    
                    elif opcode == 0x9:  # Ping帧
                        logging.info(f"收到Ping帧: {payload.hex()}")
                        # 发送Pong帧作为响应
                        pong_header = bytearray([0x8A])  # FIN=1, Opcode=0xA (Pong)
                        length = len(payload)
                        if length < 126:
                            pong_header.append(length)
                        elif length < 65536:
                            pong_header.append(126)
                            pong_header.extend(struct.pack(">H", length))
                        else:
                            pong_header.append(127)
                            pong_header.extend(struct.pack(">Q", length))
                        
                        writer.write(pong_header + payload)
                        await writer.drain()
                        logging.info("发送Pong帧响应")
                    
                    elif opcode == 0xA:  # Pong帧
                        logging.info(f"收到Pong帧: {payload.hex()}")
                        # 通常不需要对Pong帧做特殊处理
                    
                    else:
                        logging.warning(f"收到未知类型的帧 (opcode={opcode}): {payload.hex()}")
    except asyncio.CancelledError:
        logging.info(f"客户端连接被取消: {addr}")
    except Exception as e:
        logging.error(f"处理客户端连接时出错: {str(e)}", exc_info=True)
    finally:
        # 关闭连接并清理
        if client_id in clients:
            del clients[client_id]
        writer.close()
        await writer.wait_closed()
        logging.info(f"客户端 {addr} 已断开连接")

async def main():
    """启动WebSocket服务器"""
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
    
    server = await asyncio.start_server(
        handle_client, host, port
    )
    
    addr = server.sockets[0].getsockname()
    logging.info(f'WebSocket服务器已启动，监听地址: {addr}')
    
    async with server:
        await server.serve_forever()

if __name__ == "__main__":
    try:
        asyncio.run(main())
    except KeyboardInterrupt:
        logging.info("服务器已停止")