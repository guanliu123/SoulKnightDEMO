using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using TMPro;
using UnityEngine;

public class InteractiveObjectRoot : MonoBehaviour
{
    [HideInInspector]public InteractiveObjectType Type { get;protected set; }

    public GameObject itemIndicator;

    private GameObject collider;

    private bool isInteractable;

    public bool IsInteractable
    {
        get
        {
            return isInteractable;
        }
        set
        {
            if (!value)
            {
                collider = null;
                gameObject.GetComponent<RectCollider>()?.DisableCollision();
            }

            this.enabled = value;
            isInteractable = value;
        }
    }

    protected string objName;

    private void Awake()
    {
        IsInteractable=true;
        gameObject.GetComponent<RectCollider>()?.EnableCollision();
    }

    private void OnEnable()
    {
        itemIndicator.SetActive(false);
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerEnter,gameObject, (obj)=>
        {
            collider = obj;
            itemIndicator.SetActive(true);
            
        });
        TriggerManager.Instance.RegisterObserver(TriggerType.TriggerExit,gameObject, (obj) =>
        {
            collider = null;
            itemIndicator.SetActive(false);
        });
    }

    private void OnDisable()
    {
        itemIndicator.SetActive(false);
        TriggerManager.Instance.RemoveObserver(TriggerType.TriggerEnter,gameObject);
    }

    private void Update()
    {
        if (!isInteractable) return;
        //todo:改成虚拟摇杆操作ETCInput.xxx
        if (collider!=null&&Input.GetKeyDown(KeyCode.F))
        {
            //发送互动事件
            EventManager.Instance.Emit(EventId.ON_INTERACTING_OBJECT,new object[]{this.Type,this});
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*if (!isInteractable) return;

        if (other.CompareTag("Player"))
        {
            collider = other;
            itemIndicator.SetActive(true);
        }*/
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        /*if (!isInteractable) return;

        if (other.CompareTag("Player"))
        {
            collider = null;
            itemIndicator.SetActive(false);
        }*/
    }
}
