using UnityEngine;
public enum IngredientType { // Liste aller Zutaten
    Empty,
    Bean,
    Sugar,
    Peach,
    slicedPech
}
public class Ingredient : MonoBehaviour
{
    public IngredientType ingredient;
}
