using System.Collections.Generic;
using UnityEngine;



//TODO Schreibweise "_private" "[SerializeField] private rezepte" klein und KammelCase 
public class Kochen : MonoBehaviour {
    [SerializeField] private Rezepte rezeptSammlung;                    // Platz für Rezept script
    [SerializeField] private Transform spawnPunkt;                      // Spawn für fertiges Essen
    [SerializeField] private List<GameObject> essenPrefab;              // Prefabs von Spawnbarem
    
    private List<Zutat> _zutatenImTopf = new List<Zutat>();             // Liste Objekte im Topf
    private List<Zutat> _verwendeteZutaten = new List<Zutat>();         // Liste Verwendung für Kochen
    private Rezepte.Rezept _aktivesRezept;                              // Für Kochen aktives Rezept
    private float _timer;                                               // Timer für Abgleich beim Kochen
    private void OnTriggerEnter(Collider other) {                       // Wenn etwas rein fällt
        Debug.Log("TRIGGER ERKANNT: " + other.gameObject.name);         
        Zutat zutat = other.GetComponent<Zutat>();                      // Hohl das Zutaten Script vom Objekt
        Debug.Log("Zutat gefunden: " + (zutat != null));            
        
        if (zutat != null && !_zutatenImTopf.Contains(zutat)) {      // wenns ne Zutat ist und nicht im Topf...
            _zutatenImTopf.Add(zutat);                                  // ... packs in den Topf ...
            
            PruefeRezept();                                             // ... prüfe das Rezept
            Debug.Log("EINGEFÜGT "+ zutat);                             
        }
    }
    private void OnTriggerExit(Collider other) {                        // Wenn etwas raus fällt
        Zutat zutat = other.GetComponent<Zutat>();                      // Hohl info aus Zutat
        
        if (zutat != null) {                                         // Wenn es eine Zutat ist...
            _zutatenImTopf.Remove(zutat);                               // ... entferne sie aus dem Topf ...
            _aktivesRezept = null;                                      // ... entferne aktives Rezept ...
            _timer = 0;                                                 // ... setze den Timer auf null
            
            PruefeRezept();
            Debug.Log("ENTFERNT "+ zutat.zutat);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_aktivesRezept != null) {                                   // Wenn Rezept aktiv ist ...
            _timer += Time.deltaTime;                                   // ... erhöhe den Timer ...
            if (_timer >= _aktivesRezept.kochZeit) {                    // ... wenn Timer fertig ...
                KochenFertig();                                         // ... führe KochenFertig aus
            }
        }
    }

    private void PruefeRezept() {                                       
        _aktivesRezept = null;                                          
        _timer = 0;                                                     
        _verwendeteZutaten.Clear();                                     

        foreach (Rezepte.Rezept rezept in rezeptSammlung.rezepte) {         // für jedes Rezept in Sammlung
            List<Zutat> freieZutaten = new List<Zutat>(_zutatenImTopf);     // kopiere zutaten im Topf in freie Zutaten
            List<Zutat> gefunden = new List<Zutat>();                       // Speichere die Zutaten die zum Rezept passen
            
            bool passt = true;                                              // Bestätige das Rezept und Zutaten passen

            foreach (ZutatenTyp benoetigt in rezept.zutaten) {                                 // Für jede benötigte Zutat 
                Zutat passendeZutat = freieZutaten.Find(z => z.zutat == benoetigt); // Finde die Zutat
                
                if (passendeZutat == null) {         // Wenn keine passende Zutat...
                    passt = false;                      // ... passt auf false ...
                    break;                              // ... hier ENDE
                }
                gefunden.Add(passendeZutat);            // Wenn passende Zutat füge gefunden hinzu
                freieZutaten.Remove(passendeZutat);     // und entferne freie Zutat aus der Prüfliste
            }
            if (passt) {                                            // Wenn es passt ...
                _aktivesRezept = rezept;                            
                _verwendeteZutaten = gefunden;
                Debug.Log("REZEPT GEFUNDEN " + rezept.ergebnis);
                return;
            }
        }
    }

    private void KochenFertig() {                           
        Rezepte.Rezept fertigesRezept = _aktivesRezept;
        _aktivesRezept = null;
        _timer = 0;

        foreach (Zutat zutat in _verwendeteZutaten) {       // Für jede Zutat in verwendete Zutaten ...
            _zutatenImTopf.Remove(zutat);                   // ... entferne zutat aus dem Topf ...
            Destroy(zutat.gameObject);                      // ... zerstöre das Zutaten Objekt ...
        }
        _verwendeteZutaten.Clear();                         // ... leere die verwendete Zutat
        
        foreach (GameObject prefab in essenPrefab) {                                                    // Für jedes Objekt im essenPrefab ...
            Essen essen = prefab.GetComponent<Essen>();                                                 // ... hohl dir die Informationen aus Essen
            if (essen != null && essen.essen == fertigesRezept.ergebnis) {                           // Wenn das prefab ein Essen ist und egebenis eines Rezepts ...
                GameObject neuesEssen = Instantiate(prefab, spawnPunkt.position, spawnPunkt.rotation);  // ... erzeuge das Objekt am SpawnPunkt ...
                Rigidbody rigid = neuesEssen.GetComponent<Rigidbody>();                                 // ... hohl dir den Rigidbody des neuen Essens

                if (rigid != null) {                                             // Wenn es einen Body hat ...
                    Vector3 richtung =new Vector3(                                  // ... gib ihm ne richtung ...
                        Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));          // ... gib ihr zufallswerte ...
                    rigid.AddForce(richtung.normalized * 8f, ForceMode.Impulse);    // ... schleuder es in die richtung!
                }

                Debug.Log("GEKOCHT " + fertigesRezept.ergebnis);    
                PruefeRezept();                                     // Führe Prüfung aus
                return;                                             // Beende
            }
        }
    }
}
