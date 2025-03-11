using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserData
{
    public static bool HasLogin
    {
        get;private set;
    }
    public static bool isNew { get; private set; }

    public static Dictionary<int, TCharacterData> PlayerCharacters { get; private set; }

   static UserData()
    {
        PlayerCharacters = new Dictionary<int, TCharacterData>();
    }
    
    public static void UpdateUserData(TServerDataLogin userData)
    {
        //todo:接收服务器传来的数据更新玩家数据
    }
    
    
}
