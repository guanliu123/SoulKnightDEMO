// using Game;
// using StarkSDKSpace;
using System.Collections;
using System.Collections.Generic;
using UIFrameWork;
using UnityEngine;
using UnityEngine.SceneManagement;
//游戏的根管理器
public class GameRoot : MonoBehaviour
{
    public string host="localhost";
    public static GameRoot Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        InitNecessaryComponents();
        //Application.targetFrameRate = 60;
    }
    private void Start()
    {
        //进入时是什么场景就加载对应脚本
        if (SceneManager.GetActiveScene().name == "InitScene")
        {
            SceneSystem.Instance.SetScene(new InitScene());
        }
    }

    public void SwitchScene(SceneBase scene)
    {
        SceneSystem.Instance.SetScene(scene);
    }

    public void ChangeScene(string sceneName)
    {
        //SceneManager.LoadScene(sceneName);
        string sn = "Resources_moved/" + sceneName;
        StartCoroutine(Delay(sn));
    }

    private IEnumerator Delay(string sceneName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
        while (!ao.isDone)
        {
            yield return new WaitForSeconds(3.0f);
        }
    }

    public void StartBattle()
    {
         PanelManager.Instance.OpenPanel(new LoadingPanel());
         LevelManager.Instance.Init();
    }

    private void InitNecessaryComponents()
    {
        //NetInit();
        LoadManager.Instance.Init();
        TimerManager.Instance.Init();
        TableManager.Instance.Init();
        ConfigData.Init();
        EventManager.Instance.Init();
    }
    
    public void NetInit()
    {
        //这里后面要把host配置一下
        NetManager.Instance.Init();
        NetManager.Instance.Connect(host);
        NetReciver.Instance.Init(new ResponseRegister());
    }
}
