using System;
using UnityEngine;

[Serializable]
public struct FoodPrefabPair : IEquatable<FoodPrefabPair>
{
    [SerializeField] internal FoodType foodType;
    [SerializeField] internal GameObject gameObject;
    public FoodPrefabPair(GameObject gameObject, FoodType foodType)
    {
        this.foodType = foodType;
        this.gameObject = gameObject;
    }
    public bool Equals(FoodPrefabPair other)
    {
        return foodType == other.foodType && Equals(gameObject, other.gameObject);
    }
    public override bool Equals(object obj)
    {
        return obj is FoodPrefabPair other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine((int)foodType, gameObject);
    }
}
