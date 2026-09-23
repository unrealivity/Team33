using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _interactAction;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange;
    [SerializeField] private LayerMask interactionLayer;
    
    private readonly RaycastHit[] _hitBuffer = new RaycastHit[16];
    private readonly HashSet<IHoverable> _currentlyHovered = new();
    private readonly HashSet<IHoverable> _hitThisFrame = new();
    private readonly List<IHoverable> _toExit = new();

    private IInteractable _closestInteractable;

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
        int hitCount = Physics.RaycastNonAlloc(
            _camera.transform.position,
            _camera.transform.forward,
            _hitBuffer,
            interactionRange,
            interactionLayer);
        
        Array.Sort(_hitBuffer, 0, hitCount, DistanceComparer.Instance);

        _closestInteractable = null;
        _hitThisFrame.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = _hitBuffer[i].collider;

            if (hitCollider.TryGetComponent(out IHoverable hoverable))
            {
                _hitThisFrame.Add(hoverable);
                if (_currentlyHovered.Add(hoverable))
                    hoverable.OnHoverEnter(); // wasn't hovered last frame
            }

            if (_closestInteractable == null && hitCollider.TryGetComponent(out IInteractable interactable))
                _closestInteractable = interactable;
        }

        _toExit.Clear();
        foreach (IHoverable hovered in _currentlyHovered)
        {
            if (!_hitThisFrame.Contains(hovered))
                _toExit.Add(hovered);
        }
        foreach (IHoverable exited in _toExit)
        {
            exited.OnHoverExit();
            _currentlyHovered.Remove(exited);
        }
    }
    
    private void InteractPerformed(InputAction.CallbackContext ctx)
    {
        _closestInteractable?.Interact();
    }

    private class DistanceComparer : IComparer<RaycastHit>
    {
        public static readonly DistanceComparer Instance = new();
        public int Compare(RaycastHit a, RaycastHit b)
        { 
            return a.distance.CompareTo(b.distance);
        }
    }
}
