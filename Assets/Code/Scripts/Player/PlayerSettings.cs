using UnityEngine;
using System;

public static class PlayerSettings
{
    private const string SensitivityKey = "Sensitivity";
    private const float DefaultSensitivity = 90f;

    public static float MouseSensitivity { get; private set; } =
        PlayerPrefs.GetFloat(SensitivityKey, DefaultSensitivity);

    public static event Action<float> SensitivityChanged;

    public static void SetSensitivity(float value)
    {
        if (Mathf.Approximately(MouseSensitivity, value)) return;

        MouseSensitivity = value;
        PlayerPrefs.SetFloat(SensitivityKey, value);
        PlayerPrefs.Save();
        SensitivityChanged?.Invoke(value);
    }
}
