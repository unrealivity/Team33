using System.Collections.Generic;
using UnityEngine;

public class Cooking : MonoBehaviour {
    [SerializeField] private Recipes recipeCollection;                 // Platz für Recipe script
    [SerializeField] private Transform spawnPoint;                     // Spawn für fertiges Food
    [SerializeField] private List<GameObject> foodPrefab;              // Prefabs von Spawnbarem
    
    private List<Ingredient> _ingredientsInPot = new List<Ingredient>();          // Liste Objekte im Topf
    private List<Ingredient> _ingredientsInUse = new List<Ingredient>();          // Liste Verwendung für Cooking
    private Recipes.Recipe _activRecipe;                                          // Für Cooking aktives Recipe
    private float _timer;                                                         // Timer für Abgleich beim Cooking
    private void OnTriggerEnter(Collider other) {                                 // Wenn etwas rein fällt
        Debug.Log("TRIGGER FOUND : " + other.gameObject.name);         
        Ingredient ingredient = other.GetComponent<Ingredient>();                 // Hohl das Zutaten Script vom Objekt
        Debug.Log("INGREDIENT FOUND : " + (ingredient != null));            
        
        if (ingredient != null && !_ingredientsInPot.Contains(ingredient)) {   // wenns ne Ingredient ist und nicht im Topf...
            _ingredientsInPot.Add(ingredient);                                    // ... packs in den Topf ...
            
            CheckRecipe();                                                        // ... prüfe das Recipe
            Debug.Log("SHITS THROW'N IN "+ ingredient);                             
        }
    }
    private void OnTriggerExit(Collider other) {                    // Wenn etwas raus fällt
        Ingredient ingredient = other.GetComponent<Ingredient>();   // Hohl info aus Ingredient
        
        if (ingredient != null) {                                // Wenn es eine Ingredient ist...
            _ingredientsInPot.Remove(ingredient);                   // ... entferne sie aus dem Topf ...
            _activRecipe = null;                                    // ... entferne aktives Recipe ...
            _timer = 0;                                             // ... setze den Timer auf null
            
            CheckRecipe();
            Debug.Log("SHITS THROW'N OUT "+ ingredient.ingredient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_activRecipe != null) {                       // Wenn Recipe aktiv ist ...
            _timer += Time.deltaTime;                     // ... erhöhe den Timer ...
            if (_timer >= _activRecipe.cookingTime) {     // ... wenn Timer fertig ...
                CookingDone();                            // ... führe CookingDone aus
            }
        }
    }

    private void CheckRecipe() {                                       
        _activRecipe = null;                                          
        _timer = 0;                                                     
        _ingredientsInUse.Clear();                                     

        foreach (Recipes.Recipe recipe in recipeCollection.recipe) {                    // für jedes Recipe in Sammlung
            List<Ingredient> freeIngredient = new List<Ingredient>(_ingredientsInPot);  // kopiere ingredient im Topf in freie Zutaten
            List<Ingredient> found = new List<Ingredient>();                            // Speichere die Zutaten die zum Recipe passen
            
            bool match = true;                                                          // Bestätige das Recipe und Zutaten passen

            foreach (IngredientType used in recipe.ingredient) {                                 // Für jede benötigte Ingredient 
                Ingredient matchingIngredient = freeIngredient.Find(z => z.ingredient == used); // Finde die Ingredient
                
                if (matchingIngredient == null) {            // Wenn keine passende Ingredient...
                    match = false;                              // ... passt auf false ...
                    break;                                      // ... hier ENDE
                }
                found.Add(matchingIngredient);                  // Wenn passende Ingredient füge gefunden hinzu
                freeIngredient.Remove(matchingIngredient);      // und entferne freie Ingredient aus der Prüfliste
            }
            if (match) {                                        // Wenn es passt ...
                _activRecipe = recipe;                            
                _ingredientsInUse = found;
                Debug.Log("FOUND RECEPIE " + recipe.result);
                return;
            }
        }
    }

    private void CookingDone() {                           
        Recipes.Recipe finishedRecipe = _activRecipe;
        _activRecipe = null;
        _timer = 0;

        foreach (Ingredient ingredient in _ingredientsInUse) {      // Für jede Ingredient in verwendete Zutaten ...
            _ingredientsInPot.Remove(ingredient);                   // ... entferne ingredient aus dem Topf ...
            Destroy(ingredient.gameObject);                         // ... zerstöre das Zutaten Objekt ...
        }
        _ingredientsInUse.Clear();                                  // ... leere die verwendete Ingredient
        
        foreach (GameObject prefab in foodPrefab) {                                                    // Für jedes Objekt im foodPrefab ...
            Food food = prefab.GetComponent<Food>();                                                 // ... hohl dir die Informationen aus Food
            if (food != null && food.food == finishedRecipe.result) {                           // Wenn das prefab ein Food ist und egebenis eines Rezepts ...
                GameObject newFood = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);  // ... erzeuge das Objekt am SpawnPunkt ...
                Rigidbody rigid = newFood.GetComponent<Rigidbody>();                                 // ... hohl dir den Rigidbody des neuen Essens

                if (rigid != null) {                                             // Wenn es einen Body hat ...
                    Vector3 richtung =new Vector3(                                  // ... gib ihm ne richtung ...
                        Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));          // ... gib ihr zufallswerte ...
                    rigid.AddForce(richtung.normalized * 8f, ForceMode.Impulse);    // ... schleuder es in die richtung!
                }

                Debug.Log("COOKED " + finishedRecipe.result);    
                CheckRecipe();                                      // Führe Prüfung aus
                return;                                             // Beende
            }
        }
    }
}
