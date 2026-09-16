using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _grabAction;
    
    [Header("Grab Settings")]
    [SerializeField] private float grabRange;
    [SerializeField] private float grabStrength;
    [SerializeField] private float idealDistance;
    [SerializeField] private float grabForceDamper;
    [SerializeField] private float maxFollowForce;
    [SerializeField] private GameObject holdPointGameObject;
    [SerializeField] private LayerMask grabRaycastLayer;

    private GameObject _objectHeldInHand;
    private Rigidbody _rigidbody;

    private Transform _holdPoint;
    private Camera _camera;

    private void Awake()
    {   
        _grabAction = inputActionAsset.FindAction("Grab");
        _grabAction.performed += GrabActionOnPerformed;
        _grabAction.canceled += GrabActionOnCanceled;
        
        _camera = GetComponentInChildren<Camera>();
        
        holdPointGameObject.transform.Translate(0,0,idealDistance);
        _holdPoint = holdPointGameObject.transform;

    }
    private void GrabActionOnCanceled(InputAction.CallbackContext obj)
    {
        if (_objectHeldInHand != null)
        {
            _objectHeldInHand.GetComponent<Grabbable>()?.Drop();
        }
        _objectHeldInHand = null;
    }
    private void GrabActionOnPerformed(InputAction.CallbackContext obj)
    {
        if (!Physics.Raycast(_camera.transform.position, _camera.transform.forward,out RaycastHit grabHit, grabRange, grabRaycastLayer)) return;

        _objectHeldInHand = grabHit.collider.gameObject;
        _objectHeldInHand?.GetComponent<Grabbable>()?.PickUp(_holdPoint,grabStrength,grabForceDamper,maxFollowForce);
    }
}
