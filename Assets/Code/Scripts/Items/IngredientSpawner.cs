using System.Collections.Generic;
using UnityEngine;



public class IngredientSpawner : MonoBehaviour
{   
    
    [SerializeField] private List<Recipes> recipeListsPerStation ;
    [SerializeField] private List<ItemPrefabPair> ingredientPrefabPairs;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform parentContainer;
    private void Awake()
    {
        GameController.FoodAddedToBacklog += SpawnIngredientsForRecipe;
    }


    private void SpawnIngredientsForRecipe(ItemType foodToSpawnIngredientsFor,TimedTask _task)
    {
        if(!TryToFindRecipeFor(foodToSpawnIngredientsFor))
        {
            Debug.LogError("Food you are trying to spawn(" + foodToSpawnIngredientsFor + ") has no recipe");
            return;
        }
        
        
        foreach (Recipes stationRecipes in recipeListsPerStation)
        {
            foreach (Recipes.Recipe stationSpecificRecipe in stationRecipes.recipeList)
            {
                
                if (stationSpecificRecipe.result != foodToSpawnIngredientsFor) continue;
                
                foreach (ItemType ingredientToSpawn in stationSpecificRecipe.ingredients)
                {
                    if (TryToFindRecipeFor(ingredientToSpawn))
                    {
                        SpawnIngredientsForRecipe(ingredientToSpawn,_task);
                        continue;
                    }
                    SpawnIngredientFromType(ingredientToSpawn);
                }
            }
        }
    }

    private void SpawnIngredientFromType(ItemType ingredientTypeToSpawn)
    {
        foreach (ItemPrefabPair ingredientPrefabPair in ingredientPrefabPairs)
        {
            if (ingredientPrefabPair.item == ingredientTypeToSpawn)
            {
                GameObject spawnedObject = PoolManager.Instance.Get(ingredientPrefabPair.prefab, parentContainer, spawnPoint.position, spawnPoint.rotation);
                return;
            }
        }
    }

    private bool TryToFindRecipeFor(ItemType itemToCheck)
    {
        bool hasRecipe = false;

        foreach (Recipes stationRecipes in recipeListsPerStation)
        {
            foreach (Recipes.Recipe stationSpecificRecipe in stationRecipes.recipeList)
            {
                if (stationSpecificRecipe.result != itemToCheck) continue;
                hasRecipe = true;
            }
        }
        return hasRecipe;
    }
    
}
