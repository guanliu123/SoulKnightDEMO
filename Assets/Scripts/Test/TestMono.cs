using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadManager.Instance.Init();
        TimerManager.Instance.Init();
        TableManager.Instance.Init();
        ConfigData.Init();
        CharacterDataCenter.Instance.Init();
        AbstractManager.Instance.GetController<EnemyController>().TurnOnController();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            EnemyFactory.Instance.GetEnemy(EnemyType.EliteGoblinGuard);
        }
    }
}
