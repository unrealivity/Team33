using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private List<Level> gameLevels;
    public static event Action<ItemType,TimedTask> FoodAddedToBacklog;
    public static event Action<ItemType> FindCookedFood;
    
    private Level _currentLevel;
    private bool _currentLevelIsCleared;



    private void Start()
    {
        StartCoroutine(PlayGame());
    }

    private void Update()
    {
        TaskManager.Instance.Tick(Time.deltaTime);
    }

    private IEnumerator PlayGame()
    {
        foreach (Level level in gameLevels)
        {   
            PlayLevel(level);
            
            if (level.isTimed)
            {
                yield return WaitForTimeOrClear(level.timeUntilNextLevelAppears);
            }
            else
            {
                yield return new WaitUntil(() => !OrderBacklog.Instance.HasActiveOrders);
            }
        }
        
    }
    
    private IEnumerator WaitForTimeOrClear(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration && OrderBacklog.Instance.HasActiveOrders)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    
    private void PlayLevel(Level levelToPlay)
    {
        foreach (DishTimePair levelSection in levelToPlay.dishesToBeCooked)
        {
            TimedTask dishTask = null;
            
            if (levelSection.isTimed)
            {
                dishTask = new TimedTask(levelSection.timeInSeconds);
                dishTask.OnComplete += () =>
                {
                    FindCookedFood?.Invoke(levelSection.dishToCook);
                    TaskManager.Instance.RemoveTask(dishTask);
                };
                TaskManager.Instance.AddTask(dishTask);
            }
            
            FoodAddedToBacklog?.Invoke(levelSection.dishToCook, dishTask);
        }
        
    }
    
}
