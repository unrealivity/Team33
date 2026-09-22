using System;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    private Camera _camera;
    private void Awake()
    {
        _camera = Camera.main;
    }
    void Update()
    {
        Quaternion rotation = _camera.transform.rotation;
        transform.LookAt(transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }
}
