using System.Collections;
using UnityEngine;

//creez noul namespace pe care il voi folosi de peste tot din joc
namespace RTS {
    //un fel de Singleton Pattern
    public static class GameService {
        //orice membru si metoda va trebui sa fie statica acum

        public static float senzitivity = 2;

        //dimensiunea de unde incepe camera sa se miste
        //de la marginea ecranului

        //CONSTANTE
        public static int SCROLL_DIM { get { return 10; }}
        public static int ROTATE_SPEED { get { return 100; }}
        public static int ROTATE_DIM { get { return 15; }}

        public static int MAX_FOV { get {return 125;} }
        public static int MIN_FOV { get {return 2; } }
        public static int MAX_ORTOGRAPHIC_SIZE { get {return 20;} }
        public static int MIN_ORTOGRAPHIC_SIZE { get {return 1;} }
        public static int DELTA_FOV {get {return 2;}}
        public static float DELTA_ORTOGRAPHIC_SIZE { get {return 0.5f; } }
        public static float MIN_CAMERA_HEIGHT { get { return 10; } }
        public static float MAX_CAMERA_HEIGHT { get { return 40; } }

        //END_CONSTANTE


        public static float GetSenzitivity() {
            return senzitivity;
        }

        public static void SetSenzitivity(float korean) {
            senzitivity = korean;
        }

        

    }
}
