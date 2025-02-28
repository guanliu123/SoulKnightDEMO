using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UIFrameWork;
using UnityEngine;

public class SelectInfoPanel : BasePanel
{
    private PlayerType playerType;
    
    public SelectInfoPanel(PlayerType _playerType) : base(new UIType(UIInfo.SelectInfoPanel))
    {
        playerType = _playerType;
        LogTool.Log("角色类型"+playerType.ToString());
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }
}
