using System;
using UnityEngine;

public abstract class Station : MonoBehaviour,IHoverable,IInteractable
{
    [SerializeField] private GameObject interactionIndicator;
    
    public virtual void OnHoverEnter()
    {
        interactionIndicator.SetActive(true);
    }
    public virtual void OnHoverExit()
    {
        interactionIndicator.SetActive(false);
    }
    public abstract void Interact();
}
