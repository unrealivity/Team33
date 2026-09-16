using Unity.VisualScripting;
using UnityEngine;

public class Grabbable : MonoBehaviour,IHoverable
{
    private Rigidbody _rigidbody;
    private Transform _holdTarget;
    private bool _isHeld;
    private float _grabForce;
    private float _forceDamper;
    private float _maxFollowForce;
    

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    public void PickUp(Transform holdTarget, float grabForce, float grabForceDamper, float maxFollowForce)
    {
        _holdTarget = holdTarget;
        _isHeld = true;
        _grabForce = grabForce;
        _forceDamper = grabForceDamper;
        _maxFollowForce = maxFollowForce;
    }

    public void Drop()
    {
        _isHeld = false;
        _holdTarget = null;
    }

    private void FixedUpdate()
    {
        if (!_isHeld) return;
        
        Vector3 toTarget = _holdTarget.position - _rigidbody.position;
        Vector3 velocityError = -_rigidbody.linearVelocity;
        
        Vector3 force = (toTarget * _grabForce) + (velocityError * _forceDamper);
        force = Vector3.ClampMagnitude(force, _maxFollowForce);
        
        _rigidbody.AddForce(force, ForceMode.Acceleration);
    }


    public void OnHoverEnter()
    {
        throw new System.NotImplementedException();
    }
    public void OnHoverExit()
    {
        throw new System.NotImplementedException();
    }
}
