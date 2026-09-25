using System.Collections.Generic;

public class TaskManager
{   
    public static TaskManager Instance { get; } = new();

    private readonly List<TimedTask> activeTasks = new();
    private readonly List<TimedTask> pendingRemovals = new();
    private bool _isTicking;

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
