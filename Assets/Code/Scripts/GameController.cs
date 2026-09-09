using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]

public class GameController : MonoBehaviour
{
    
    [SerializeField] private List<Level> gameLevels;
    private List<FoodType> _foodBacklog = new();
    
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
        foreach (SubSection levelSection in levelToPlay.dishesToBeCooked)
        {
            _foodBacklog.Add(levelSection.value);
            yield return new WaitForSeconds(levelSection.timeInSeconds);
        }
        
        
        
    }


}
