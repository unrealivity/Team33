using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private void Awake()
    {
        GameController.foodAddedToBacklog += AddToToDoList;
    }

    private void AddToToDoList(FoodType foodType)
    {
        
    }

}
