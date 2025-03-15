using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static MainThreadDispatcher _instance;
    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("MainThreadDispatcher");
                _instance = go.AddComponent<MainThreadDispatcher>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private readonly Queue<Action> _executionQueue = new Queue<Action>();
    private readonly object _lock = new object();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Enqueue(Action action)
    {
        lock (_lock)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_lock)
        {
            while (_executionQueue.Count > 0)
            {
                Action action = _executionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }
}