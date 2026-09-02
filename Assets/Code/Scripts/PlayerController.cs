using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour
{
    //input
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _lookAction;
    private InputAction _moveAction;
    
    
    private Rigidbody _rigidbody;
    private Vector3 _movementToAdd;
    [SerializeField] private float playerSpeed;

    private Camera _camera; 
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float xRotation;

    private void Awake()
    {
        _moveAction = inputActionAsset.FindAction("Move");
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GetComponentInChildren<Camera>();
        _lookAction = inputActionAsset.FindAction("Look");
    }
    void Update()
    {   
        // Mouse look
        float mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        float mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;
        
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        
        // Rotate camera vertically
        _camera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        
        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        _movementToAdd  = new Vector3(_moveAction.ReadValue<Vector2>().x, 0, _moveAction.ReadValue<Vector2>().y);
    }

    private void FixedUpdate()
    {   
        Vector3 worldMovement = transform.TransformDirection(_movementToAdd);
        _rigidbody.AddForce(worldMovement * playerSpeed);
    }

}
