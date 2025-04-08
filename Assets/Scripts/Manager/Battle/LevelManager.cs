using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;

public class LevelManager : SingletonBase<LevelManager>
{
    //第几大关
    public int NowLevelNumber { get; private set; }
    //关卡ID，方便读取房间
    public int NowLevelID { get; private set; }
    //第几小关
    public int NowStage { get; private set; }

    public override void Init()
    {
        base.Init();
        NowLevelNumber = 1;
        
        MapManager.Instance.Init();

        GenerateLevel();
    }
    
    private void RandomLevel()
    {
        var levelList = TableManager.Instance.Tables.TBLevel.DataList;
        List<int> randomIdList = new List<int>();
        foreach (var item in levelList)
        {
            if (item.Levelnumber == NowLevelNumber)
            {
                randomIdList.Add(item.Id);
            }
        }

        int index = Random.Range(0, randomIdList.Count);

        NowLevelID = randomIdList[index];
        NowStage++;
    }

    private void GenerateLevel()
    {
        PanelManager.Instance.OpenPanel(new LoadingPanel());

        RandomLevel();
        MapManager.Instance.StartGenerateMap();
    }
    
    protected override void RegisterEvent()
    {
        EventManager.Instance.SingOn(EventId.MAPMANAGER_CONFIG_UPDATE_COMPLETED,OnMapManagerInitCompleted);
        EventManager.Instance.SingOn(EventId.MAP_GENERATION_COMPLETED,OnMapGenerateCompleted);
        EventManager.Instance.SingOn(EventId.PlayerDie,Settlement);
        
        EventManager.Instance.SingOn(EventId.GenerateLevel,GenerateLevel);
    }

    protected override void UnregisterEvent()
    {
        EventManager.Instance.SingOff(EventId.MAPMANAGER_CONFIG_UPDATE_COMPLETED,OnMapManagerInitCompleted);
        EventManager.Instance.SingOff(EventId.MAP_GENERATION_COMPLETED,OnMapGenerateCompleted);
        EventManager.Instance.SingOff(EventId.PlayerDie,Settlement);
        
        EventManager.Instance.SingOff(EventId.GenerateLevel,GenerateLevel);
    }

    #region 事件函数
    
    private void OnMapManagerInitCompleted()
    {
        LogTool.Log("开始生成地图");
        //MonoManager.Instance.StartCoroutine(MapManager.Instance.GenerateMap());
        MapManager.Instance.GenerateMap();
    }
    private void OnMapGenerateCompleted()
    {
        //经过2s再关闭加载界面
        TimerManager.Register(2f, () =>
        {
            PanelManager.Instance.ClosePanel(UIInfo.LoadingPanel);
            PanelManager.Instance.OpenPanel(new BattleInfoPanel());
        });
    }

    #endregion

    public void Settlement()
    {
        EventManager.Instance.Emit(EventId.BackToHome);
    }
}
