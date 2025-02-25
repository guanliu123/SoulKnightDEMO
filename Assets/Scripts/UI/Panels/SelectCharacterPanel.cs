using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UIFrameWork;
using UnityEngine;

public class SelectCharacterPanel : BasePanel
{
    //当前选中的角色
    private Collider2D collider;
    private CameraSystem system;
    
    public SelectCharacterPanel() : base(new UIType(UIInfo.SelectCharacterPanel))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        system = AbstractManager.Instance.GetSystem<CameraSystem>();
        MonoManager.Instance.AddUpdateAction(OnCheckSelect);
    }

    private void OnCheckSelect()
    {
        //todo:改成手机检测屏幕点击事件的方法
        if (Input.GetMouseButtonDown(0))
        {
            collider = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.1f,
                LayerMask.GetMask("Player"));
            if (collider)
            {
                system.SetSelectTarget(collider.transform);
                system.ChangeCamera(CustomCameraType.SelectCamera);
                PanelManager.Instance.OpenPanel
                    (new SelectInfoPanel(collider.transform.parent.gameObject.GetComponent<CharacterRoot>().characterType));
            }
        }
    }
}
