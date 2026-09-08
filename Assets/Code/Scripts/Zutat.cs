using UnityEngine;
public enum ZutatenTyp { // Liste aller Zutaten
    Leer,
    Bohnen,
    Zucker,
    Pfirsich,
    GeschnittenerPfirsich
}
public class Zutat : MonoBehaviour
{
    public ZutatenTyp zutat;
}
