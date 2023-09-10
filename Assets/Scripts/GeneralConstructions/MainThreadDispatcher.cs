using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using WasderGQ.Sudoku.Generic;


public class MainThreadDispatcher : Singleton<MainThreadDispatcher>
{
    private static Queue<Action> _actions = new Queue<Action>();
    private void Update()
    {
        lock (_actions)
        {
            while (_actions.Count > 0)
            {
                Action action = _actions.Dequeue();
                action.Invoke();
            }
        }
    }
    public static void Enqueue(Action action)
    {
        lock (_actions)
        {
            _actions.Enqueue(action);
        }
    }
}