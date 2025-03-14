using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase
{
    public bool IsBattle { get;protected set; }
    public GameObject gameObject { get; protected set; }
    public Transform transform => gameObject.transform;
    public CharacterRoot Root { get; protected set; }
    
    public CharacterAttribute Attribute { get; protected set; }
    
    //检测子弹的物体
    public GameObject TriggerBox { get;protected set; }

    private bool isLeft;
    public bool IsLeft
    {
        get => isLeft;
        set
        {
            if (value)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }

            isLeft = value;
        }
    }
    private bool isInit;
    private bool isStart;
    private bool isShouldRemove;
    private bool isAlreadyRemove;

    public void SetIsBattle(bool isBattle)
    {
        IsBattle = isBattle;
    }
    
    public CharacterBase(GameObject obj)
    {
        gameObject = obj;
        Root = gameObject.GetComponent<CharacterRoot>();
        if (Root == null)
        {
            LogTool.LogError($"角色身上未挂载CharacterRoot！");
        }
        Root.SetCharacter(this);

        TriggerBox = Root.GetTriggerBox();
    }

    public void GameUpdate()
    {
        if (!isInit)
        {
            isInit = true;
            OnInit();
        }
        OnCharacterUpdate();
    }
    
    protected virtual void OnInit(){}
    protected virtual void OnCharacterStart(){}

    protected virtual void OnCharacterUpdate()
    {
        if (!isStart)
        {
            isStart = true;
            OnCharacterStart();
        }
    }
    protected virtual void OnCharacterDieStart(){}
    protected virtual void OnCharacterDieUpdate(){}

    public virtual void UnderAttack(int damage)
    {
        Attribute.CurrentHp-=damage;
        ItemFactory.Instance.GetPopupNum(transform.position).SetText(damage);
    }

    public void Remove()
    {
        isShouldRemove = true;
    }
}
