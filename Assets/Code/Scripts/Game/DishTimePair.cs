using System;
using UnityEngine;

[Serializable]
public class DishTimePair
{
    public ItemType dishToCook;
    public int timeInSeconds;
    public bool isTimed = true;
}
