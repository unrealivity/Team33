using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rezepte", menuName = "Cooking/Rezepte")]   // Erstellt das ScriptableObject

public class Rezepte : ScriptableObject {
    [System.Serializable]
    public class Rezept {
        public List<ZutatenTyp> zutaten;    // Liste für eingang zutaten
        public EssensTyp ergebnis;          // ausgang essen
        public float kochZeit;              // Zahlenwert für Kochdauer
    }
    
    public List<Rezept> rezepte;            // Weitere Rezepte
}

