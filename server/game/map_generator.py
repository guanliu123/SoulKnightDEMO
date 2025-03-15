import random
import logging

class MapGenerator:
    """地图生成器，负责生成游戏地图"""
    
    def __init__(self):
        self.room_types = ["normal", "elite", "boss", "shop", "chest"]
        self.enemy_types = ["slime", "bat", "goblin", "skeleton", "wizard", "knight"]
        self.item_types = ["health_potion", "mana_potion", "weapon", "armor", "accessory"]
    
    def generate_map(self, seed=None):
        """
        生成游戏地图
        参数:
            seed: 随机种子，用于确保地图生成的一致性
        返回:
            地图数据字典
        """
        if seed is not None:
            random.seed(seed)
        
        # 生成地图基本信息
        map_data = {
            "width": random.randint(8, 12),
            "height": random.randint(8, 12),
            "rooms": [],
            "enemies": [],
            "items": [],
            "start_position": {"x": 0, "y": 0}
        }
        
        # 生成房间
        num_rooms = random.randint(10, 15)
        for i in range(num_rooms):
            room = self.generate_room(i, map_data["width"], map_data["height"])
            map_data["rooms"].append(room)
        
        # 生成敌人
        num_enemies = random.randint(20, 30)
        for i in range(num_enemies):
            enemy = self.generate_enemy(i, map_data["rooms"])
            map_data["enemies"].append(enemy)
        
        # 生成物品
        num_items = random.randint(10, 15)
        for i in range(num_items):
            item = self.generate_item(i, map_data["rooms"])
            map_data["items"].append(item)
        
        # 设置起始位置
        start_room = map_data["rooms"][0]
        map_data["start_position"] = {
            "x": start_room["x"] + start_room["width"] // 2,
            "y": start_room["y"] + start_room["height"] // 2
        }
        
        logging.info(f"生成地图: {num_rooms}个房间, {num_enemies}个敌人, {num_items}个物品")
        
        return map_data
    
    def generate_room(self, room_id, map_width, map_height):
        """生成单个房间"""
        room_type = random.choice(self.room_types)
        
        # 根据房间类型调整大小
        if room_type == "boss":
            width = random.randint(5, 7)
            height = random.randint(5, 7)
        elif room_type == "shop":
            width = random.randint(4, 6)
            height = random.randint(4, 6)
        else:
            width = random.randint(3, 5)
            height = random.randint(3, 5)
        
        # 随机位置
        x = random.randint(0, map_width - width)
        y = random.randint(0, map_height - height)
        
        return {
            "id": room_id,
            "type": room_type,
            "x": x,
            "y": y,
            "width": width,
            "height": height,
            "cleared": False
        }
    
    def generate_enemy(self, enemy_id, rooms):
        """生成单个敌人"""
        # 随机选择一个房间放置敌人
        room = random.choice(rooms)
        
        # 不在起始房间生成敌人
        if room["id"] == 0:
            room = random.choice(rooms[1:])
        
        # 根据房间类型选择敌人类型
        if room["type"] == "boss":
            enemy_type = random.choice(["knight", "wizard"])
            health = random.randint(80, 100)
            damage = random.randint(15, 20)
            speed = random.uniform(0.8, 1.2)
        elif room["type"] == "elite":
            enemy_type = random.choice(["goblin", "skeleton"])
            health = random.randint(40, 60)
            damage = random.randint(8, 12)
            speed = random.uniform(1.0, 1.5)
        else:
            enemy_type = random.choice(["slime", "bat"])
            health = random.randint(20, 30)
            damage = random.randint(5, 8)
            speed = random.uniform(1.2, 1.8)
        
        # 在房间内随机位置
        x = room["x"] + random.randint(1, room["width"] - 1)
        y = room["y"] + random.randint(1, room["height"] - 1)
        
        return {
            "id": enemy_id,
            "type": enemy_type,
            "x": x,
            "y": y,
            "health": health,
            "damage": damage,
            "speed": speed,
            "room_id": room["id"]
        }
    
    def generate_item(self, item_id, rooms):
        """生成单个物品"""
        # 随机选择一个房间放置物品
        room = random.choice(rooms)
        
        # 根据房间类型选择物品类型
        if room["type"] == "chest":
            item_type = random.choice(["weapon", "armor", "accessory"])
            rarity = random.choice(["common", "uncommon", "rare"])
        elif room["type"] == "shop":
            item_type = random.choice(self.item_types)
            rarity = random.choice(["common", "uncommon"])
        else:
            item_type = random.choice(["health_potion", "mana_potion"])
            rarity = "common"
        
        # 在房间内随机位置
        x = room["x"] + random.randint(1, room["width"] - 1)
        y = room["y"] + random.randint(1, room["height"] - 1)
        
        # 根据物品类型和稀有度设置属性
        properties = {}
        
        if item_type == "weapon":
            properties["damage"] = random.randint(10, 30) * (1 + {"common": 0, "uncommon": 0.5, "rare": 1}[rarity])
            properties["attack_speed"] = random.uniform(0.8, 1.5)
            properties["range"] = random.uniform(1.0, 3.0)
        elif item_type == "armor":
            properties["defense"] = random.randint(5, 15) * (1 + {"common": 0, "uncommon": 0.5, "rare": 1}[rarity])
        elif item_type == "accessory":
            properties["effect"] = random.choice(["speed", "health", "mana", "damage"])
            properties["value"] = random.uniform(0.1, 0.3) * (1 + {"common": 0, "uncommon": 0.5, "rare": 1}[rarity])
        elif item_type == "health_potion":
            properties["heal_amount"] = random.randint(20, 40)
        elif item_type == "mana_potion":
            properties["mana_amount"] = random.randint(20, 40)
        
        return {
            "id": item_id,
            "type": item_type,
            "rarity": rarity,
            "x": x,
            "y": y,
            "properties": properties,
            "room_id": room["id"]
        }