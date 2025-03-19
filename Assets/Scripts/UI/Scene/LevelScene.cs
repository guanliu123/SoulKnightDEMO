using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScene : SceneBase
{
    public override void Init()
    {
        base.Init();
        scenePath = SceneInfo.LevelScene;
        sceneName = "LevelScene";
        basePanel = new LoadingPanel();
    }

    protected override void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        base.SceneLoaded(scene, mode);
        EventManager.Instance.Clear();
        PanelManager.Instance.OpenPanel(basePanel);
        LevelManager.Instance.Init();
        AudioManager.Instance.PlayBKMusic("bgm_1High");
    }
}
