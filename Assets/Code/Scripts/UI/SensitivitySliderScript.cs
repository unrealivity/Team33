using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SensitivitySliderScript : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sensitivityTextMeshProUGUI;

    private void Start()
    {
        slider.value = PlayerSettings.MouseSensitivity;
        UpdateLabel(slider.value);
        slider.onValueChanged.AddListener(OnSliderChanged);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
    }

    private void OnSliderChanged(float value)
    {
        PlayerSettings.SetSensitivity(value);
        UpdateLabel(value);
    }

    private void UpdateLabel(float value)
    {
        float normalizedValue = Mathf.InverseLerp(slider.minValue, slider.maxValue, value);
        float displayValue = Mathf.RoundToInt(Mathf.Lerp(1f,100f, normalizedValue));
        sensitivityTextMeshProUGUI.text = "Sensitivity: " + displayValue;
    }
}