using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;

public class EnemyController : AbstractController
{
    protected List<EnemyBase> enemys;
    public EnemyController(){}

    protected override void Init()
    {
        base.Init();
        enemys = new();
    }


    protected override void OnAfterRunUpdate()
    {
        for (int i = 0; i < enemys.Count; i++)
        {
            enemys[i].GameUpdate();

            /*if (enemys[i].IsAlreadyRemove == false)
            {
                enemys[i].GameUpdate();
            }
            else
            {
                enemys.RemoveAt(i);
            }*/
        }
    }

    public void AddEnemy(EnemyType type)
    {
        enemys.Add(EnemyFactory.Instance.GetEnemy(type));
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

        enemy = EnemyFactory.Instance.GetEnemy(EnemyType.Stake);
        enemy.room = room;
        enemy.gameObject.transform.position = pos;
        enemy.isWork = isWork;
        enemys.Add(enemy);
    }
}
