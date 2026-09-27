using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PauseManager.PauseStateChanged += OnPauseStateChanged;
        WinScreenController.Victory += OnVictory;
    }
    
    private void OnDestroy()
    {
        PauseManager.PauseStateChanged -= OnPauseStateChanged;
        WinScreenController.Victory -= OnVictory;
    }

    private void OnVictory()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void OnPauseStateChanged(bool isPaused)
    {
        if (isPaused && PauseManager.PauseOwner == null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;    
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
