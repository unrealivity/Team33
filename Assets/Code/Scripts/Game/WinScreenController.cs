using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScreenController : MonoBehaviour
{
    [SerializeField] private LuzzAnimationController _animationController;
    bool _arrivedAtLocation = false;
    public static event Action Victory;
    
    private void Start()
    {   
        _animationController.StartRoute(0);
        StartCoroutine(PlayDance());
        Victory?.Invoke();
    }
    
    private IEnumerator PlayDance()
    {   
        _arrivedAtLocation = false;
        void ArriveHandler() => _arrivedAtLocation = true;
        LuzzAnimationController.RouteComplete += ArriveHandler;
        yield return new WaitUntil(() => _arrivedAtLocation );
        LuzzAnimationController.RouteComplete -= ArriveHandler;
        _animationController.StartRoute(1);
        StartCoroutine(PlayDance());
    }
}
