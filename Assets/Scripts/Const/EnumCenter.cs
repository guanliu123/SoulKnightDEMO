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

    public enum PlayerType
    {
        Knight=1001,
        Rogue=1002,
    }

    public enum PlayerWeaponType
    {
        BadPistol=1,
        AK47=2,
        DoubleBladeSword=3,
        BlueFireGatling=4,
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
