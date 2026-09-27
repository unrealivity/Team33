using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class OrderBacklog : MonoBehaviour
{ 
    public static OrderBacklog Instance { get; private set; }

    public event Action<ItemType, TimedTask> OrderAdded;
    public event Action<ItemType> OrderFulfilled;
    public event Action<ItemType> OrderFailed;

    private readonly Dictionary<ItemType, Queue<TimedTask>> _pendingOrders = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameController.FoodAddedToBacklog += HandleOrderAdded;
        ServingCounter.FoodCollected += HandleOrderFulfilled;
        ServingCounter.FoodFailedToCollect += HandleOrderFailed;
    }

    private void OnDestroy()
    {
        if (Instance != this) return; // don't unsubscribe if this was a duplicate that got destroyed

        GameController.FoodAddedToBacklog -= HandleOrderAdded;
        ServingCounter.FoodCollected -= HandleOrderFulfilled;
        ServingCounter.FoodFailedToCollect -= HandleOrderFailed;
        Instance = null;
    }
    
    public bool IsOrdered(ItemType foodType)
    {
        return _pendingOrders.TryGetValue(foodType, out var queue) && queue.Count > 0;
    }

    public bool HasActiveOrders 
    {
        get
        {
            foreach (var queue in _pendingOrders.Values)
                if (queue.Count > 0) return true;
            return false;
        }
    }
    

    private void HandleOrderAdded(ItemType foodType, TimedTask dishTask)
    {
        if (!_pendingOrders.TryGetValue(foodType, out var queue))
        {
            queue = new Queue<TimedTask>();
            _pendingOrders[foodType] = queue;
        }
        queue.Enqueue(dishTask);
        OrderAdded?.Invoke(foodType, dishTask);
    }
    
    private void HandleOrderFulfilled(ItemType foodType)
    {
        ResolveOldest(foodType);
        OrderFulfilled?.Invoke(foodType);
    }
    
    private void HandleOrderFailed(ItemType foodType)
    {
        ResolveOldest(foodType);
        OrderFailed?.Invoke(foodType);
    }
    
    
    private void ResolveOldest(ItemType foodType)
    {
        if (!_pendingOrders.TryGetValue(foodType, out var queue) || queue.Count == 0) return;

        TimedTask resolvedTask = queue.Dequeue();
        if (resolvedTask != null) // nothing to cancel for an untimed dish
            TaskManager.Instance.RemoveTask(resolvedTask);
    }
}
