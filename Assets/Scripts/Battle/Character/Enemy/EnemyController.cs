using System.Collections;
using System.Collections.Generic;
using Edgar.Unity;
using EnumCenter;
using UnityEngine;

public class EnemyController : AbstractController
{
    protected List<EnemyBase> enemys;

    //当前房间跟玩家在一起的敌人
    public List<EnemyBase> enemysInRoom { get;protected set; }

    private int[][] enemyData;

    public EnemyController()
    {
        enemys = new();
        enemysInRoom = new();
    }

    protected override void Init()
    {
        base.Init();
        EventManager.Instance.SingOn(EventId.ToNextLevel,UpdateEnemyData);
    }

    private void UpdateEnemyData()
    {
        var t = TableManager.Instance.Tables.TBLevel[LevelManager.Instance.NowLevelID].Enemys;
        var list = t.Split(",");
        enemyData=new int[list.Length][];
        for (int i = 0; i < list.Length; i++)
        {
            var temp = list[i].Split("|");
            enemyData[i]=new int[2];
            enemyData[i][0] = int.Parse(temp[0]);
            enemyData[i][1]=int.Parse(temp[1]);
        }
    }


    protected override void OnAfterRunUpdate()
    {
        for (int i = 0; i < enemys.Count; i++)
        {
            //enemys[i].GameUpdate();

            if (enemys[i].IsAlreadyRemove == false)
            {
                enemys[i].GameUpdate();
            }
            else
            {
                enemys.RemoveAt(i);
            }
        }
    }

    public void AddEnemy(EnemyType type)
    {
        var t = EnemyFactory.Instance.GetEnemy(type);
        enemys.Add(t);
        t.isWork = true;
    }

    public void AddEnemyInScene(EnemyType type)
    {
        enemys.Add(EnemyFactory.Instance.GetEnemyInScene(type));
    }
    
    /*public void AddBoss(Room room, Vector2 pos)
    {
        IBoss boss = EnemyFactory.Instance.GetBoss(BossType.DevilSnare);
        boss.m_Room = room;
        boss.m_Room.CurrentEnemyNum += 1;
        boss.transform.position = pos;
        bosses.Add(boss);
    }*/
    public void SpawnEnemy(Room room, Vector2 pos, bool isWork, bool isElite)
    {
        MonoManager.Instance.StartCoroutine(WaitForSpawnEnemy(room, pos, isWork, isElite));
        room.CurrentEnemyNum += 1;
    }
    private IEnumerator WaitForSpawnEnemy(Room room, Vector2 pos, bool isWork, bool isElite)
        {
            ItemFactory.Instance.GetEffect(EffectType.Pane, pos).AddToController();
            yield return new WaitForSeconds(0.8f);
            EnemyBase enemy = null;
            /*if (isElite)
            {
                enemy = EnemyFactory.Instance.GetEliteEnemy();
            }
            else
            {
                enemy = EnemyFactory.Instance.GetRandomEnemy();
            }*/
            int randomE = GetRandomEnemyByWeight();
            enemy = EnemyFactory.Instance.GetEnemy((EnemyType)randomE);
            enemy.room = room;
            enemy.gameObject.transform.position = pos;
            enemy.isWork = isWork;
            enemys.Add(enemy);

            if (room == AbstractManager.Instance.GetController<RoomController>().EnterRoom)
            {
                enemysInRoom.Add(enemy);
            }
        }
    
    int GetRandomEnemyByWeight()
    {
        if (enemyData == null)
        {
            UpdateEnemyData();
        }
        // 计算总权重
        int totalWeight = 0;
        foreach (int[] enemy in enemyData)
        {
            totalWeight += enemy[1];
        }

        // 生成随机数（0 <= randomValue < totalWeight）
        int randomValue = Random.Range(0, totalWeight);

        // 累计权重检测
        int cumulativeWeight = 0;
        foreach (int[] enemy in enemyData)
        {
            cumulativeWeight += enemy[1];
            if (cumulativeWeight > randomValue)
            {
                return enemy[0];
            }
        }

        return -1; // 理论上不会执行到这里
    }

    public Transform GetNearestEnemy(Vector3 playerPos)
    {
        float minDist = float.MaxValue;
        EnemyBase nearest = null;
        for (int i = 0; i < enemysInRoom.Count; i++)
        {
            enemysInRoom[i].SetLocked(false);

            var dis = Vector3.Distance(playerPos, enemysInRoom[i].transform.position);
            if (enemysInRoom[i]!=null&&!enemysInRoom[i].IsAlreadyRemove)
            {
                if(dis<minDist&&dis<10f)
                {
                    minDist=dis;
                    nearest = enemysInRoom[i];
                }
            }
            else
            {
                enemysInRoom.RemoveAt(i);
            }
        }
        
        nearest?.SetLocked(true);
        return nearest?.transform;
    }

    public void AddInRoom(EnemyBase e)
    {
        enemysInRoom.Add(e);
    }
}
