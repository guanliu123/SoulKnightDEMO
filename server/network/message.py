import json
import struct
import logging

class Message:
    """消息封装类，处理消息的序列化和反序列化"""
    
    @staticmethod
    def pack(msg_type, msg_data):
        """
        将消息打包为二进制格式
        格式: [消息长度(4字节)][消息类型(2字节)][消息内容(JSON)]
        """
        msg_content = json.dumps(msg_data, ensure_ascii=False).encode('utf-8')
        msg_type_bytes = struct.pack('!H', msg_type)
        msg_len = len(msg_content) + len(msg_type_bytes)
        msg_len_bytes = struct.pack('!I', msg_len)
        
        return msg_len_bytes + msg_type_bytes + msg_content
    
    @staticmethod
    def unpack(msg_data):
        """
        将二进制消息解包为消息类型和内容
        返回: (msg_type, msg_content)
        """
        msg_type = struct.unpack('!H', msg_data[:2])[0]
        msg_content = json.loads(msg_data[2:].decode('utf-8'))
        
        return msg_type, msg_content
    
    @staticmethod
    def get_length(msg_data):
        """
        获取消息长度
        """
        return struct.unpack('!I', msg_data[:4])[0]