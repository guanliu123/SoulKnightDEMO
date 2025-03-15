import logging
import sqlite3
import json
import time
import hashlib
import os

class DBManager:
    """数据库管理器，负责处理数据库操作"""
    
    def __init__(self, db_path="data/soulknight.db"):
        self.db_path = db_path
        
        # 确保数据目录存在
        os.makedirs(os.path.dirname(db_path), exist_ok=True)
        
        # 初始化数据库
        self._init_db()
    
    def _init_db(self):
        """初始化数据库"""
        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            # 创建用户表
            cursor.execute('''
            CREATE TABLE IF NOT EXISTS users (
                user_id TEXT PRIMARY KEY,
                username TEXT UNIQUE,
                password_hash TEXT,
                salt TEXT,
                created_at REAL,
                last_login REAL
            )
            ''')
            
            # 创建玩家数据表
            cursor.execute('''
            CREATE TABLE IF NOT EXISTS player_data (
                user_id TEXT PRIMARY KEY,
                int_dict TEXT,
                obj_dict TEXT,
                last_update_time REAL,
                FOREIGN KEY (user_id) REFERENCES users (user_id)
            )
            ''')
            
            conn.commit()
            conn.close()
            
            logging.info("数据库初始化成功")
        
        except Exception as e:
            logging.error(f"初始化数据库时发生错误: {e}")
    
    def user_exists(self, username):
        """检查用户名是否存在"""
        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            cursor.execute("SELECT 1 FROM users WHERE username = ?", (username,))
            result = cursor.fetchone()
            
            conn.close()
            
            return result is not None
        
        except Exception as e:
            logging.error(f"检查用户名时发生错误: {e}")
            return False
    
    def create_user(self, username, password):
        """创建新用户"""
        try:
            # 生成用户ID
            import uuid
            user_id = str(uuid.uuid4())
            
            # 生成盐值和密码哈希
            salt = os.urandom(16).hex()
            password_hash = self._hash_password(password, salt)
            
            # 当前时间
            current_time = time.time()
            
            # 插入用户记录
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            cursor.execute(
                "INSERT INTO users (user_id, username, password_hash, salt, created_at, last_login) VALUES (?, ?, ?, ?, ?, ?)",
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
                "INSERT INTO player_data (user_id, int_dict, obj_dict, last_update_time) VALUES (?, ?, ?, ?)",
                (
                    user_id,
                    json.dumps(default_int_dict),
                    json.dumps(default_obj_dict),
                    current_time
                )
            )
            
            conn.commit()
            conn.close()
            
            return user_id
        
        except Exception as e:
            logging.error(f"创建用户时发生错误: {e}")
            return None
    
    def verify_user(self, username, password):
        """验证用户名和密码"""
        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            cursor.execute(
                "SELECT user_id, password_hash, salt FROM users WHERE username = ?",
                (username,)
            )
            
            result = cursor.fetchone()
            
            if not result:
                conn.close()
                return None
            
            user_id, stored_hash, salt = result
            
            # 验证密码
            input_hash = self._hash_password(password, salt)
            
            if input_hash != stored_hash:
                conn.close()
                return None
            
            # 更新最后登录时间
            current_time = time.time()
            cursor.execute(
                "UPDATE users SET last_login = ? WHERE user_id = ?",
                (current_time, user_id)
            )
            
            conn.commit()
            conn.close()
            
            return {"user_id": user_id, "username": username}
        
        except Exception as e:
            logging.error(f"验证用户时发生错误: {e}")
            return None
    
    def get_player_data(self, user_id):
        """获取玩家数据"""
        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            cursor.execute(
                "SELECT int_dict, obj_dict FROM player_data WHERE user_id = ?",
                (user_id,)
            )
            
            result = cursor.fetchone()
            
            conn.close()
            
            if not result:
                return None
            
            int_dict_str, obj_dict_str = result
            
            return {
                "int_dict": json.loads(int_dict_str),
                "obj_dict": json.loads(obj_dict_str)
            }
        
        except Exception as e:
            logging.error(f"获取玩家数据时发生错误: {e}")
            return None
    
    def save_player_data(self, user_id, player_data):
        """保存玩家数据"""
        try:
            conn = sqlite3.connect(self.db_path)
            cursor = conn.cursor()
            
            int_dict = player_data.get("int_dict", {})
            obj_dict = player_data.get("obj_dict", {})
            last_update_time = player_data.get("last_update_time", time.time())
            
            cursor.execute(
                "UPDATE player_data SET int_dict = ?, obj_dict = ?, last_update_time = ? WHERE user_id = ?",
                (
                    json.dumps(int_dict),
                    json.dumps(obj_dict),
                    last_update_time,
                    user_id
                )
            )
            
            conn.commit()
            conn.close()
            
            return True
        
        except Exception as e:
            logging.error(f"保存玩家数据时发生错误: {e}")
            return False
    
    def _hash_password(self, password, salt):
        """哈希密码"""
        return hashlib.pbkdf2_hmac(
            'sha256', 
            password.encode('utf-8'), 
            bytes.fromhex(salt), 
            100000
        ).hex()