using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _moveAction;
    
    [Header("Horizontal Movement")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    
    private float _forwardMovementValue;
    private float _rightMovementValue;
    private Vector3 _movementToAdd;
    
    [Header("Capsule Float")]
    [SerializeField] private float rideHeight;
    [SerializeField] private float rideSpringStrength;
    [SerializeField] private float rideSpringDamper;
    [SerializeField] private LayerMask floatRaycastLayer;
    
    private Rigidbody _rigidbody;
    private Camera _camera;


    private void Awake()
    {
        _moveAction = inputActionAsset.FindAction("Move");
        
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        //read movement input
        _forwardMovementValue = _moveAction.ReadValue<Vector2>().y;
        _rightMovementValue = _moveAction.ReadValue<Vector2>().x;
    }

    private void FixedUpdate()
    {
        FloatCapsule();
        Vector3 velocityChange = CalculateVelocityChange();
        _rigidbody.AddForce(velocityChange,ForceMode.VelocityChange);
    }
    
    private Vector3 CalculateVelocityChange()
    {
        var cameraForward = new Vector3(_camera.transform.forward.x,0,_camera.transform.forward.z);
        var cameraRight = new Vector3(_camera.transform.right.x,0,_camera.transform.right.z) ;
        Vector3 targetDirection = (cameraForward * _forwardMovementValue + cameraRight * _rightMovementValue).normalized;
        Vector3 targetMovement = targetDirection * maxSpeed;
        Vector3 velocityChange = targetMovement-(new Vector3(_rigidbody.linearVelocity.x,0,_rigidbody.linearVelocity.z));
        velocityChange = Vector3.ClampMagnitude(velocityChange,acceleration);
        return velocityChange;
    }

    private void FloatCapsule()
    {   
        // raycast down to float capsule over floor
        bool raycastDidHit = Physics.Raycast(transform.position, Vector3.down, out RaycastHit raycastHit, rideHeight, floatRaycastLayer);
        if (!raycastDidHit) return;
        
        Vector3 velocity = _rigidbody.linearVelocity;
        float relativeVelocity = Vector3.Dot(Vector3.down,velocity);
        float x = raycastHit.distance - rideHeight;
        float springForce = (x * rideSpringStrength) - (relativeVelocity * rideSpringDamper);
        _rigidbody.AddForce(Vector3.down * springForce);
    }
}
