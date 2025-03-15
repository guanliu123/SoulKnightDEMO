import json
import os
import logging

class ConfigManager:
    """配置管理器，负责读取和管理配置"""
    
    def __init__(self, config_file="config.json"):
        self.config_file = config_file
        self.config = {}
        self.load_config()
    
    def load_config(self):
        """加载配置文件"""
        try:
            if os.path.exists(self.config_file):
                with open(self.config_file, 'r', encoding='utf-8') as f:
                    self.config = json.load(f)
                logging.info(f"已加载配置文件: {self.config_file}")
            else:
                logging.warning(f"配置文件不存在: {self.config_file}，将使用默认配置")
        except Exception as e:
            logging.error(f"加载配置文件时发生错误: {e}")
    
    def get(self, section, key, default=None):
        """获取配置项"""
        if section in self.config and key in self.config[section]:
            return self.config[section][key]
        return default
    
    def get_section(self, section, default=None):
        """获取配置节"""
        return self.config.get(section, default or {})