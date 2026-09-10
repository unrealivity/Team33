using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class IngredientSpawner : MonoBehaviour
{   
    
    [SerializeField] private List<Recipes> recipeListsPerStation ;
    [SerializeField] private List<IngredientPrefabPair> ingredientPrefabPairs;
    [SerializeField] private Transform spawnPoint;
    private void Awake()
    {
        GameController.requestIngredients += SpawnIngredientsForRecipe;
    }

    private void SpawnIngredientsForRecipe(FoodType foodToSpawnIngredientsFor)
    {
        
        foreach (Recipes stationRecipes in recipeListsPerStation)
        {
            foreach (var foodResult in stationRecipes.recipe)
            {
                
                if (foodResult.result != foodToSpawnIngredientsFor) continue;
                
                foreach (IngredientType ingredientToSpawn in foodResult.ingredient)
                {
                    foreach (IngredientPrefabPair ingredientPrefabPair in ingredientPrefabPairs)
                    {
                        if (ingredientPrefabPair.ingredient == ingredientToSpawn)
                        {
                            Instantiate(ingredientPrefabPair.prefab,spawnPoint.position,spawnPoint.rotation);
                        }
                    }
                }
            }
        }
        
        
    }
}
