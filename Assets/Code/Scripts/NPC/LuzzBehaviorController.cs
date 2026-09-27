using System;
using System.Collections;
using UnityEngine;

public class LuzzBehaviorController : MonoBehaviour
{   
    [Header("Vacuum Settings")]
    [SerializeField] private Transform targetLocation;
    [SerializeField] private float maxCollectForce;
    [SerializeField] private float collectForce;
    [SerializeField] private float forceDamper;
    [SerializeField] private float bufferDistance;
    
    [Header("Eating Animation")]
    [SerializeField] private float durationBetweenEatingAnimationFrames;
    [SerializeField] private float eatingDuration;
        
    private bool _arrivedAtLocation;
    public static event Action<float,float ,float ,Vector3 ,float > VacuumFood;
    private bool _isOnTrip;
    private bool _tripQueued;
    private LuzzAnimationController _luzzAnimationController;
    
    private void Awake()
    {
        Food.FoodWaitingOnTable += TryGoOnEatingTrip;
        if (TryGetComponent(out LuzzAnimationController luzzAnimationController))
        {
            _luzzAnimationController = luzzAnimationController;
        }
    }
    private void OnDisable()
    {
        Food.FoodWaitingOnTable -= TryGoOnEatingTrip;
    }
    private void TryGoOnEatingTrip()
    {
        if(_tripQueued)return;
        if (_isOnTrip)
        {
            _tripQueued = true;
            StartCoroutine(QueueTrip());
            return;
        }
        StartCoroutine(EatingTrip());
    }
    
    private IEnumerator EatingTrip()
    {
        _isOnTrip = true;
        _arrivedAtLocation = false;
        _luzzAnimationController.StartRoute(0);
        void ArriveHandler() => _arrivedAtLocation = true;
        LuzzAnimationController.RouteComplete += ArriveHandler;
        yield return new WaitUntil(() => _arrivedAtLocation );
        LuzzAnimationController.RouteComplete -= ArriveHandler;
        yield return StartCoroutine(EatFood(eatingDuration,durationBetweenEatingAnimationFrames));
        StartCoroutine(ReturnTrip());
    }
    private IEnumerator EatFood(float duration,float durationBetweenAnimationFrames)
    {
        VacuumFood?.Invoke(  collectForce,  forceDamper,  maxCollectForce,  targetLocation.position,  bufferDistance);
        var wait = new WaitForSeconds(durationBetweenAnimationFrames); // cached, no per-loop allocation
        float endTime = Time.time + duration;

        while (Time.time < endTime)
        {
            _luzzAnimationController.Open();
            yield return wait;

            _luzzAnimationController.Close();
            yield return wait;
        }

        _luzzAnimationController.Close();
    }
    private IEnumerator ReturnTrip()
    {
        _arrivedAtLocation = false;
        _luzzAnimationController.StartRoute(1);
        void ArriveHandler() => _arrivedAtLocation = true;
        LuzzAnimationController.RouteComplete += ArriveHandler;
        yield return new WaitUntil(() => _arrivedAtLocation );
        LuzzAnimationController.RouteComplete -= ArriveHandler;
        _isOnTrip = false;
    }
    private IEnumerator QueueTrip()
    {
        yield return new WaitUntil(() => !_isOnTrip);
        _tripQueued = false;
        StartCoroutine(EatingTrip());
    }
    
}
