using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    
    public List<DishTimePair> dishesToBeCooked;
    [Space]
    
    
    public bool isTimed = true;
    [Range(0,200)]
    public int timeUntilNextLevelAppears;

    [Space]
    [Range(1, 10)]
    public int difficulty;
}
