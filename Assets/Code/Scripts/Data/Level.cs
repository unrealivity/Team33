using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    
    public List<DishTimePair> dishesToBeCooked;
    [Space]
    
    
    public bool isTimed = true;
    [Range(12,200)]
    public int timeForLevel;

    [Space]
    [Range(1, 10)]
    public int difficulty;
}
