import logging
import mysql.connector
import json
import time
import hashlib
import os
import uuid

class MySQLManager:
    """MySQL数据库管理器，负责处理数据库操作"""
    
    def __init__(self, host='localhost', user='root', password='', database='soulknight'):
        self.config = {
            'host': host,
            'user': user,
            'password': password,
            'database': database
        }
        
        # 初始化数据库
        self._init_db()
    
    def _get_connection(self):
        """获取数据库连接"""
        try:
            return mysql.connector.connect(**self.config)
        except mysql.connector.Error as err:
            logging.error(f"数据库连接错误: {err}")
            return None
    
    def _init_db(self):
        """初始化数据库"""
        try:
            # 尝试连接到数据库
            conn = mysql.connector.connect(
                host=self.config['host'],
                user=self.config['user'],
                password=self.config['password']
            )
            cursor = conn.cursor()
            
            # 创建数据库（如果不存在）
            cursor.execute(f"CREATE DATABASE IF NOT EXISTS {self.config['database']}")
            cursor.execute(f"USE {self.config['database']}")
            
            # 创建用户表
            cursor.execute('''
            CREATE TABLE IF NOT EXISTS users (
                user_id VARCHAR(36) PRIMARY KEY,
                username VARCHAR(50) UNIQUE,
                password_hash VARCHAR(128),
                salt VARCHAR(32),
                created_at DOUBLE,
                last_login DOUBLE
            )
            ''')
            
            # 创建玩家数据表
            cursor.execute('''
            CREATE TABLE IF NOT EXISTS player_data (
                user_id VARCHAR(36) PRIMARY KEY,
                int_dict TEXT,
                obj_dict TEXT,
                last_update_time DOUBLE,
                FOREIGN KEY (user_id) REFERENCES users (user_id)
            )
            ''')
            
            conn.commit()
            conn.close()
            
            logging.info("MySQL数据库初始化成功")
        
        except mysql.connector.Error as err:
            logging.error(f"初始化数据库时发生错误: {err}")
    
    def user_exists(self, username):
        """检查用户名是否存在"""
        conn = self._get_connection()
        if not conn:
            return False
        
        try:
            cursor = conn.cursor()
            cursor.execute("SELECT 1 FROM users WHERE username = %s", (username,))
            result = cursor.fetchone()
            conn.close()
            
            return result is not None
        
        except mysql.connector.Error as err:
            logging.error(f"检查用户名时发生错误: {err}")
            if conn:
                conn.close()
            return False
    
    def create_user(self, username, password):
        """创建新用户"""
        conn = self._get_connection()
        if not conn:
            return None, "数据库连接失败"
        
        try:
            # 检查用户名是否已存在
            if self.user_exists(username):
                conn.close()
                return None, "用户名已存在"
            
            # 生成用户ID
            user_id = str(uuid.uuid4())
            
            # 生成盐值和密码哈希
            salt = os.urandom(16).hex()
            password_hash = self._hash_password(password, salt)
            
            # 当前时间
            current_time = time.time()
            
            cursor = conn.cursor()
            
            # 插入用户记录
            cursor.execute(
                "INSERT INTO users (user_id, username, password_hash, salt, created_at, last_login) VALUES (%s, %s, %s, %s, %s, %s)",
                (user_id, username, password_hash, salt, current_time, current_time)
            )
            
            # 创建默认玩家数据
            default_int_dict = {
                "level": 1,
                "exp": 0,
                "coins": 100,
                "gems": 10,
                "energy": 100,
                "max_energy": 100
            }
            
            default_obj_dict = {
                "characters": {
                    "knight": {
                        "unlocked": True,
                        "level": 1,
                        "skins": ["default"]
                    }
                },
                "weapons": {},
                "items": {}
            }
            
            cursor.execute(
                "INSERT INTO player_data (user_id, int_dict, obj_dict, last_update_time) VALUES (%s, %s, %s, %s)",
                (
                    user_id,
                    json.dumps(default_int_dict),
                    json.dumps(default_obj_dict),
                    current_time
                )
            )
            
            conn.commit()
            conn.close()
            
            return user_id, None
        
        except mysql.connector.Error as err:
            logging.error(f"创建用户时发生错误: {err}")
            if conn:
                conn.rollback()
                conn.close()
            return None, f"数据库错误: {err}"
    
    def verify_user(self, username, password):
        """验证用户名和密码"""
        conn = self._get_connection()
        if not conn:
            return None, "数据库连接失败"
        
        try:
            cursor = conn.cursor(dictionary=True)
            
            cursor.execute(
                "SELECT user_id, password_hash, salt FROM users WHERE username = %s",
                (username,)
            )
            
            result = cursor.fetchone()
            
            if not result:
                conn.close()
                return None, "用户名不存在"
            
            user_id = result['user_id']
            stored_hash = result['password_hash']
            salt = result['salt']
            
            # 验证密码
            input_hash = self._hash_password(password, salt)
            
            if input_hash != stored_hash:
                conn.close()
                return None, "密码错误"
            
            # 更新最后登录时间
            current_time = time.time()
            cursor.execute(
                "UPDATE users SET last_login = %s WHERE user_id = %s",
                (current_time, user_id)
            )
            
            conn.commit()
            conn.close()
            
            return {"user_id": user_id, "username": username}, None
        
        except mysql.connector.Error as err:
            logging.error(f"验证用户时发生错误: {err}")
            if conn:
                conn.close()
            return None, f"数据库错误: {err}"
    
    def get_player_data(self, user_id):
        """获取玩家数据"""
        conn = self._get_connection()
        if not conn:
            return None, "数据库连接失败"
        
        try:
            cursor = conn.cursor(dictionary=True)
            
            cursor.execute(
                "SELECT int_dict, obj_dict FROM player_data WHERE user_id = %s",
                (user_id,)
            )
            
            result = cursor.fetchone()
            conn.close()
            
            if not result:
                return None, "玩家数据不存在"
            
            return {
                "int_dict": json.loads(result['int_dict']),
                "obj_dict": json.loads(result['obj_dict'])
            }, None
        
        except mysql.connector.Error as err:
            logging.error(f"获取玩家数据时发生错误: {err}")
            if conn:
                conn.close()
            return None, f"数据库错误: {err}"
    
    def save_player_data(self, user_id, int_dict, obj_dict):
        """保存玩家数据"""
        conn = self._get_connection()
        if not conn:
            return False, "数据库连接失败"
        
        try:
            cursor = conn.cursor()
            
            last_update_time = time.time()
            
            cursor.execute(
                "UPDATE player_data SET int_dict = %s, obj_dict = %s, last_update_time = %s WHERE user_id = %s",
                (
                    json.dumps(int_dict),
                    json.dumps(obj_dict),
                    last_update_time,
                    user_id
                )
            )
            
            conn.commit()
            conn.close()
            
            return True, None
        
        except mysql.connector.Error as err:
            logging.error(f"保存玩家数据时发生错误: {err}")
            if conn:
                conn.rollback()
                conn.close()
            return False, f"数据库错误: {err}"
    
    def _hash_password(self, password, salt):
        """哈希密码"""
        return hashlib.pbkdf2_hmac(
            'sha256', 
            password.encode('utf-8'), 
            bytes.fromhex(salt), 
            100000
        ).hex()