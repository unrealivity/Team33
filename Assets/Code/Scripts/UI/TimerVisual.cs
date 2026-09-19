using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerVisual : MonoBehaviour
{
    [SerializeField] private Image image;
    TimedTask _boundTask;


    
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

    private void OnDestroy()
    {
        UnBind();
    }
}
