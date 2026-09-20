using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class UIController : MonoBehaviour
{

    [SerializeField] private List<ItemPrefabPair> foodTypeIconPrefabPairs;
    [SerializeField] private GameObject foodIconContainer;
    private List<ItemPrefabPair> _activeRecipes = new();
    private void Awake()
    {
        OrderBacklog.Instance.OrderAdded += AddToToDoList;
        OrderBacklog.Instance.OrderFulfilled += SuccessRemoveFromToDoList;
        OrderBacklog.Instance.OrderFailed += FailedRemoveFromToDoList;
    }
    private void OnDestroy()
    {
        OrderBacklog.Instance.OrderAdded -= AddToToDoList;
        OrderBacklog.Instance.OrderFulfilled -= SuccessRemoveFromToDoList;
        OrderBacklog.Instance.OrderFailed -= FailedRemoveFromToDoList;
    }

    private void AddToToDoList(ItemType foodType,TimedTask dishTask)
    {
        foreach (ItemPrefabPair foodIconPair in foodTypeIconPrefabPairs)
        {
            if(foodIconPair.item != foodType) continue;
            GameObject foodRequestGameObject = Instantiate(foodIconPair.prefab, foodIconContainer.transform);
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

            GameObject icon = _activeRecipes[i].prefab;
            _activeRecipes.RemoveAt(i);

            if (icon.TryGetComponent(out TimerVisual timerVisual))
            {   
                timerVisual.OnFeedbackComplete += () => Destroy(icon);
                timerVisual.PlayFeedback(success);
            }

            return;
        }
    }
    
}
