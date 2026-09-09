using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class SerializableTimedPair<T1, T2>
{
    public T1 value;
    public T2 timeInSeconds;
}
[Serializable]
public class SubSection : SerializableTimedPair<FoodType,int>{}



[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    
    public List<SubSection> dishesToBeCooked;
    [Space]
    
    
    public bool isTimed = true;
    [Range(12,200)]
    public int timeForLevel;

    [Space]
    [Range(1, 10)]
    public int difficulty;
}
