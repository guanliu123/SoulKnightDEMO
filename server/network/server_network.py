import asyncio
import logging
import json
import threading
import base64
import hashlib
import struct
from typing import Dict, Any

class ServerNetwork:
    def __init__(self, host: str, port: int):
        self.host = host
        self.port = port
        self.clients = {}  # 存储所有连接的客户端 {client_id: (reader, writer)}
        self.message_handler = None  # 消息处理器引用
        self.server = None
        self.is_running = False
        self.main_loop = None
        self.server_thread = None
    
    def set_message_handler(self, message_handler):
        """设置消息处理器"""
        self.message_handler = message_handler
        self.message_handler.set_network(self)
    
    async def handle_client(self, reader, writer):
        """处理客户端连接"""
        addr = writer.get_extra_info('peername')
        client_id = id(writer)
        self.clients[client_id] = (reader, writer)
        logging.info(f"客户端连接: {addr}")
        
        # 接收数据缓冲区
        buffer = bytearray()
        handshake_done = False
        
        try:
            # 处理握手
            handshake_done = await self.handle_websocket_handshake(reader, writer)
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
                    result = self.parse_websocket_frame(buffer)
                    
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
                                await self.process_message(client_id, message)
                            except UnicodeDecodeError:
                                # 如果无法解码为UTF-8，打印二进制数据
                                logging.warning(f"收到文本帧但无法解码为UTF-8: {payload.hex()}")
                        
                        elif opcode == 0x2:  # 二进制帧
                            # 尝试将二进制数据解析为JSON
                            try:
                                message = payload.decode('utf-8')
                                logging.info(f"二进制帧解码为文本: {message}")
                                await self.process_message(client_id, message)
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
            if client_id in self.clients:
                del self.clients[client_id]
            writer.close()
            await writer.wait_closed()
            logging.info(f"客户端 {addr} 已断开连接")
    
    async def process_message(self, client_id, message):
        """处理接收到的消息"""
        try:
            # 尝试将消息解码为文本
            if isinstance(message, bytes):
                try:
                    message = message.decode('utf-8')
                except UnicodeDecodeError:
                    logging.warning("收到二进制数据，无法解码为UTF-8")
                    return
            
            data = json.loads(message)
            logging.debug(f"收到消息: {message}")
            
            if self.message_handler:
                # 将消息转发给消息处理器
                await self.message_handler.handle_message(client_id, data)
            else:
                logging.error("未设置消息处理器")
        except json.JSONDecodeError:
            logging.error(f"无效的JSON格式: {message}")
        except Exception as e:
            logging.error(f"处理消息时出错: {str(e)}", exc_info=True)
    
    async def send_to_client(self, client_id, data):
        """向指定客户端发送消息"""
        if client_id in self.clients:
            try:
                _, writer = self.clients[client_id]
                await self.send_message(writer, json.dumps(data))
                logging.debug(f"发送消息到客户端 {client_id}: {data}")
            except Exception as e:
                logging.error(f"发送消息失败: {str(e)}")
                # 如果发送失败，可能客户端已断开连接
                if client_id in self.clients:
                    del self.clients[client_id]
        else:
            logging.warning(f"客户端 {client_id} 不存在或已断开连接")
    
    async def send_error(self, client_id, order_id, error_message):
        """发送错误消息"""
        error_data = {
            "c": 9999,  # 错误响应ID
            "o": order_id,
            "s": 1,     # 错误状态
            "d": error_message
        }
        await self.send_to_client(client_id, error_data)
    
    async def broadcast(self, data, exclude_client_id=None):
        """广播消息给所有客户端，可选择排除特定客户端"""
        for client_id, (_, writer) in list(self.clients.items()):
            if exclude_client_id is not None and client_id == exclude_client_id:
                continue
            
            try:
                await self.send_message(writer, json.dumps(data))
            except Exception as e:
                logging.error(f"广播消息到客户端 {client_id} 失败: {str(e)}")
                # 如果发送失败，可能客户端已断开连接
                if client_id in self.clients:
                    del self.clients[client_id]
    
    async def send_message(self, writer, message):
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
    
    def parse_websocket_frame(self, data):
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
    
    async def handle_websocket_handshake(self, reader, writer):
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
    
    async def run_server(self):
        """运行WebSocket服务器"""
        self.server = await asyncio.start_server(
            self.handle_client, self.host, self.port
        )
        self.is_running = True
        logging.info(f"WebSocket服务器已启动，监听地址: {self.host}:{self.port}")
        
        async with self.server:
            await self.server.serve_forever()
    
    def start(self):
        """启动服务器（非阻塞）"""
        try:
            self.main_loop = asyncio.new_event_loop()
            
            def run_async_server():
                asyncio.set_event_loop(self.main_loop)
                self.main_loop.run_until_complete(self.run_server())
            
            self.server_thread = threading.Thread(target=run_async_server)
            self.server_thread.daemon = True
            self.server_thread.start()
            return True
        except Exception as e:
            logging.error(f"启动服务器失败: {str(e)}", exc_info=True)
            return False
    
    def stop(self):
        """停止服务器"""
        if self.is_running and self.main_loop:
            async def close_server():
                self.server.close()
                await self.server.wait_closed()
                self.is_running = False
                logging.info("WebSocket服务器已停止")
            
            future = asyncio.run_coroutine_threadsafe(close_server(), self.main_loop)
            try:
                future.result(timeout=5)  # 等待最多5秒
            except Exception as e:
                logging.error(f"关闭服务器时出错: {str(e)}")
            
            # 关闭事件循环
            self.main_loop.stop()
            if self.server_thread:
                self.server_thread.join(timeout=5)