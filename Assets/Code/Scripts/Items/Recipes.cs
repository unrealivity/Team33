using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipes", menuName = "Cooking/Recipes")]   // Erstellt das ScriptableObject

public class Recipes : ScriptableObject
{
    [System.Serializable]
    public class Recipe {
        public List<ItemType> ingredients; // Liste für eingang ingredient
        public ItemType result;                // ausgang food
        public float prepareValue;               // Zahlenwert für Kochdauer/Schnittanzahl etc
    }
    
    public List<Recipe> recipeList;                 // Weitere Recipes
 
}

