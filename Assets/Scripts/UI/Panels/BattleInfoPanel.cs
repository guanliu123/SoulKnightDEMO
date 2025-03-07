using System.Collections;
using System.Collections.Generic;
using cfg;
using EnumCenter;
using TMPro;
using UIFrameWork;
using UnityEngine;

public class BattleInfoPanel : BasePanel
{
    //private Character characterCfg;
    public BattleInfoPanel() : base(new UIType(UIInfo.BattleInfoPanel))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        var camera = AbstractManager.Instance.GetSystem<CameraSystem>();
        camera.ChangeCamera(CustomCameraType.FollowCamera);

        var mainPlayer = AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
        camera.SetFollowTarget(mainPlayer.transform);
    }
}
