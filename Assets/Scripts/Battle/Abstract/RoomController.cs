using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edgar.Unity;
using EnumCenter;
using UnityEngine;

public class Room
{
    //地图生成器里生成的房间实例
    public RoomInstanceGrid2D roomInstance;
    public int CurrentEnemyNum;//由于enemys延迟加入，需要这个变量获取加入后的数量
    public int WaveNum = Random.Range(2, 5);//波次
    private int spawnEnemyNum = Random.Range(5, 14);//敌人数量
    public int SpawnEnemyNum => spawnEnemyNum + WaveNum; 
}

public class RoomController : AbstractController
{
    private List<RoomInstanceGrid2D> RoomInstances;
    private EnemyController enemyController;
    public Room EnterRoom { get;private set; }
    private PlayerBase player;

    private PetBase pet;
    //private Animator m_FinishAnim;
    private bool isStart;
    private bool isEnterEnemyFloor;
    private bool isEnterEnemyFloorStart;
    private bool isClearEnemyStart;
    public RoomController()
    {
        //m_FinishAnim = UnityTool.Instance.GetGameObjectFromCanvas("Finish").GetComponent<Animator>();
    }
    protected override void Init()
    {
        base.Init();
        enemyController = AbstractManager.Instance.GetController<EnemyController>();
    }

    public override void RegisterEvents()
    {
        base.RegisterEvents();
        EventManager.Instance.On<Room>(EventId.EnemyDie,CheckRoomRefresh);
        EventManager.Instance.On(EventId.ToNextLevel,ResetData);
    }

    public override void UnregisterEvents()
    {
        base.UnregisterEvents();
        EventManager.Instance.Off<Room>(EventId.EnemyDie,CheckRoomRefresh);
        EventManager.Instance.Off(EventId.ToNextLevel,ResetData);
    }

    private void ResetData()
    {
        isStart = false;
        isEnterEnemyFloor = false;
    }

    private void CheckRoomRefresh(Room room)
    {
        if (room == EnterRoom)
        {
            EnterRoom.CurrentEnemyNum--;
        }
    }
    protected override void OnAfterRunUpdate()
    {
        base.OnAfterRunUpdate();
        if (AbstractManager.Instance.GetController<PlayerController>().MainPlayer != null && !isStart)
        {
            isStart = true;
            player = AbstractManager.Instance.GetController<PlayerController>().MainPlayer;
            pet=AbstractManager.Instance.GetController<PlayerController>().pets[0];
            RoomInstances = MapManager.Instance.gameObject.GetComponent<RoomPostProcessing>().GetRoomInstances();
            foreach (RoomInstanceGrid2D roomInstance in RoomInstances)
            {
                Room room = new Room();
                room.roomInstance = roomInstance;
                if ((roomInstance.Room as CustomRoom).RoomType == RoomType.BirthRoom)
                {
                    player.gameObject.transform.position = GameTool.GetComponentFromChild<CompositeCollider2D>(roomInstance.RoomTemplateInstance, "Floor").bounds.center;
                    pet.ResetPos();
                }
                else if ((roomInstance.Room as CustomRoom).RoomType == RoomType.EnemyRoom)
                {
                    SpawnEnemies(room, false);
                }
                else if ((roomInstance.Room as CustomRoom).RoomType == RoomType.TreasureRoom)
                {
                    CreateOtherTreasureBox(room);
                }
                else if ((roomInstance.Room as CustomRoom).RoomType == RoomType.BossRoom)
                {
                    room.WaveNum = 0;
                    //enemyController.AddBoss(room, GetFloorCenter(room));
                }
                TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter, player.root.GetTriggerDetection().gameObject, GetFloorCollider(room).gameObject, obj =>
                {
                    RoomType roomType = (roomInstance.Room as CustomRoom).RoomType;
                    if (roomType == RoomType.EnemyRoom ||roomType == RoomType.BossRoom)
                    {
                        EnterRoom = room;
                        isEnterEnemyFloor = true;
                        isEnterEnemyFloorStart = false;
                        isClearEnemyStart = false;
                    }
                });
                TriggerManager.Instance.RegisterObserver(TriggerType.TriggerExit, player.root.GetTriggerDetection().gameObject, GetFloorCollider(room).gameObject, obj =>
                {
                    RoomType roomType = (roomInstance.Room as CustomRoom).RoomType;
                    if (roomType == RoomType.EnemyRoom || roomType == RoomType.BossRoom)
                    {
                        isEnterEnemyFloor = false;
                    }
                });
            }
        }
    }
    protected override void AlwaysUpdate()
    {
        base.AlwaysUpdate();
        if (isEnterEnemyFloor)
        {
            RoomType roomType = (EnterRoom.roomInstance.Room as CustomRoom).RoomType;
            if (IsPlayerInFloor(GetFloorCollider(EnterRoom).bounds, player.root.GetTriggerDetection().GetComponent<CapsuleCollider2D>().bounds))
            {
                if (!isEnterEnemyFloorStart)
                {
                    isEnterEnemyFloorStart = true;

                    if (roomType != RoomType.BossRoom)
                    {
                        EventManager.Instance.Emit(EventId.OnPlayerEnterBattleRoom, EnterRoom);
                    }
                    else
                    {
                        EventManager.Instance.Emit(EventId.OnPlayerEnterBossRoom, EnterRoom);
                    }
                    CloseDoor(EnterRoom.roomInstance);
                }
            }
            if (EnterRoom.CurrentEnemyNum == 0 && EnterRoom.WaveNum > 0)
            {
                //todo：精英怪房先注释了
                /*if ((EnterRoom.roomInstance.Room as CustomRoom).RoomType == RoomType.EliteEnemyRoom)
                {
                    SpawnEnemies(EnterRoom, true);
                }
                else
                {
                    SpawnEnemies(EnterRoom, false);
                }*/
                SpawnEnemies(EnterRoom, false);
            }
            else if (EnterRoom.CurrentEnemyNum == 0 && !isClearEnemyStart)
            {
                isClearEnemyStart = true;
                CreateWhiteTreasureBox(EnterRoom);
                ShowBattleFinishAnim();
                OpenDoor(EnterRoom.roomInstance);
                TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter, player.root.GetTriggerDetection().gameObject, GetFloorCollider(EnterRoom).gameObject);
            }
        }
    }
    private void CloseDoor(RoomInstanceGrid2D roomInstance)
    {
        foreach (DoorInstanceGrid2D door in roomInstance.Doors)
        {
            GameObject roomObj = door.ConnectedRoomInstance.RoomTemplateInstance;
            SetDoorAnimator(roomObj, true);
        }
        player.SetIsBattle(true);
    }
    private void ShowBattleFinishAnim()
    {
        /*m_FinishAnim.gameObject.SetActive(true);
        m_FinishAnim.Play("Finish", 0, 0);
        CoroutinePool.Instance.StartAnimatorCallback(m_FinishAnim, "Finish", () =>
        {
            m_FinishAnim.gameObject.SetActive(false);
        });*/
    }
    private void OpenDoor(RoomInstanceGrid2D roomInstance)
    {
        player.SetIsBattle(false);
        foreach (DoorInstanceGrid2D door in roomInstance.Doors)
        {
            GameObject roomObj = door.ConnectedRoomInstance.RoomTemplateInstance;
            SetDoorAnimator(roomObj, false);
        }
    }
    private void CreateWhiteTreasureBox(Room room)
    {
        //todo:获取宝箱，先注释了
        LogTool.Log("生成白宝箱");
        //ItemFactory.Instance.GetTreasureBox(TreasureBoxType.White, RandomPointInBounds(GetFloorCollider(room).bounds, 3));
    }
    private void CreateOtherTreasureBox(Room room)
    {
        //todo:宝箱相关，先注释了
        /*if (ModelContainer.Instance.GetModel<MemoryModel>().Stage < 6)
        {
            ItemFactory.Instance.GetTreasureBox(TreasureBoxType.Brown, GetFloorCenter(room));
        }
        else
        {
            ItemFactory.Instance.GetTreasureBox(TreasureBoxType.Blue, GetFloorCenter(room));
        }*/
    }
    private void SetDoorAnimator(GameObject roomObj, bool isUp)
    {
        GameObject d = null;

        if ((d = GameTool.GetGameObjectFromChildren(roomObj, "LeftVerDownDoor")) != null)
        {
            d.GetComponent<Animator>().SetBool("isUp", isUp);
        }
        if ((d =GameTool.GetGameObjectFromChildren(roomObj, "RightVerDownDoor")) != null)
        {
            d.GetComponent<Animator>().SetBool("isUp", isUp);
        }
        if ((d = GameTool.GetGameObjectFromChildren(roomObj, "TopHorDownDoor")) != null)
        {
            d.GetComponent<Animator>().SetBool("isUp", isUp);
        }
        if ((d = GameTool.GetGameObjectFromChildren(roomObj, "BottomHorDownDoor")) != null)
        {
            d.GetComponent<Animator>().SetBool("isUp", isUp);
        }
    }
    private bool IsPlayerInFloor(Bounds floorBounds, Bounds playerBounds)
    {
        Vector2 dir = playerBounds.center - floorBounds.center;
        if (floorBounds.Contains(playerBounds.center))
        {

            if (dir.y < 0 && Mathf.Abs(dir.x) < floorBounds.extents.x - 2)//down
            {
                if (dir.y > -floorBounds.extents.y + 1.5)
                {
                    return true;
                }
            }
            else if (dir.x < 0 && Mathf.Abs(dir.y) < floorBounds.extents.y - 2)//left
            {
                if (dir.x > -floorBounds.extents.x)
                {
                    return true;
                }
            }
            else if (dir.x > 0 && Mathf.Abs(dir.y) < floorBounds.extents.y - 2)//right
            {
                if (dir.x < floorBounds.extents.x)
                {
                    return true;
                }
            }
            else
            {
                if (dir.y < floorBounds.extents.y)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void SpawnEnemies(Room room, bool isElite)
    {
        CompositeCollider2D FloorCollider = GetFloorCollider(room);
        var totalEnemiesCount = room.SpawnEnemyNum;
        while (room.CurrentEnemyNum < totalEnemiesCount)
        {
            // Find random position inside floor collider bounds
            var position = RandomPointInBounds(FloorCollider.bounds, 2f);

            // Check if the point is actually inside the collider as there may be holes in the floor, and the point is not in the wall.
            if (!FloorCollider.OverlapPoint(position))
            {
                continue;
            }

            // We want to make sure that there is no other collider in the radius of 1
            if (Physics2D.OverlapCircleAll(position, 1f).Any(x => !x.isTrigger))
            {
                continue;
            }
            // Pick random enemy prefab

            // Create an instance of the enemy and set position and parent
            enemyController.SpawnEnemy(room, position, isEnterEnemyFloorStart, isElite);
        }
        room.WaveNum--;
    }

    private Vector3 RandomPointInBounds(Bounds bounds, float margin = 0)
    {
        return new Vector3(
            RandomRange(bounds.min.x + margin, bounds.max.x - margin),
            RandomRange(bounds.min.y + margin, bounds.max.y - margin),
            RandomRange(bounds.min.z + margin, bounds.max.z - margin)
        );
    }

    private static float RandomRange(float min, float max)
    {
        return (float)(Random.Range(0f, 1f) * (max - min) + min);
    }
    private Vector2 GetFloorCenter(Room room)
    {
        return GameTool.GetComponentFromChild<CompositeCollider2D>(room.roomInstance.RoomTemplateInstance, "Floor").bounds.center;
    }
    public Vector3 RandomPointInFloor(Room room, float margin = 0)
    {
        CompositeCollider2D collider = GetFloorCollider(room);
        while (true)
        {
            Vector2 position = RandomPointInBounds(collider.bounds, margin);
            if (!collider.OverlapPoint(position))
            {
                continue;
            }
            // We want to make sure that there is no other collider in the radius of 1
            if (Physics2D.OverlapCircleAll(position, 1f).Any(x => !x.isTrigger))
            {
                continue;
            }
            return position;
        }
    }
    public CompositeCollider2D GetFloorCollider(Room room)
    {
        return GameTool.GetComponentFromChild<CompositeCollider2D>(room.roomInstance.RoomTemplateInstance, "Floor");
    }
}
