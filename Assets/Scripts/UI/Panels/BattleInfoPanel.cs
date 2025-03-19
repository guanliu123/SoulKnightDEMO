using System.Collections;
using System.Collections.Generic;
using cfg;
using EnumCenter;
using TMPro;
using UIFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfoPanel : BasePanel
{
    private PlayerBase player;
    private Slider hpSlider;
    private Slider mpSlider;
    private Slider shieldSlider;
    private TMP_Text hpText;
    private TMP_Text mpText;
    private TMP_Text shieldText;

    
    //private Character characterCfg;
    public BattleInfoPanel() : base(new UIType(UIInfo.BattleInfoPanel))
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        var camera = AbstractManager.Instance.GetSystem<CameraSystem>();
        camera.ChangeCamera(CustomCameraType.FollowCamera);

        player = AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
        camera.SetFollowTarget(player.transform);

        hpSlider = FindComponent<Slider>("Slider_HP");
        mpSlider = FindComponent<Slider>("Slider_Mana");
        shieldSlider = FindComponent<Slider>("Slider_Shield");
        
        hpText=FindComponent<TMP_Text>("Text_HP");
        mpText=FindComponent<TMP_Text>("Text_Mana");
        shieldText=FindComponent<TMP_Text>("Text_Shield");
        
        MonoManager.Instance.AddUpdateAction(ListenerPlayerState);
    }

    private void ListenerPlayerState()
    {
        var att = player.Attribute as PlayerAttribute;
        
        hpSlider.value =(float)att.CurrentHp/att.MaxHp;
        hpText.text = att.CurrentHp+"/"+att.MaxHp;
        
        mpSlider.value =(float)att.CurrentMp/att.MaxMp;
        mpText.text = att.CurrentMp+"/"+att.MaxMp;
        
        shieldSlider.value =(float)att.CurrentShield/att.MaxShield;
        shieldText.text = att.CurrentShield+"/"+att.MaxShield;
    }

    public override void OnExit()
    {
        base.OnExit();
        MonoManager.Instance.RemoveUpdateAction(ListenerPlayerState);
    }
}
