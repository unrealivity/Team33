using System;
using UnityEngine;

public class UIController : MonoBehaviour
{
    private void Awake()
    {
        GameController.FoodAddedToBacklog += AddToToDoList;
    }

    private void AddToToDoList(FoodType foodType)
    {
        
    }

}
