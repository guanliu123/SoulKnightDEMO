namespace EnumCenter
{
    public enum RoomType
    {
        BasicRoom=0,
        BirthRoom=1,
        EnemyRoom = 2,
        BossRoom=3,
        TeleportRoom=4,
        TreasureRoom=5,
        ShopRoom=6,
        Corridor=7,
    }

    public enum CustomCameraType
    {
        StaticCamera=0,
        SelectCamera=1,
        FollowCamera=2,
    }

    //在CharacterRoot用于标识其子类继承物的具体Type
    public enum CharacterType
    {
        None=0,
        Player=1,
        Pet=2,
        Enemy=3,
    }

    public enum PlayerType
    {
        None=0,
        Knight=1001,
        Rogue=1002,
    }

    public enum PetType
    {
        None=0,
        LittleCool=1,
    }

    //场景上可互动物体的root的type
    public enum ItemRootType
    {
        None=0,
        Weapon=1,
    }

    public enum WeaponType
    {
        BadPistol=1001,
        AK47=1002,
        DoubleBladeSword=1003,
        BlueFireGatling=1004,
    }
    
    public enum InteractiveObjectType
    {
        Weapon=1,
    }
    
    public enum GameModeType
    {
        SingleMode=1<<0,
        MultipleMode=1<<1,
    }

    public enum EffectType
    {
        EffectBoom=1,
    } 

    public enum BulletType
    {
        Bullet_1=1,
    }
    
    public enum BuffType
    {
        DebuffType=1,
    }
}
