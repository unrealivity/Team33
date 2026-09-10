using UnityEngine;
using UnityEngine.Serialization;

public enum FoodType { // Liste aller Gerichte
    Empty,
    BeanStew,
    slicedPeach,
    PeachCompote
   }
public class Food : MonoBehaviour {
    public FoodType food;
}
