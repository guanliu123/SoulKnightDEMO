using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UIFrameWork;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (!EventSystem.current.IsPointerOverGameObject()&& Input.GetMouseButtonDown(0))
        {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = 10;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            collider = Physics2D.OverlapCircle(worldPos, 0.1f, LayerMask.GetMask("Player"));
            if (collider)
            {
                LogTool.Log(collider);
                system.SetSelectTarget(collider.transform);
                system.ChangeCamera(CustomCameraType.SelectCamera);
                PanelManager.Instance.OpenPanel 
                    (new SelectInfoPanel(collider.transform.parent.gameObject.GetComponent<CharacterRoot>().playerType));
            }
        }
    }

    public override void OnPause()
    {
        base.OnPause();
        MonoManager.Instance.RemoveUpdateAction(OnCheckSelect);
    }

    public override void OnResume()
    {
        base.OnResume();
        MonoManager.Instance.AddUpdateAction(OnCheckSelect);
    }

    public override void OnExit()
    {
        base.OnExit();
        MonoManager.Instance.RemoveUpdateAction(OnCheckSelect);
    }
}
