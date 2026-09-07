using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
    
    [Header("View Settings")]
    [SerializeField] private float mouseSensitivity;
    private Camera _camera;
    private float _mouseX;
    private float _mouseY;
    private float _rotationPitch;
    private float _rotationYaw;

    private bool _mouseDown;
    
    
    
    private LayerMask _playerLayer;
    private Rigidbody _rigidbody;
    



    private void Awake()
    {   
        _moveAction = inputActionAsset.FindAction("Move");
        _lookAction = inputActionAsset.FindAction("Look");
        _grabAction = inputActionAsset.FindAction("Grab");
        _interactAction = inputActionAsset.FindAction("Interact");
        
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        
        _playerLayer = LayerMask.GetMask("Default");
    }
    void Update()
    {   
        //read movement input
        _forwardMovementValue = _moveAction.ReadValue<Vector2>().y;
        _rightMovementValue = _moveAction.ReadValue<Vector2>().x;

        //read mouse input
        _mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        _mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        _mouseDown = (_grabAction.ReadValue<float>() > 0.5f);
        
        
        
        //calculate vertical camera rotation
        _rotationPitch -= _mouseY;
        _rotationPitch = Mathf.Clamp(_rotationPitch, -82f, 82f);
        
        // pitch camera vertically
        _camera.transform.localRotation = Quaternion.Euler(_rotationPitch, 0f, 0f);
        if (_mouseDown)
        {
            Debug.Log("Mouse is Down!");  
        }

    }

    private void FixedUpdate()
    {
        bool raycastDidHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out RaycastHit raycastHit, rideHeight, _playerLayer);
        if (raycastDidHit)
        {
            Vector3 velocity = _rigidbody.linearVelocity;
            float relativeVelocity = Vector3.Dot(Vector3.down,velocity);
            float x = raycastHit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);
            _rigidbody.AddForce(Vector3.down * springForce);
        }
        
        _rotationYaw += _mouseX;
        _rigidbody.MoveRotation(Quaternion.Euler(0f,_rotationYaw,0f));

        
        Vector3 targetDirection = (transform.forward * _forwardMovementValue + transform.right * _rightMovementValue).normalized;
        Vector3 targetMovement = targetDirection * maxSpeed;
        Vector3 velocityChange = targetMovement-(new Vector3(_rigidbody.linearVelocity.x,0,_rigidbody.linearVelocity.z));
        velocityChange = Vector3.ClampMagnitude(velocityChange,acceleration);
        
        _rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
    }

    private void OnLook(InputAction.CallbackContext context) 
    {
        _mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        _mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;
    }
    
}
