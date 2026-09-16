using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _interactAction;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayer;
    
    private IHoverable _currentHoverable;
    private IInteractable _currentInteractable;
    private RaycastHit _hoverHit;

    private Camera _camera;
    
    private void Awake()
    {
        _interactAction = inputActionAsset.FindAction("Interact");
        _interactAction.performed += InteractPerformed; 
        
        _camera = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        CheckHover();
    }

    private void CheckHover()
    {   
        bool hit = Physics.Raycast(_camera.transform.position, _camera.transform.forward, out _hoverHit, interactionRange, interactionLayer);
        
        if (hit && _hoverHit.collider.TryGetComponent(out IHoverable hitHoverable))
        {
            //quit if hovering the same thing
            if (hitHoverable == _currentHoverable) return;
            _currentHoverable?.OnHoverExit();
            _currentHoverable = hitHoverable;
            _currentHoverable.OnHoverEnter();
        }
        else
        {   
            _currentHoverable?.OnHoverExit();
            _currentHoverable = null;
        }

        if (_currentHoverable == null)
        {
            _currentInteractable = null;
        }
    }
    
    private void InteractPerformed(InputAction.CallbackContext ctx)
    {
        if (_currentHoverable == null) return;
        if (!_hoverHit.collider.TryGetComponent(out IInteractable hitInteractable)) return;
        if (hitInteractable == _currentInteractable) return;
        
        _currentInteractable = hitInteractable;
        _currentInteractable.Interact();
        _currentInteractable = null;
    }
}
