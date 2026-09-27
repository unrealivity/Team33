using System;
using UnityEngine;

public static class PauseManager
{
    public static bool IsPaused { get; private set; }
    public static IInteractable PauseOwner { get; private set; }
    public static event Action<bool> PauseStateChanged;

    public static void SetPaused(bool paused, IInteractable owner = null)
    {
        if (IsPaused == paused) return;

        IsPaused = paused;
        PauseOwner = paused ? owner : null;
        Time.timeScale = paused ? 0f : 1f;
        PauseStateChanged?.Invoke(paused);
    }

    public static void TogglePause() => SetPaused(!IsPaused);
}