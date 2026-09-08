using UnityEngine;
public enum EssensTyp { // Liste aller Gerichte
    Leer,
    BohnenEintopf,
    GeschnittenerPfirsich,
    PfirsichKompott
   }
public class Essen : MonoBehaviour {
    public EssensTyp essen;
}
