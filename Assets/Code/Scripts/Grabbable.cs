using Unity.VisualScripting;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Transform _holdTarget;
    private bool _isHeld;
    private float _grabForce;
    private float _forceDamper;

    

    private void Awake() => _rigidbody = GetComponent<Rigidbody>();

    public void PickUp(Transform holdTarget, float grabForce, float grabForceDamper)
    {
        _holdTarget = holdTarget;
        _isHeld = true;
        _grabForce = grabForce;
        _forceDamper = grabForceDamper;
    }

    public void Drop()
    {
        _isHeld = false;
        _holdTarget = null;
    }

    private void FixedUpdate()
    {
        if (!_isHeld) return;
        var x = _holdTarget.position - _rigidbody.position;
        _rigidbody.AddForce((x*_grabForce)-(_rigidbody.linearVelocity*_forceDamper));
    }
}
