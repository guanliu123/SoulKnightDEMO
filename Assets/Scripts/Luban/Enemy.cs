
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg
{
public sealed partial class Enemy : Luban.BeanBase
{
    public Enemy(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = _buf["id"]; }
        { if(!_buf["name"].IsString) { throw new SerializationException(); }  Name = _buf["name"]; }
        { if(!_buf["hp"].IsNumber) { throw new SerializationException(); }  Hp = _buf["hp"]; }
        { if(!_buf["speed"].IsNumber) { throw new SerializationException(); }  Speed = _buf["speed"]; }
        { if(!_buf["Initialweapon"].IsNumber) { throw new SerializationException(); }  Initialweapon = _buf["Initialweapon"]; }
    }

    public static Enemy DeserializeEnemy(JSONNode _buf)
    {
        return new Enemy(_buf);
    }

    /// <summary>
    /// ID
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 角色名称
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 初始生命值
    /// </summary>
    public readonly int Hp;
    /// <summary>
    /// 初始速度
    /// </summary>
    public readonly float Speed;
    /// <summary>
    /// 初始武器对应id
    /// </summary>
    public readonly int Initialweapon;
   
    public const int __ID__ = 67100520;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "hp:" + Hp + ","
        + "speed:" + Speed + ","
        + "Initialweapon:" + Initialweapon + ","
        + "}";
    }
}

}

