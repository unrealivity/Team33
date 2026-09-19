using System;
using UnityEngine;

public class IngredientDetector : MonoBehaviour
{
    public event Action<Ingredient> OnIngredientEnter;
    public event Action<Ingredient> OnIngredientExit;
    
    public event Action<Collider> OnColliderEnter;
    public event Action<Collider> OnColliderExit;
    
    private void OnTriggerEnter(Collider other)
    {   
        OnColliderEnter?.Invoke(other);
        if(other.TryGetComponent(out Ingredient ingredient))
        {
            OnIngredientEnter?.Invoke(ingredient);
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        OnColliderExit?.Invoke(other);
        if(other.TryGetComponent(out Ingredient ingredient))
        {
            OnIngredientExit?.Invoke(ingredient);
        }
    }
}
