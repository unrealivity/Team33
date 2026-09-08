using UnityEngine;
public enum IngredientType { // Liste aller Zutaten
    Empty,
    Bean,
    Shugar,
    Peach,
    slicedPech
}
public class Ingredient : MonoBehaviour
{
    public IngredientType ingredient;
}
