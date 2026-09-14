using System;
using UnityEngine;

public class FoodCollector : MonoBehaviour
{
    [SerializeField] private GameObject foodSection;
    [SerializeField] private float maxCollectForce;
    [SerializeField] private float collectForce;
    [SerializeField] private float forceDamper;
    private Food _currentFood;
    private Rigidbody _currentFoodRigidbody;
    private bool collecting;
    [SerializeField] private float bufferDistance;
    public static event Action<FoodType> FoodCollected; 


    private void Awake()
    {
        GameController.FindCookedFood += TryToFindFood;
    }

    private void TryToFindFood(FoodType foodTypeToCollect)
    {
        foreach (var food in foodSection.GetComponentsInChildren<Food>())
        {
            if (food.food != foodTypeToCollect) continue;
            CollectFood(food);
            return;
        }
        Debug.Log("No food found");
    }

    private void CollectFood(Food food)
    {
        _currentFood = food;
        _currentFoodRigidbody = _currentFood.GetComponent<Rigidbody>();
        _currentFoodRigidbody.useGravity = false;
        _currentFoodRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        _currentFoodRigidbody.detectCollisions = false;
        collecting = true;
    }

    private void FixedUpdate()
    {   
        if(!collecting) return;
        Vector3 toTarget = transform.position - _currentFoodRigidbody.position;
        Vector3 velocityError = -_currentFoodRigidbody.linearVelocity;
        
        Vector3 force = (toTarget * collectForce) + (velocityError * forceDamper);
        force = Vector3.ClampMagnitude(force, maxCollectForce);
        
        _currentFoodRigidbody.AddForce(force, ForceMode.Acceleration);

        if ((_currentFoodRigidbody.position - transform.position).sqrMagnitude > bufferDistance) return;
        FoodCollected?.Invoke(_currentFood.food);
        Destroy(_currentFood.gameObject);
        collecting = false;
    }
}
