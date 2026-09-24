using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class LuzzAnimationController : MonoBehaviour
{
    [Serializable] // Auswahl für wegpunkte / animation vom wegpunkt aus / geschwindigkeit bis zum nächsten wegpunkt
    public class RouteStep {
        public Transform waypoint;
        public AnimationClip animation;
        public float moveSpeed = 2f;
    }
    [Serializable] 
    public class Route {
        public List<RouteStep> steps;
    }

    [Header("Routes")] // Liste der Routen
    [SerializeField] private List<Route> routes;

    [Header("Movement")]
    [SerializeField] private float waypointDistance = 0.1f;
    [SerializeField] private int startRoute = 0;
    [SerializeField] private float turnSpeed = 180f;

    [Header("Animation")] // Felder um die AnimationsClips einfacher zu verwenden
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip animClose;
    [SerializeField] private AnimationClip animOpen;
    [SerializeField] private AnimationClip animEnter;
    [SerializeField] private AnimationClip animRest;
    [SerializeField] private AnimationClip animShoot;
    [SerializeField] private AnimationClip animWalk;
    
    private AnimationClip _currentAnimation;
    private PlayableGraph _animationGraph;
    private Rigidbody _rigidbody;
    private int _currentRoute;
    private int _currentWaypoint;
    private bool _isRouteRunning;

    public static event Action RouteComplete;
    
    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _currentRoute = startRoute;
        _currentWaypoint = 0;
    }
    private void FixedUpdate() {
        if (_isRouteRunning) {
            MoveAlongRoute();
        }
    }
    private void MoveAlongRoute() {
        if (routes.Count == 0) return;
        if (_currentRoute >= routes.Count) return;
        
        Route route = routes[_currentRoute];
        
        if (route.steps.Count == 0) return;
        if (_currentWaypoint >= route.steps.Count) return;

        RouteStep step = route.steps[_currentWaypoint];

        if (step.animation != null && step.animation != _currentAnimation) {
            PlayAnimation(step.animation);
        }

        if (_currentWaypoint >= route.steps.Count - 1) {
            RouteComplete?.Invoke();
            _isRouteRunning = false;
            return;
        }

        Transform target = route.steps[_currentWaypoint + 1].waypoint;
        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion newRotation = Quaternion.RotateTowards(_rigidbody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
            _rigidbody.MoveRotation(newRotation);
        }
        Vector3 movement = direction.normalized * (step.moveSpeed * Time.fixedDeltaTime);
        _rigidbody.MovePosition(_rigidbody.position + movement);

        if (direction.magnitude <= waypointDistance) {
            _currentWaypoint++;
        }
    }
    // Mit LuzzController.StartRoute(); lässt sich nun eine zuvor angelegte Route ausführen
    public void StartRoute(int routeIndex) {
        if (_isRouteRunning) return;
        if (routeIndex < 0 || routeIndex >= routes.Count) return;
        _currentRoute = routeIndex;
        _currentWaypoint = 0;
        _isRouteRunning = true;
    }
    
    private void PlayAnimation(AnimationClip clip) {
        if (_animationGraph.IsValid()) {
            _animationGraph.Destroy();
        }
        AnimationPlayableUtilities.PlayClip(animator, clip, out _animationGraph);
        _currentAnimation = clip;
    }
    // Mit LuzzController.Walk(); usw. können einzelne Animationen wie Posen abgerufen werden.
    public void Close() {
        PlayAnimation(animClose);
    }
    public void Open() {
        PlayAnimation(animOpen);
    }
    public void Enter() {
        PlayAnimation(animEnter);
    }
    public void Rest() {
        PlayAnimation(animRest);
    }
    public void Shoot() {
        PlayAnimation(animShoot);
    }
    public void Walk() {
        PlayAnimation(animWalk);
    }

    private void OnDestroy() {
        if (_animationGraph.IsValid()) {
            _animationGraph.Destroy();
        }
    }
    
    
    // TODO Löschen ist nur fürs Testen gedacht.
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            StartRoute(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            StartRoute(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            Enter();

        if (Keyboard.current.digit4Key.wasPressedThisFrame)
            Rest();

        if (Keyboard.current.digit5Key.wasPressedThisFrame)
            Shoot();

        if (Keyboard.current.digit6Key.wasPressedThisFrame)
            Walk();
        
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
            Open();
        
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
            Close();
    }
}

