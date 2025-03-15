using System;
using System.Collections;
using System.Collections.Generic;
using EnumCenter;
using UnityEngine;
using UnityEngine.Events;

public class TriggerDetection : MonoBehaviour
{
    private Dictionary<GameObject, UnityAction<GameObject>> enterDic = new();
    private Dictionary<GameObject, UnityAction<GameObject>> exitDic = new();
    
    //储存对应标签需要做的事
    private Dictionary<string, UnityAction<GameObject>> enterTagDic = new();
    private Dictionary<string, UnityAction<GameObject>> exitTagDic = new();

    public void OnTriggerEnter2D(Collider2D other)
    {
        TriggerManager.Instance.NotisfyObserver(TriggerType.TriggerEnter, gameObject, other.gameObject);

        if (enterDic.ContainsKey(other.gameObject))
        {
            enterDic[other.gameObject]?.Invoke(other.gameObject);
        }

        if (enterTagDic.ContainsKey(other.gameObject.tag))
        {
            enterTagDic[other.gameObject.tag]?.Invoke(other.gameObject);
        }
    }

    /*public void OnTriggerEnter2D(Collider other)
    {
        if (enterDic.ContainsKey(other.gameObject))
        {
            enterDic[other.gameObject]?.Invoke(other.gameObject);
        }

        if (enterTagDic.ContainsKey(other.gameObject.tag))
        {
            enterTagDic[other.gameObject.tag]?.Invoke(other.gameObject);
        }
    }*/

    public void OnTriggerExit2D(Collider2D other)
    {
        TriggerManager.Instance.NotisfyObserver(TriggerType.TriggerExit, gameObject, other.gameObject);

        if (exitDic.ContainsKey(other.gameObject))
        {
            exitDic[other.gameObject]?.Invoke(other.gameObject);
        }

        if (exitTagDic.ContainsKey(other.gameObject.tag))
        {
            exitTagDic[other.gameObject.tag]?.Invoke(other.gameObject);
        }
    }

    public void AddTriggerListener(TriggerType type,GameObject target,UnityAction<GameObject> callback)
    {
        switch (type)
        {
            case TriggerType.TriggerEnter:
                if (enterDic.ContainsKey(target))
                {
                    enterDic[target] += callback;
                }
                else
                {
                    enterDic.Add(target,callback);
                }
                break;
            case TriggerType.TriggerExit:
                if (exitDic.ContainsKey(target))
                {
                    exitDic[target] += callback;
                }
                else
                {
                    exitDic.Add(target,callback);
                }
                break;
        }
    }

    public void AddTriggerListener(TriggerType type,string tag,UnityAction<GameObject> callback)
    {
        switch (type)
        {
            case TriggerType.TriggerEnter:
                if (enterTagDic.ContainsKey(tag))
                {
                    enterTagDic[tag] += callback;
                }
                else
                {
                    enterTagDic.Add(tag,callback);
                }
                break;
            case TriggerType.TriggerExit:
                if (exitTagDic.ContainsKey(tag))
                {
                    exitTagDic[tag] += callback;
                }
                else
                {
                    exitTagDic.Add(tag,callback);
                }
                break;
        }
    }

    public void OnExit()
    {
        enterDic.Clear();
        exitDic.Clear();
        enterTagDic.Clear();
        exitTagDic.Clear();
    }
}
