using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class UIController : MonoBehaviour
{

    [SerializeField] private List<ItemPrefabPair> foodTypeIconPrefabPairs;
    [SerializeField] private GameObject foodIconContainer;
    private List<ItemPrefabPair> _activeRecipes = new();
    private OrderBacklog _backlog;
    
    private void Awake()
    {
        _backlog = OrderBacklog.Instance;
        _backlog.OrderAdded += AddToToDoList;
        _backlog.OrderFulfilled += SuccessRemoveFromToDoList;
        _backlog.OrderFailed += FailedRemoveFromToDoList;
    }
    private void OnDestroy()
    {
        if (_backlog == null) return;
        _backlog.OrderAdded -= AddToToDoList;
        _backlog.OrderFulfilled -= SuccessRemoveFromToDoList;
        _backlog.OrderFailed -= FailedRemoveFromToDoList;
    }

    private void AddToToDoList(ItemType foodType,TimedTask dishTask)
    {
        foreach (ItemPrefabPair foodIconPair in foodTypeIconPrefabPairs)
        {
            if(foodIconPair.item != foodType) continue;
            var foodRequestGameObject = PoolManager.Instance.Get(foodIconPair.prefab, foodIconContainer.transform, transform.position, transform.rotation );
            if (foodRequestGameObject.TryGetComponent(out TimerVisual timerVisual))
            {
                if (dishTask != null)
                {
                    timerVisual.Bind(dishTask);
                }
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
                timerVisual.OnFeedbackComplete += () =>
                {
                    if (PoolManager.Instance != null) PoolManager.Instance.Release(icon);
                };
                timerVisual.PlayFeedback(success);
            }

            return;
        }
    }
    
}
