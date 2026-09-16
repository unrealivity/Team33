using System.Collections.Generic;
using UnityEngine;

// TODO LÖSCHEN
//using UnityEngine.InputSystem;

public class Cutting : Station {
    
    private Ingredient _ingredientOnBoard;
    private int _clicks = 0;
    
    [System.Serializable]
    public class CuttingRecipe {
        public IngredientType ingredient;
        public FoodType result;
        public int neededClicks;
    }
    
    [SerializeField] private float foodSpawnForce = 4f;
    
    [SerializeField] private List<CuttingRecipe> Recipe;
    [SerializeField] private List<GameObject> FoodPrefab;
    [SerializeField] private Transform SpawnPoint;
    [SerializeField] private GameObject CutIndicator;
    
    
//TODO Löschen?!     
// [SerializeField] private Camera PlayerCamera;    
// [SerializeField] private float InteractionRange = 3f;    

    private void OnTriggerEnter(Collider other) {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null) {
            foreach (CuttingRecipe recipe in Recipe) {
                if (recipe.ingredient == ingredient.ingredient) {
                    _ingredientOnBoard = ingredient;
                    CutIndicator.SetActive(true);
                    Debug.Log("ON BOARD " + ingredient.ingredient);
                    return;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        Ingredient ingredient = other.GetComponent<Ingredient>();
        if (ingredient != null && ingredient == _ingredientOnBoard) {
            _ingredientOnBoard = null;
            _clicks = 0;
            CutIndicator.SetActive(false);
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

        foreach (CuttingRecipe recipe in Recipe) {
            if (recipe.ingredient == _ingredientOnBoard.ingredient) {
                _clicks++;
                Debug.Log("CUTTING" + _clicks + " / " + recipe.neededClicks);

                if (_clicks >= recipe.neededClicks) {
                    CuttingDone(recipe);
                }

                return;
            }
        }
    }

    private void CuttingDone(CuttingRecipe recipe) {
        Ingredient oldIngredient = _ingredientOnBoard;

        _ingredientOnBoard = null;
        _clicks = 0;
        CutIndicator.SetActive(false);
        
        Destroy(oldIngredient.gameObject);
        foreach (GameObject prefab in FoodPrefab) {
            Food food = prefab.GetComponent<Food>();
            if (food != null && food.food == recipe.result) {
                GameObject newFood = Instantiate(prefab, SpawnPoint.position, SpawnPoint.rotation);
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CutIndicator.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    { 
// TODO LÖSCHEN?!        
/*        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) {
            RaycastHit hit;
            if (Physics.Raycast(PlayerCamera.transform.position, 
                    PlayerCamera.transform.forward, 
                    out hit, InteractionRange)) {
                if (hit.collider.gameObject == CutIndicator) {
                    Cut();
                }
            }
        }
*/    }
}
//TODO Soundeffekte für Kochen(brutzeln oder blubbern) / Kochenfertig(Eieruhr Ping)
//TODO Item States beim Schneiden / Schneid UI(Cut Icon) / Koch UI(Timer)
//TODO Debug löschen
//TODO Komentare aktuallisieren oder löschen