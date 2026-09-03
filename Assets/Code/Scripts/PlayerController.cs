using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //input
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _lookAction;
    private InputAction _moveAction;
    
    
    private Rigidbody _rigidbody;
    private Vector3 _movementToAdd;
    

    [Header("Horizontal Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxAcceleration;
    [SerializeField] private float speedFactor;
    
    [Header("Capsule Float")]
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    
    [Header("View Settings")]
    [SerializeField] private float mouseSensitivity;
    private Camera _camera;
    private float mouseX;
    private float mouseY;
    private float rotationPitch;
    private float RotationYaw;
    
    
    private LayerMask _playerLayer;
    
    
    private Vector2 inputMovementValues;



    private void Awake()
    {
        _moveAction = inputActionAsset.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        _lookAction = inputActionAsset.FindAction("Look");
        _playerLayer = LayerMask.GetMask("Default");
    }
    void Update()
    {   

        inputMovementValues = new Vector2(_moveAction.ReadValue<Vector2>().x, _moveAction.ReadValue<Vector2>().y);


        // Mouse look
        mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

        rotationPitch -= mouseY;
        rotationPitch = Mathf.Clamp(rotationPitch, -85f, 85f);
        
        // pitch camera vertically
        _camera.transform.localRotation = Quaternion.Euler(rotationPitch, 0f, 0f);
        

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
            Debug.DrawLine(transform.position,transform.position+(Vector3.down*springForce),Color.yellow);
            _rigidbody.AddForce(Vector3.down * springForce);
        }
        
        RotationYaw += mouseX;
        _rigidbody.MoveRotation(Quaternion.Euler(0f,RotationYaw,0f));

        Vector3 targetDirection = (transform.forward * inputMovementValues.y + transform.right * inputMovementValues.x).normalized;
        
        Vector3 targetMovement = targetDirection * maxSpeed;
        Vector3 velocityChange = targetMovement-(new Vector3(_rigidbody.linearVelocity.x,0,_rigidbody.linearVelocity.z));
        velocityChange = Vector3.ClampMagnitude(velocityChange,maxAcceleration*Time.fixedDeltaTime);


        _rigidbody.AddForce(velocityChange * speedFactor,ForceMode.VelocityChange);
    }

}
