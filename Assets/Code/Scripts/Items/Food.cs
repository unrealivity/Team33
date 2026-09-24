using System;
using UnityEngine;


public class Food : MonoBehaviour {
    
    public ItemType foodType;
    
    private Rigidbody _rigidbody;
    private Collider _collider;
    private Grabbable _grabbable;
    
    private bool _collecting;
    private bool _atTable;
    
    private float _collectForce;
    private float _forceDamper;
    private float _maxCollectForce;
    private Vector3 _targetPosition;
    private float _bufferDistance;
    
    private int _ignoreRaycastLayer;
    
    public static event Action FoodWaitingOnTable;
    public static event Action FoodAte;
    
    private void Awake()
    {
        if (TryGetComponent(out Rigidbody foundRigidbody))
        {
            _rigidbody = foundRigidbody;
        }
        if (TryGetComponent(out Grabbable foundGrabbable))
        {
            _grabbable = foundGrabbable;
        }
        if (TryGetComponent(out Collider foundCollider))
        {
            _collider = foundCollider;
        }
        _ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
    }

    internal void CollectFood(    float collectForce, float forceDamper, float maxCollectForce, Vector3 targetPosition, float bufferDistance)
    {
        _collectForce = collectForce;
        _forceDamper = forceDamper;
        _maxCollectForce = maxCollectForce;
        _targetPosition = targetPosition;
        _bufferDistance = bufferDistance;

        _collider.excludeLayers = LayerMask.NameToLayer("Player");
        gameObject.layer = _ignoreRaycastLayer;
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        
        _collecting = true;
        
        _grabbable.Drop();
    }
    
    private void FixedUpdate()
    {   
        if(!_collecting) return;
        Vector3 toTarget = _targetPosition - _rigidbody.position;
        Vector3 velocityError = -_rigidbody.linearVelocity;
        
        Vector3 force = (toTarget * _collectForce) + (velocityError * _forceDamper);
        force = Vector3.ClampMagnitude(force, _maxCollectForce);
        
        _rigidbody.AddForce(force, ForceMode.Acceleration);

        if ((_rigidbody.position - _targetPosition).sqrMagnitude > _bufferDistance) return;
        if(!_atTable)
        {
            FoodArrivedAtTable();
            return;
        }
        EatFood();
    }
    
    private void FoodArrivedAtTable()
    {
        LuzzBehaviorController.VacuumFood += CollectFood;
        _atTable = true;
        FoodWaitingOnTable?.Invoke();
        _collecting = false;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.useGravity = true;
        _rigidbody.detectCollisions = true;
    }
    private void EatFood()
    {
        FoodAte?.Invoke();
        LuzzBehaviorController.VacuumFood -= CollectFood;
        PoolManager.Instance.Release(gameObject);
    }

    internal void ResetForReuse()
    {
        _collecting = false;
        _atTable = false;
        _rigidbody = null;
    }
}
