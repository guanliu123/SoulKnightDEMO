using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbstractManager : SingletonBase<AbstractManager>
{
    private List<AbstractController> controllers;
    private List<AbstractSystem> systems;

    public AbstractManager()
    {
        controllers = new();
        systems = new();

        RegisterInitAbstract();
    }
    
    public void RegisterInitAbstract()
    {
        //提前注册所有必要的Abstract类
        RegisterController(new ItemController());
        RegisterController(new EnemyController());
        RegisterController(new RoomController());
        //todo:结束战斗时记得注销事件
        EventManager.Instance.SingOn(EventId.MAP_GENERATION_COMPLETED, RegisterRoomController);
    }

    private void RegisterRoomController()
    {
        GetController<RoomController>().TurnOnController();
    }

    public void RegisterCameraAbstract()
    {
        RegisterSystem(new CameraSystem());
    }

    //注册玩家相关
    public void RegisterPlayerAbstract()
    {
        RegisterController(new InputController());
        RegisterController(new PlayerController());
    }

    public void OnUpdate()
    {
        foreach (var item in controllers)
        {
            item.GameUpdate();
        }
        foreach (var item in systems)
        {
            item.GameUpdate();
        }
    }

    public void RegisterController(AbstractController controller)
    {
        controllers.Add(controller);
    }

    public void RegisterSystem(AbstractSystem system)
    {
        systems.Add(system);
    }

    public T GetController<T>() where T : AbstractController
    {
        foreach (var item in controllers)
        {
            if (item is T)
            {
                return item as T;
            }
        }

        return default(T);
    }
    
    public T GetSystem<T>() where T : AbstractSystem
    {
        foreach (var item in systems)
        {
            if (item is T)
            {
                return item as T;
            }
        }

        return default(T);
    }
}
