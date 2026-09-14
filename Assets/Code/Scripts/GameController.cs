using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    [SerializeField] private List<Level> gameLevels;
    private List<FoodType> _foodBacklog = new();
    public static event Action<FoodType> FoodAddedToBacklog;
    public static event Action<FoodType> FindCookedFood; 
    public static event Action<int> DishTimerInSeconds;
    
    
    private Level _currentLevel;
    private bool _currentLevelIsCleared;



    private void Start()
    {
        StartCoroutine(PlayGame());
    }

    private IEnumerator PlayGame()
    {
        foreach (var level in gameLevels)
        {
            if (level.isTimed)
            {
                StartCoroutine(PlayLevel(level));
                yield return new WaitForSeconds(level.timeForLevel);
            }
            else
            {
                StartCoroutine(PlayLevel(level));
                yield return new WaitUntil(() => _currentLevelIsCleared);
            }
        }
    }
    
    
    private IEnumerator PlayLevel(Level levelToPlay)
    {
        foreach (DishTimePair levelSection in levelToPlay.dishesToBeCooked)
        {
            _foodBacklog.Add(levelSection.dishToCook);
            FoodAddedToBacklog?.Invoke(levelSection.dishToCook);
            DishTimerInSeconds?.Invoke(levelSection.timeInSeconds);
            yield return new WaitForSeconds(levelSection.timeInSeconds);
            FindCookedFood?.Invoke(levelSection.dishToCook);
            
        }
        
        
        
    }


}
