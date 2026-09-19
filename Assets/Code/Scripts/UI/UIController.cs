using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]

public class UIController : MonoBehaviour
{

    [SerializeField] private List<ItemPrefabPair> foodPrefabPairWithTimer;
    [SerializeField] private GameObject foodIconContainer;
    private List<ItemPrefabPair> _activeRecipes = new();
    private void Awake()
    {
        GameController.FoodAddedToBacklog += AddToToDoList;
        Food.FoodCollected += SuccessRemoveFromToDoList;
        FoodCollector.FoodNotFound += FailedRemoveFromToDoList;
    }

    private void AddToToDoList(ItemType foodType,TimedTask dishTask)
    {
        foreach (ItemPrefabPair foodPrefabPair in foodPrefabPairWithTimer)
        {
            if(foodPrefabPair.item != foodType) continue;
            GameObject foodRequestGameObject = Instantiate(foodPrefabPair.gameObject, foodIconContainer.transform);
            if (foodRequestGameObject.TryGetComponent(out TimerVisual timerVisual))
            {
                timerVisual.Bind(dishTask);
            }
            else
            {
                Debug.LogError(foodRequestGameObject.gameObject+ "prefab is missing a TimerVisual component");
            }
            _activeRecipes.Add(new ItemPrefabPair(foodRequestGameObject,foodType));
        }
    }

    private void SuccessRemoveFromToDoList(ItemType foodType)
    {
        Debug.Log( "Correct Food was delivered" );
        foreach (var foodPrefabPair in _activeRecipes)
        {
            if (foodPrefabPair.item == foodType)
            {
                Destroy(foodPrefabPair.gameObject);
                _activeRecipes.Remove(foodPrefabPair);
                return;
            }
        }
    }

    private void FailedRemoveFromToDoList(ItemType foodType)
    {
        Debug.Log( "Failed to deliver:" + foodType);
        foreach (var foodPrefabPair in _activeRecipes)
        {
            if (foodPrefabPair.item == foodType)
            {
                Destroy(foodPrefabPair.gameObject);
                _activeRecipes.Remove(foodPrefabPair);
                return;
            }
        }
    }
    
}
