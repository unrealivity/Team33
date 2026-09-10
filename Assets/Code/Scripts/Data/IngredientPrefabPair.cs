using System;
using UnityEngine;

[Serializable]
public struct IngredientPrefabPair
{
    [SerializeField] internal IngredientType ingredient;
    [SerializeField] internal GameObject prefab;
}
