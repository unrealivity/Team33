using System;
using UnityEngine;

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
