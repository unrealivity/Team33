using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager
{
    private readonly List<TimedTask> activeTasks = new();
    
    public void Tick(float deltaTime)
    {
        for (int index = activeTasks.Count - 1; index >= 0; index--)
        {
            activeTasks[index].Tick(deltaTime);
        }
    }
    
    public void AddTask(TimedTask task)
    {
        activeTasks.Add(task);
    }

    public void RemoveTask(TimedTask task)
    {
        activeTasks.Remove(task);
    }
}
