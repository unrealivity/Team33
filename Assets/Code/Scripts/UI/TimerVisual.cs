using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerVisual : MonoBehaviour
{   
    [SerializeField] private Image image;
    TimedTask _boundTask;
    

    public event Action OnFeedbackComplete;
    
    internal void Bind(TimedTask dishTask)
    {
        UnBind(); // safety if Bind is ever called twice
        _boundTask = dishTask;
        image.fillAmount = 1f;
        _boundTask.OnProgress += HandleProgress;
        _boundTask.OnComplete += HandleComplete;
    }

    private void UnBind()
    {
        if (_boundTask == null) return;
        _boundTask.OnProgress -= HandleProgress;
        _boundTask.OnComplete -= HandleComplete;
        _boundTask = null;
    }
    
    private void HandleProgress(float progress)
    {
        image.fillAmount = 1f - progress;    
    }

    private void HandleComplete()
    {
        image.fillAmount = 0;
    }

    internal void PlayFeedback(bool wasSuccessful)
    {
        UnBind();
        if (wasSuccessful)
        {
            StartCoroutine(FeedbackRoutine(Color.mediumSpringGreen));
        }
        else
        {
            StartCoroutine(FeedbackRoutine(Color.softRed));
        }
    }

    private IEnumerator FeedbackRoutine(Color feedbackColor)
    {
        image.color = feedbackColor;
        image.fillAmount = 1f;
        Vector3 punch = Vector3.one * 1.2f;
        float t = 0f;
        float feedBackDuration = 0.8f;
        while (t < feedBackDuration)
        {
            t += Time.deltaTime;
            image.fillAmount = t / feedBackDuration;
            transform.localScale = Vector3.Lerp(punch, Vector3.one, t / feedBackDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;
        OnFeedbackComplete?.Invoke();
    }
    private void OnDestroy()
    {
        UnBind();
    }
}
