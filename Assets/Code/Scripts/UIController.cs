using System;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{

    [SerializeField] private List<FoodPrefabPair> foodPrefabPairWithTimer;
    [SerializeField] private GameObject foodIconContainer;
    private List<FoodPrefabPair> _activeRecipes = new();
    private void Awake()
    {
        GameController.FoodAddedToBacklog += AddToToDoList;
        Food.FoodCollected += SuccessRemoveFromToDoList;
        FoodCollector.FoodNotFound += FailedRemoveFromToDoList;
    }

    private void AddToToDoList(FoodType foodType,int timeForDish)
    {
        foreach (FoodPrefabPair foodPrefabPair in foodPrefabPairWithTimer)
        {
            if(foodPrefabPair.foodType != foodType) continue;
            GameObject foodRequestGameObject = new GameObject();
            foodRequestGameObject = Instantiate(foodPrefabPair.gameObject, foodIconContainer.transform);
            _activeRecipes.Add(new FoodPrefabPair(foodRequestGameObject,foodType));
        }
    }

    private void SuccessRemoveFromToDoList(FoodType foodType)
    {
        Debug.Log( "Correct Food was delivered" );
        foreach (var foodPrefabPair in _activeRecipes)
        {
            if (foodPrefabPair.foodType == foodType)
            {
                Destroy(foodPrefabPair.gameObject);
                _activeRecipes.Remove(foodPrefabPair);
                return;
            }
        }
    }

    private void FailedRemoveFromToDoList(FoodType foodType)
    {
        Debug.Log( "Failed to deliver:" + foodType);
        foreach (var foodPrefabPair in _activeRecipes)
        {
            if (foodPrefabPair.foodType == foodType)
            {
                Destroy(foodPrefabPair.gameObject);
                _activeRecipes.Remove(foodPrefabPair);
                return;
            }
        }
    }
    
}
