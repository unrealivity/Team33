using System;
using UnityEngine;

public class TimedTask : ICompletable
{
    private float Duration { get; }
    private float ElapsedTime  { get ; set; }
    private float Progress => Mathf.Clamp01(ElapsedTime / Duration);
    public bool IsComplete { get; private set; }

    public event Action OnComplete;
    public event Action<float> OnProgress;
    
    
    
    public TimedTask(float duration)
    {
        Duration = duration;
    }
    
    public void Tick(float deltaTime)
    {   
        if (IsComplete) return;
        ElapsedTime += deltaTime;
        OnProgress?.Invoke(Progress);
        
        if (ElapsedTime >= Duration)
        {   
            IsComplete =  true;
            OnComplete?.Invoke();
        }
    }
}
