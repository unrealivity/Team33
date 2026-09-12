using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input stuff")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _lookAction;
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _grabAction;
    private InputAction _interactAction;
    
    
    [Header("Horizontal Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float speedFactor;
    private float _forwardMovementValue;
    private float _rightMovementValue;
    private Vector3 _movementToAdd;
    
    [Header("Capsule Float")]
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    [SerializeField] private LayerMask floatRaycastLayer;
    
    [Header("View Settings")]
    [SerializeField] private float mouseSensitivity;
    private Camera _camera;
    private float _mouseX;
    private float _mouseY;
    private float _rotationPitch;
    private float _rotationYaw;

    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange;

    [Header("Grab Settings")]
    [SerializeField] private float grabRange;
    [SerializeField] private float grabStrength;
    [SerializeField] private float idealDistance;
    [SerializeField] private float grabForceDamper;
    [SerializeField] private GameObject holdPointGameObject;
    [SerializeField] private LayerMask grabRaycastLayer;

    private GameObject _objectHeldInHand;
    private LayerMask _playerLayer;
    private Rigidbody _rigidbody;

    private Transform _holdPoint;
    

    private void Awake()
    {   
        _moveAction = inputActionAsset.FindAction("Move");
        _lookAction = inputActionAsset.FindAction("Look");
        _grabAction = inputActionAsset.FindAction("Grab");
        _interactAction = inputActionAsset.FindAction("Interact");

        _interactAction.performed += InteractPerformed; 
        _grabAction.performed += GrabActionOnPerformed;
        _grabAction.canceled += GrabActionOnCanceled;
        
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        
        _playerLayer = LayerMask.GetMask("Default");
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
        if (!ForwardRaycast(grabRange, grabRaycastLayer, out RaycastHit grabHit)) return;

        _objectHeldInHand = grabHit.collider.gameObject;
        _objectHeldInHand?.GetComponent<Grabbable>()?.PickUp(_holdPoint,grabStrength,grabForceDamper);
    }
    

    private void InteractPerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interact");
    }
    private void Update()
    {   
        //read movement input
        _forwardMovementValue = _moveAction.ReadValue<Vector2>().y;
        _rightMovementValue = _moveAction.ReadValue<Vector2>().x;

        //read mouse input
        _mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        _mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;
        
        
        //calculate vertical camera rotation
        _rotationPitch -= _mouseY;
        _rotationPitch = Mathf.Clamp(_rotationPitch, -82f, 82f);
        
        _rotationYaw += _mouseX;
        
        // pitch camera vertically
        _camera.transform.localRotation = Quaternion.Euler(_rotationPitch, _rotationYaw, 0f);
    }

    private void FixedUpdate()
    {   
        
        // raycast down to float capsule over floor
        bool raycastDidHit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, rideHeight, floatRaycastLayer);
        if (raycastDidHit)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            float relativeVelocity = Vector3.Dot(Vector3.down,velocity);
            float x = raycastHit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);
            _rigidbody.AddForce(Vector3.down * springForce);
        }
        
        var cameraForward = new Vector3(_camera.transform.forward.x,0,_camera.transform.forward.z);
        var cameraRight = new Vector3(_camera.transform.right.x,0,_camera.transform.right.z) ;
        Vector3 targetDirection = (cameraForward * _forwardMovementValue + cameraRight * _rightMovementValue).normalized;
        Vector3 targetMovement = targetDirection * maxSpeed;
        Vector3 velocityChange = targetMovement-(new Vector3(_rigidbody.linearVelocity.x,0,_rigidbody.linearVelocity.z));
        velocityChange = Vector3.ClampMagnitude(velocityChange,acceleration);
        
        _rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
    }

    private bool ForwardRaycast(float rayLength, LayerMask layerMask, out RaycastHit hit)
    {
        return Physics.Raycast(_camera.transform.position, _camera.transform.forward, out hit, rayLength, layerMask);
    }
    
}
