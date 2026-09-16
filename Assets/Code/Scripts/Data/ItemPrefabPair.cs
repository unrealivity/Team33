using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ItemPrefabPair
{
    [SerializeField] internal ItemType item;
    [SerializeField] internal GameObject gameObject;
    public ItemPrefabPair(GameObject gameObject, ItemType itemType)
    {
        item = itemType;
        this.gameObject = gameObject;
    }
}
