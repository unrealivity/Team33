using System;
using System.Collections.Generic;
using UnityEngine;

// TODO LÖSCHEN
//using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Cutting : Station {

    public static event Action CuttingEvent;
    
    private Ingredient _ingredientOnBoard;
    private int _clicks = 0;
    
    [SerializeField] private float foodSpawnForce = 4f;

    [SerializeField] private Recipes recipeCollection;
    [SerializeField] private List<GameObject> foodPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject cutIndicator;
    [SerializeField] private ObjectDetector objectDetector;
    
    [SerializeField] private GameObject foodContainerParent;           // GameObject that parents instantiated foods

    private void Awake()
    {
        objectDetector.OnIngredientEnter += HandleObjectEnter;
        objectDetector.OnIngredientExit += HandleObjectExit;
        
        cutIndicator.SetActive(false);
    }

    private void OnDestroy()
    {
        objectDetector.OnIngredientEnter -= HandleObjectEnter;
        objectDetector.OnIngredientExit -= HandleObjectExit;
    }

    private void HandleObjectEnter(Ingredient ingredient)
    {
        foreach (Recipes.Recipe recipe in recipeCollection.recipeList)
        {
            if (recipe.ingredients.Count == 1 && recipe.ingredients[0] == ingredient.ingredient)
            {
                _ingredientOnBoard = ingredient;
                cutIndicator.SetActive(true);
                Debug.Log("ON BOARD " + ingredient.ingredient);
                return;
            }
        }
    }
    
    private void HandleObjectExit(Ingredient ingredient)
    {
        if(ingredient == _ingredientOnBoard) 
        {
            _ingredientOnBoard = null;
            _clicks = 0;
            cutIndicator.SetActive(false);
            Debug.Log("OFF BOARD");
        }
    }
    
    public override void Interact() {
        Cut();
    }

    private void Cut() {
        if (_ingredientOnBoard == null) {
            return;
        }

        foreach (Recipes.Recipe recipe in recipeCollection.recipeList) {
            if (recipe.ingredients.Count == 1 && recipe.ingredients[0] == _ingredientOnBoard.ingredient) { 
                _clicks++;
                Debug.Log("CUTTING " + _clicks + " / " + recipe.prepareValue);

                CuttingEvent?.Invoke();
                
                if (_clicks >= recipe.prepareValue) {
                    CuttingDone(recipe);
                }

                return;
            }
        }
    }

    private void CuttingDone(Recipes.Recipe recipe) {
        Ingredient oldIngredient = _ingredientOnBoard;

        _ingredientOnBoard = null;
        _clicks = 0;
        cutIndicator.SetActive(false);
        
        Destroy(oldIngredient.gameObject);
        foreach (GameObject prefab in foodPrefab) {
            Food food = prefab.GetComponent<Food>();
            if (food != null && food.foodType == recipe.result) {
                GameObject newFood = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation,foodContainerParent.transform);
                Rigidbody rigid = newFood.GetComponent<Rigidbody>();

                if (rigid != null) {
                    Vector3 direction =new Vector3( Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
                    rigid.AddForce(direction.normalized * foodSpawnForce, ForceMode.Impulse); 
                }

                Debug.Log("CUT DONE " + recipe.result);
                return;
            }
        }
    }
}