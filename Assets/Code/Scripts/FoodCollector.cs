using Mono.Cecil;
using System;
using UnityEngine;

public class FoodCollector : MonoBehaviour
{
    [SerializeField] private GameObject foodSection;
    [SerializeField] private float maxCollectForce;
    [SerializeField] private float collectForce;
    [SerializeField] private float forceDamper;
    private Food _currentFood;
    private Rigidbody _currentFoodRigidbody;
    private bool _collecting;
    [SerializeField] private float bufferDistance;
    public static event Action<ItemType> FoodNotFound;



    private void Awake()
    {
        GameController.FindCookedFood += TryToFindFood;
    }

    private void TryToFindFood(ItemType foodTypeToCollect)
    {
        foreach (var food in foodSection.GetComponentsInChildren<Food>())
        {
            if (food.foodType != foodTypeToCollect) continue;
            if (food.isClaimed) continue;

            food.isClaimed = true;
            food.CollectFood(collectForce,forceDamper,maxCollectForce,transform.position,bufferDistance);
            return;
        }
        FoodNotFound?.Invoke(foodTypeToCollect);
    }


    
}
