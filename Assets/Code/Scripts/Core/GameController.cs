using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private List<Level> gameLevels;
    public static event Action<ItemType,TimedTask> FoodAddedToBacklog;
    public static event Action<ItemType> FindCookedFood;
    private readonly TaskManager taskManager = new();
    
    
    private Level _currentLevel;
    private bool _currentLevelIsCleared;



    private void Start()
    {
        StartCoroutine(PlayGame());
    }

    private void Update()
    {
        taskManager.Tick(Time.deltaTime);
    }

    private IEnumerator PlayGame()
    {
        foreach (Level level in gameLevels)
        {
            if (level.isTimed)
            {
                PlayLevel(level);
                yield return new WaitForSeconds(level.timeUntilNextLevelAppears);
            }
            else
            {
                PlayLevel(level);
                yield return new WaitUntil(() => _currentLevelIsCleared);
            }
        }
    }
    
    
    private void PlayLevel(Level levelToPlay)
    {
        foreach (DishTimePair levelSection in levelToPlay.dishesToBeCooked)
        {

            var dishTask = new TimedTask(levelSection.timeInSeconds);
            dishTask.OnComplete += () =>
            {
                FindCookedFood?.Invoke(levelSection.dishToCook);
                taskManager.RemoveTask(dishTask);
            };
            taskManager.AddTask(dishTask);
            
            FoodAddedToBacklog?.Invoke(levelSection.dishToCook, dishTask);
        }
        
    }
    
}
