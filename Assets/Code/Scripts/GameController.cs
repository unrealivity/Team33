using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private List<Level> gameLevels;
    public static event Action<FoodType,int> FoodAddedToBacklog;
    public static event Action<FoodType> FindCookedFood; 

    
    
    private Level _currentLevel;
    private bool _currentLevelIsCleared;



    private void Start()
    {
        StartCoroutine(PlayGame());
    }

    private IEnumerator PlayGame()
    {
        foreach (Level level in gameLevels)
        {
            if (level.isTimed)
            {
                PlayLevel(level);
                yield return new WaitForSeconds(level.timeForLevel);
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
            FoodAddedToBacklog?.Invoke(levelSection.dishToCook,levelSection.timeInSeconds);
            StartCoroutine(TimeDish(levelSection.dishToCook,levelSection.timeInSeconds));
        }
        
    }
    private IEnumerator TimeDish(FoodType foodToCook, int timeToCook)
    {
        yield return new WaitForSeconds(timeToCook);
        FindCookedFood?.Invoke(foodToCook);
    }

}
