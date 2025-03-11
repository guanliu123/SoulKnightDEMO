using System.Collections.Generic;
using EnumCenter;

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
}
