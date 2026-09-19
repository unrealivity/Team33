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

    
    
    private List<Food> _foodOnCounter = new();
    [SerializeField] private ObjectDetector objectDetector;
    public static event Action<ItemType> FoodNotFound;

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
        if (oldestMatch != null)
            Collect(oldestMatch);
        else
            FoodNotFound?.Invoke(completedDish);
    }

    private void TryCollect()
    {
        if (_foodOnCounter.Count == 0) return;
        Collect(_foodOnCounter[0]);
    }

    private void Collect(Food food)
    {
        if (!_foodOnCounter.Remove(food)) return; // guards double-collect if AutoCollect and Interact race
        food.CollectFood(collectForce,forceDamper,maxCollectForce,targetLocation.position,bufferDistance);
    }
}
