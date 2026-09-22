using System;
using UnityEngine;


public class Food : MonoBehaviour {
    public ItemType foodType;
    private bool _collecting;
    private Rigidbody _rigidbody;
    private float _collectForce;
    private float _forceDamper;
    private float _maxCollectForce;
    private Vector3 _targetPosition;
    private float _bufferDistance;
    public static event Action<ItemType> FoodCollected;
    private Grabbable _grabbable; 
    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            _rigidbody = rigidbody;
        }
        if (TryGetComponent<Grabbable>(out Grabbable grabbable))
        {
            _grabbable = grabbable;
        }
    }

    internal void CollectFood(    float collectForce, float forceDamper, float maxCollectForce, Vector3 targetPosition, float bufferDistance)
    {
        _collectForce = collectForce;
        _forceDamper = forceDamper;
        _maxCollectForce = maxCollectForce;
        _targetPosition = targetPosition;
        _bufferDistance = bufferDistance;
        
        _rigidbody.useGravity = false;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _rigidbody.detectCollisions = false;

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
        FoodCollected?.Invoke(foodType);
        PoolManager.Instance.Release(gameObject);
        _collecting = false;
    }

    internal void ResetForReuse()
    {
        _collecting = false;
    }
}
