using System;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    public LoginPanel() : base(new UIType(UIInfo.LoginPanel))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        FindComponent<Button>("Btn_Login").onClick.AddListener(OnLogin);
        FindComponent<Button>("Btn_Close").onClick.AddListener(Close);
    }

    public void OnLogin()
    {
        //todo:使用netmanager发送协议登录，接收返回的数据,使用UserData.UpdateData更新
    }
}
