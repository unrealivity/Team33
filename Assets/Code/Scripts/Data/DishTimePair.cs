using System;
using UnityEngine;

[Serializable]
public struct DishTimePair
{
    [SerializeField] internal FoodType dishToCook;
    [SerializeField] internal int timeInSeconds;
    
}
