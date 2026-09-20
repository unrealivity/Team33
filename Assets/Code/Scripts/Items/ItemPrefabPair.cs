using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ItemPrefabPair
{
    [SerializeField] internal ItemType item;
    [SerializeField] internal GameObject prefab;
    public ItemPrefabPair(GameObject prefab, ItemType itemType)
    {
        item = itemType;
        this.prefab = prefab;
    }
}
