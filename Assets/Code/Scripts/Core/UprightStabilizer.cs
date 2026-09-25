using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class UprightStabilizer : MonoBehaviour
{
    [SerializeField] private float uprightTorque = 50f;
    [SerializeField] private float damping = 2f;
    [SerializeField] private float maxTorque = 150f;
    [SerializeField] private Vector3 targetUp = Vector3.up;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Quaternion deltaRotation = Quaternion.FromToRotation(transform.up, targetUp);
        deltaRotation.ToAngleAxis(out float angleDegrees, out Vector3 axis);

        if (angleDegrees > 180f) angleDegrees -= 360f;

        Vector3 correctiveTorque = axis * (angleDegrees * Mathf.Deg2Rad * uprightTorque);
        Vector3 dampingTorque = -_rigidbody.angularVelocity * damping;

        Vector3 totalTorque = Vector3.ClampMagnitude(correctiveTorque + dampingTorque, maxTorque);
        _rigidbody.AddTorque(totalTorque, ForceMode.Acceleration);
    }
}
