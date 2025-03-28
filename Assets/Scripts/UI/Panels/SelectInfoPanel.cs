using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using TMPro;
using UIFrameWork;
using UnityEngine;
using UnityEngine.UI;

public class SelectInfoPanel : BasePanel
{
    private PlayerType playerType;
    
    public SelectInfoPanel(PlayerType _playerType) : base(new UIType(UIInfo.SelectInfoPanel))
    {
        playerType = _playerType;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        FindComponent<Button>("Btn_Back").onClick.AddListener(() =>
        {
            AbstractManager.Instance.GetSystem<CameraSystem>().ChangeCamera(CustomCameraType.StaticCamera);
            PanelManager.Instance.ClosePanel(UIInfo.SelectInfoPanel);
        });
        FindComponent<Button>("Btn_Select").onClick.AddListener(() =>
        {
            var t = AbstractManager.Instance.GetController<PlayerController>();
            t.SetMainPlayer(playerType);
            t.TurnOnController();
            PanelManager.Instance.ClosePanel(UIInfo.SelectInfoPanel);
            PanelManager.Instance.ClosePanel(UIInfo.SelectCharacterPanel);
            PanelManager.Instance.OpenPanel(new BattleInfoPanel());
        });

        var characterCfg = CharacterDataCenter.Instance.GetPlayerData(playerType);
        FindComponent<TMP_Text>("CharacterName").text = characterCfg.Name;
        FindComponent<TMP_Text>("TextHp").text = characterCfg.HP.ToString();
        FindComponent<TMP_Text>("TextArmor").text = characterCfg.Shield.ToString();
        FindComponent<TMP_Text>("TextMp").text = characterCfg.MP.ToString();
    }
}
