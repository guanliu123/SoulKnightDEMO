using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UIFrameWork;
using UnityEngine;

public class BattleInfoPanel : BasePanel
{
    public BattleInfoPanel() : base(new UIType(UIInfo.BattleInfoPanel))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        var camera = AbstractManager.Instance.GetSystem<CameraSystem>();
        camera.ChangeCamera(CustomCameraType.FollowCamera);
        camera.SetFollowTarget(AbstractManager.Instance.GetController<PlayerController>().MainPlayer.transform);

    }
}
