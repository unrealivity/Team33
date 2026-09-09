using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Cooking/Recipes")]   // Erstellt das ScriptableObject

public class Recipes : ScriptableObject {
    [System.Serializable]
    public class Recipe {
        public List<IngredientType> ingredient; // Liste für eingang ingredient
        public FoodType result;                // ausgang food
        public float cookingTime;               // Zahlenwert für Kochdauer
    }
    
    public List<Recipe> recipe;                 // Weitere Recipes
}

