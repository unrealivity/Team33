using System;
using System.Collections.Generic;
using UnityEngine;

public class ServingCounter : Station
{
    [SerializeField] private Transform targetLocation;
    [SerializeField] private float maxCollectForce;
    [SerializeField] private float collectForce;
    [SerializeField] private float forceDamper;
    [SerializeField] private float bufferDistance;

    
    
    private readonly List<Food> _foodOnCounter = new();
    [SerializeField] private ObjectDetector objectDetector;
    public static event Action<ItemType> FoodFailedToCollect;
    public static event Action<ItemType> FoodCollected;

    private void Awake()
    {
        objectDetector.OnFoodEnter += AddFoodToCounter;
        objectDetector.OnFoodExit += RemoveFoodFromCounter;
        GameController.FindCookedFood += AutoCollect;
    }
    


    public override void Interact()
    {
        TryCollect();
    }

    private void AddFoodToCounter(Food foodToAdd)
    {
        _foodOnCounter.Add(foodToAdd);
    }
    private void RemoveFoodFromCounter(Food foodToRemove)
    {
        _foodOnCounter.Remove(foodToRemove);
    }

    private void AutoCollect(ItemType completedDish)
    {
        Food oldestMatch = _foodOnCounter.Find(f => f.foodType == completedDish);
        if (oldestMatch is not null)
        {
            Collect(oldestMatch);
        }
        else
        {
            FoodFailedToCollect?.Invoke(completedDish);
        }
    }

    private void TryCollect()
    {
        Food oldestOrderedMatch = _foodOnCounter.Find(f => OrderBacklog.Instance.IsOrdered(f.foodType));
        if (oldestOrderedMatch != null)
            Collect(oldestOrderedMatch);
    }

    private void Collect(Food food)
    {
        if (!_foodOnCounter.Remove(food)) return; // guards double-collect if AutoCollect and Interact race
        FoodCollected?.Invoke(food.foodType);
        food.CollectFood(collectForce,forceDamper,maxCollectForce,targetLocation.position,bufferDistance);
    }
}
