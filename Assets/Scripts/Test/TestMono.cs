using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class TestMono : MonoBehaviour
{
    public EnemyController t;
    // Start is called before the first frame update
    void Start()
    {
        LoadManager.Instance.Init();
        TimerManager.Instance.Init();
        GameManager.Instance.Init();
        TableManager.Instance.Init();
        ConfigData.Init();
        ItemDataCenter.Instance.Init();
        CharacterDataCenter.Instance.Init();
        AbstractManager.Instance.Init();
        t = AbstractManager.Instance.GetController<EnemyController>();
        t.TurnOnController();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            EnemyFactory.Instance.GetEnemy(EnemyType.EliteGoblinGuard);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            EnemyFactory.Instance.GetEnemy(EnemyType.GoblinGuard);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //EnemyFactory.Instance.GetEnemy(EnemyType.GoblinGiant);
            t.AddEnemy(EnemyType.GoblinGiant);
        }
    }
}
