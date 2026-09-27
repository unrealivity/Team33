using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    private readonly List<TimedTask> activeTasks = new();
    private readonly List<TimedTask> pendingRemovals = new();
    private bool _isTicking;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance != this) return;
        Instance = null;
    }

    public void Tick(float deltaTime)
    {
        _isTicking = true;
        for (int index = 0; index < activeTasks.Count; index++)
        {
            activeTasks[index].Tick(deltaTime);
        }
        _isTicking = false;

        if (pendingRemovals.Count > 0)
        {
            foreach (var task in pendingRemovals)
                activeTasks.Remove(task);
            pendingRemovals.Clear();
        }
    }

    public void AddTask(TimedTask task) => activeTasks.Add(task);

    public void RemoveTask(TimedTask task)
    {
        if (_isTicking)
            pendingRemovals.Add(task);
        else
            activeTasks.Remove(task);
    }
}
