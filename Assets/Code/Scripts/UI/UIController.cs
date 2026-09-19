using System;
using System.Collections.Generic;
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
        ServingCounter.FoodNotFound += FailedRemoveFromToDoList;
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

    private void SuccessRemoveFromToDoList(ItemType foodType) => ResolveOrder(foodType, success: true);
    private void FailedRemoveFromToDoList(ItemType foodType) => ResolveOrder(foodType, success: false);

    private void ResolveOrder(ItemType foodType, bool success)
    {
        for (int i = 0; i < _activeRecipes.Count; i++)
        {
            if (_activeRecipes[i].item != foodType) continue;

            GameObject icon = _activeRecipes[i].gameObject;
            _activeRecipes.RemoveAt(i);

            if (icon.TryGetComponent(out TimerVisual timerVisual))
            {
                Destroy(timerVisual.gameObject);
            }

            return;
        }
    }
    
}
