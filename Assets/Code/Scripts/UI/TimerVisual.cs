using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerVisual : MonoBehaviour
{
    private Image _image;
    private bool _isRunning;

    private void Awake()
    {
        if (TryGetComponent(out Image foundImageComponent))
        {
            _image = foundImageComponent;
        }
        else
        {
            Debug.LogError(this.gameObject+" is missing an Image component.");
        }
    }

    internal void StartTimer(float timeInSeconds)
    {
        StartCoroutine(RunTimer(timeInSeconds));
    }

    private IEnumerator RunTimer(float durationInSeconds)
    {
        _image.fillAmount = 1;
        float elapsed = 0;

        while (elapsed < durationInSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / durationInSeconds);
            _image.fillAmount = Mathf.Lerp(1f, 0, t);
            yield return null;
        }
        _image.fillAmount = 0;
    }
    
}
