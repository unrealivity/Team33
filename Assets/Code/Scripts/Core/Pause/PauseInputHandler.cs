using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction _pauseAction;

    private void Awake()
    {
        _pauseAction = inputActionAsset.FindAction("Pause");
        _pauseAction.performed += OnPausePerformed;
    }

    private void OnDestroy()
    {
        _pauseAction.performed -= OnPausePerformed;
    }

    private void OnPausePerformed(InputAction.CallbackContext ctx) => PauseManager.TogglePause();
}
