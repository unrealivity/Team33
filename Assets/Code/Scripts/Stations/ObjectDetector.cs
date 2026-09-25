using System;
using UnityEngine;


public class ObjectDetector : MonoBehaviour
{
    public event Action<Ingredient> OnIngredientEnter;
    public event Action<Ingredient> OnIngredientExit;
    
    public event Action<Food> OnFoodEnter;
    public event Action<Food> OnFoodExit;
    
    public event Action<Collider> OnColliderEnter;
    public event Action<Collider> OnColliderExit;
    
    private void OnTriggerEnter(Collider other)
    {   
        OnColliderEnter?.Invoke(other);
        if(other.TryGetComponent(out Ingredient ingredient))
        {
            OnIngredientEnter?.Invoke(ingredient);
        }
        if(other.TryGetComponent(out Food food))
        {
            OnFoodEnter?.Invoke(food);
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        OnColliderExit?.Invoke(other);
        if(other.TryGetComponent(out Ingredient ingredient))
        {
            OnIngredientExit?.Invoke(ingredient);
        }
        if(other.TryGetComponent(out Food food))
        {
            OnFoodExit?.Invoke(food);
        }
    }
}
