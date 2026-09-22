using System.Collections.Generic;
using UnityEngine;

public class ObjectDispenser : Station
{
    
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxExistingSpawns;
    [SerializeField] private Transform parentContainer;
    
    private readonly List<GameObject> _dispensedIngredients = new();
    
    public override void Interact()
    {
        TryDispenseItem();
    }

    private void TryDispenseItem()
    {
        if (CheckActiveSpawnCount() < maxExistingSpawns)
        {
            SpawnItem();
        }
        else
        {
            Debug.Log("SpawnCap for dispenser already full");
        }
    }
    
    private void SpawnItem()
    {
        GameObject spawnedObject = PoolManager.Instance.Get(objectToSpawn, parentContainer, spawnPoint.position, spawnPoint.rotation);
        _dispensedIngredients.Add(spawnedObject);
    }

    private int CheckActiveSpawnCount()
    {
        _dispensedIngredients.RemoveAll(obj => obj == null || !obj.activeInHierarchy);
        return _dispensedIngredients.Count;
    }
}
