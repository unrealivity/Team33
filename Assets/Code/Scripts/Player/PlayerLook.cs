using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{   
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _lookAction;
    
    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float topLimit;
    [SerializeField] private float bottomLimit;
    
    private float _mouseX;
    private float _mouseY;
    private float _rotationPitch;
    private float _rotationYaw;
    
    private Camera _camera;



    private void Awake()
    {
        _lookAction = inputActionAsset.FindAction("Look");
        
        _camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        //read mouse input
        _mouseX = _lookAction.ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
        _mouseY = _lookAction.ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;
        
        //calculate vertical camera rotation
        _rotationPitch -= _mouseY;
        _rotationPitch = Mathf.Clamp(_rotationPitch, -bottomLimit, topLimit);
        
        _rotationYaw += _mouseX;
        
        _camera.transform.localRotation = Quaternion.Euler(_rotationPitch, _rotationYaw, 0f);
    }
}
