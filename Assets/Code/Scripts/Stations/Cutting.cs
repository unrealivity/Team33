using System;
using System.Collections.Generic;
using UnityEngine;

// TODO LÖSCHEN
//using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Cutting : Station {
    
    private Ingredient _ingredientOnBoard;
    private int _clicks = 0;
    
    [SerializeField] private float foodSpawnForce = 4f;

    [SerializeField] private Recipes recipeCollection;
    [SerializeField] private List<GameObject> foodPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject cutIndicator;
    [SerializeField] private IngredientDetector ingredientDetector;
    
//TODO Löschen?!     
// [SerializeField] private Camera playerCamera;    
// [SerializeField] private float interactionRange = 3f;
    [SerializeField] private GameObject foodContainerParent;           // GameObject that parents instantiated foods

    private void Awake()
    {
        ingredientDetector.OnIngredientEnter += HandleIngredientEnter;
        ingredientDetector.OnIngredientExit += HandleIngredientExit;
        
        cutIndicator.SetActive(false);
    }

    private void OnDestroy()
    {
        ingredientDetector.OnIngredientEnter -= HandleIngredientEnter;
        ingredientDetector.OnIngredientExit -= HandleIngredientExit;
    }

    private void HandleIngredientEnter(Ingredient ingredient)
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
    
    private void HandleIngredientExit(Ingredient ingredient)
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
//TODO löschen!?!    
/*
    private void OnMouseDown() {
        if (_ingredientOnBoard == null) {
            return;
        }
        foreach (CuttingRecipe recipe in Recipe) {
            if (recipe.ingredient == _ingredientOnBoard.ingredient) {
                _clicks++;
                Debug.Log("CUTTING" + _clicks + " / " + recipe.neededClicks);
                return;
            }
        }
    }
*/
    private void Cut() {
        if (_ingredientOnBoard == null) {
            return;
        }

        foreach (Recipes.Recipe recipe in recipeCollection.recipeList) {
            if (recipe.ingredients.Count == 1 && recipe.ingredients[0] == _ingredientOnBoard.ingredient) { 
                _clicks++;
                Debug.Log("CUTTING " + _clicks + " / " + recipe.prepareValue);

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
    

    // Update is called once per frame
    /*
     void Update()
       {
                // TODO LÖSCHEN?!
          if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
               RaycastHit hit;
               if (Physics.Raycast(playerCamera.transform.position,
                       playerCamera.transform.forward,
                       out hit, interactionRange)) {
                   if (hit.collider.gameObject == cutIndicator) {
                       Cut();
                   }
               }
           }
      }*/ 
}

//TODO Soundeffekte für Kochen(brutzeln oder blubbern) / Kochenfertig(Eieruhr Ping)
//TODO Item States beim Schneiden / Schneid UI(Cut Icon) / Koch UI(Timer)
//TODO Debug löschen
//TODO Komentare aktuallisieren oder löschen