using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Cooking : Station {
    
    [SerializeField] private float foodSpawnForce = 8f;
    [SerializeField] private float ejectForce = 8f;
    
    [SerializeField] private Recipes recipeCollection;                 // Platz für Recipe script
    [SerializeField] private Transform spawnPoint;                     // Spawn für fertiges Food
    [SerializeField] private List<GameObject> foodPrefab;              // Prefabs von Spawnbarem
    [FormerlySerializedAs("EjectButton")]
    [SerializeField] private GameObject ejectButton;                   // Feld für Auswurf Button
    [SerializeField] private GameObject foodContainerParent;           // GameObject that parents instantiated foods
    
    private List<Rigidbody> _objectsInPot = new List<Rigidbody>();                // Liste für Objekte im Topf
    private List<Ingredient> _ingredientsInPot = new List<Ingredient>();          // Liste Zutaten im Topf
    private List<Ingredient> _ingredientsInUse = new List<Ingredient>();          // Liste Verwendung für Cooking
    private Recipes.Recipe _activeRecipe;                                          // Für Cooking aktives Recipe
    private float _timer;                                                         // Timer für Abgleich beim Cooking

    private void OnTriggerEnter(Collider other) {                                 // Wenn etwas rein fällt
        Rigidbody rigid = other.attachedRigidbody;
        Debug.Log("TRIGGER FOUND : " + other.gameObject.name);         
        Ingredient ingredient = other.GetComponent<Ingredient>();                 // Hohl das Zutaten Script vom Objekt
        Debug.Log("INGREDIENT FOUND : " + (ingredient != null));
        
        if (rigid != null && !_objectsInPot.Contains(rigid)) {
            _objectsInPot.Add(rigid);
            ejectButton.SetActive(true);
        }
        if (ingredient != null && !_ingredientsInPot.Contains(ingredient)) {   // wenns ne Ingredient ist und nicht im Topf...
            _ingredientsInPot.Add(ingredient);                                    // ... packs in den Topf ...
            
            CheckRecipe();                                                        // ... prüfe das Recipe
            Debug.Log("SHITS THROW'N IN "+ ingredient);                             
        }
    }
    private void OnTriggerExit(Collider other) {                    // Wenn etwas raus fällt
        Rigidbody rigid = other.attachedRigidbody;
        
        Ingredient ingredient = other.GetComponent<Ingredient>();   // Hohl info aus Ingredient

        if (rigid != null) {
            _objectsInPot.Remove(rigid);
        }
        if (_objectsInPot.Count == 0) {
            ejectButton.SetActive(false);
        }
        if (ingredient != null) {                                // Wenn es eine Ingredient ist...
            _ingredientsInPot.Remove(ingredient);                   // ... entferne sie aus dem Topf ...
            _activeRecipe = null;                                    // ... entferne aktives Recipe ...
            _timer = 0;                                             // ... setze den Timer auf null
            
            CheckRecipe();
            Debug.Log("SHITS THROW'N OUT "+ ingredient.ingredient);
        }
    }

    private void Start() {
        ejectButton.SetActive(false);
    }

    void Update()
    {
        if (_activeRecipe != null) {                       // Wenn Recipe aktiv ist ...
            _timer += Time.deltaTime;                     // ... erhöhe den Timer ...
            if (_timer >= _activeRecipe.prepareValue) {     // ... wenn Timer fertig ...
                CookingDone();                            // ... führe CookingDone aus
            }
        }
    }

    private void CheckRecipe() {                                       
        _activeRecipe = null;                                          
        _timer = 0;                                                     
        _ingredientsInUse.Clear();                                     

        foreach (Recipes.Recipe recipe in recipeCollection.recipeList) {                    // für jedes Recipe in Sammlung
            List<Ingredient> freeIngredient = new List<Ingredient>(_ingredientsInPot);  // kopiere ingredient im Topf in freie Zutaten
            List<Ingredient> found = new List<Ingredient>();                            // Speichere die Zutaten die zum Recipe passen
            
            bool match = true;                                                          // Bestätige das Recipe und Zutaten passen

            foreach (ItemType used in recipe.ingredients) {                                 // Für jede benötigte Ingredient 
                Ingredient matchingIngredient = freeIngredient.Find(z => z.ingredient == used); // Finde die Ingredient
                
                if (matchingIngredient == null) {            // Wenn keine passende Ingredient...
                    match = false;                              // ... passt auf false ...
                    break;                                      // ... hier ENDE
                }
                found.Add(matchingIngredient);                  // Wenn passende Ingredient füge gefunden hinzu
                freeIngredient.Remove(matchingIngredient);      // und entferne freie Ingredient aus der Prüfliste
            }
            if (match) {                                        // Wenn es passt ...
                _activeRecipe = recipe;                            
                _ingredientsInUse = found;
                Debug.Log("FOUND RECEPIE " + recipe.result);
                return;
            }
        }
    }

    private void CookingDone() {                           
        Recipes.Recipe finishedRecipe = _activeRecipe;
        _activeRecipe = null;
        _timer = 0;

        foreach (Ingredient ingredient in _ingredientsInUse) {      // Für jede Ingredient in verwendete Zutaten ...
            _ingredientsInPot.Remove(ingredient);                   // ... entferne ingredient aus dem Topf ...
            Rigidbody rigid = ingredient.GetComponent<Rigidbody>();
            if (rigid != null) {
                _objectsInPot.Remove(rigid);
            }
            Destroy(ingredient.gameObject);             // ... zerstöre das Zutaten Objekt ...
        }
        _ingredientsInUse.Clear();                      // ... leere die verwendete Ingredient

        foreach (GameObject prefab in foodPrefab) {     // Für jedes Objekt im foodPrefab ...
            Food food = prefab.GetComponent<Food>();    // ... hohl dir die Informationen aus Food

            if (food != null && food.foodType == finishedRecipe.result) {                            // Wenn das prefab ein Food ist und egebenis eines Rezepts ...
                GameObject newFood = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation); // ... erzeuge das Objekt am SpawnPunkt ...
                Rigidbody rigid = newFood.GetComponent<Rigidbody>();                                // ... hohl dir den Rigidbody des neuen Essens

                if (rigid != null) {                                          // Wenn es einen Body hat ...
                    Vector3 richtung = new Vector3(                              // ... gib ihm ne richtung ...
                        Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));       // ... gib ihr zufallswerte ...
                    rigid.AddForce(richtung.normalized * foodSpawnForce, ForceMode.Impulse); // ... schleuder es in die richtung!
                }
                Debug.Log("COOKED " + finishedRecipe.result);
                CheckRecipe();  // Führe Prüfung aus
                return;         // Beende
            }
        }
    }

    public override void Interact() {
        EjectObjects();
    }

    private void EjectObjects() {
        _activeRecipe = null;
        _timer = 0;
        _ingredientsInUse.Clear();
        _ingredientsInPot.Clear();

        List<Rigidbody> objectsToEject = new List<Rigidbody>(_objectsInPot);
        _objectsInPot.Clear();
        ejectButton.SetActive(false);
        foreach (Rigidbody rigid in objectsToEject) {
            Vector3 direction = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
            rigid.AddForce(direction.normalized * ejectForce, ForceMode.Impulse);
        }
    }
//TODO nur für Tests dannach Löschen...    
    [ContextMenu("Test Eject")]
    private void TestEject()
    {
        Interact();
    }
}
//TODO Soundeffekte für Schneiden(messer auf Holz) / Objekt fertig Geschnitten(Dumpfes Plop)
//TODO Debug löschen
//TODO Komentare aktuallisieren oder löschen