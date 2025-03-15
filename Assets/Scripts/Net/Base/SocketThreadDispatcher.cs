using System;
using System.Collections.Generic;
using UnityEngine;

public class SocketThreadDispatcher : MonoBehaviour
{
    private static SocketThreadDispatcher _instance;
    private readonly Queue<Action> _executionQueue = new Queue<Action>();
    private readonly object _lock = new object();

    public static SocketThreadDispatcher Instance()
    {
        if (_instance == null)
        {
            _instance = FindObjectOfType<SocketThreadDispatcher>();
            if (_instance == null)
            {
                GameObject go = new GameObject("SocketThreadDispatcher");
                _instance = go.AddComponent<SocketThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
        }
        return _instance;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (_lock)
        {
            _executionQueue.Enqueue(action);
        }
    }
}