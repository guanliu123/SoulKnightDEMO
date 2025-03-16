using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EnumCenter;
using UIFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeScene : SceneBase
{
    public override void Init()
    {
        base.Init();
        scenePath = SceneInfo.HomeScene;
        sceneName = "HomeScene";
        basePanel = new LoadingPanel();
    }

    protected override void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.SceneLoaded(scene, mode);
        PanelManager.Instance.OpenPanel(basePanel);

        //GameManager.Instance.TestPlayer();
        InitNecessaryAsync().Forget();
    }

    private void InitNecessary()
    {
        AbstractManager.Instance.TurnOnCameraAbstract();
        AbstractManager.Instance.TurnOnPlayerAbstract();
        
        //todo:测试敌人
        var t=AbstractManager.Instance.GetController<EnemyController>();
        t.AddEnemyInScene(EnemyType.Stake);
        t.TurnOnController();
    }
    
    private async UniTaskVoid InitNecessaryAsync()
    {
        // 同步初始化
        AbstractManager.Instance.TurnOnCameraAbstract();
        AbstractManager.Instance.TurnOnPlayerAbstract();
        
        // 等待一帧（模拟异步分割）
        await UniTask.Yield();
        
        // 异步生成敌人
        /*var enemyController = AbstractManager.Instance.GetController<EnemyController>();
        enemyController.AddEnemyInScene(EnemyType.Stake); // 假设已实现 UniTask 版本
        enemyController.TurnOnController();*/

        // 后续操作
        Debug.Log("InitNecessary 完成，执行后续操作");
        // 添加你的回调逻辑
        PanelManager.Instance.ClosePanel(UIInfo.LoadingPanel);
        PanelManager.Instance.OpenPanel(new SelectCharacterPanel());
    }
}
