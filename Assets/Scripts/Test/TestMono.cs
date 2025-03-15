using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    public GameObject obj;
    // Start is called before the first frame update
    void Start()
    {
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,obj, (obj) =>
        {
            LogTool.Log(obj.name+"碰撞到了！！！");
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
