using System;
using UnityEngine;

[Serializable]
public struct DishTimePair
{
    [SerializeField] internal ItemType dishToCook;
    [SerializeField] internal int timeInSeconds;
    
}
