using System.Collections;
using UnityEngine;

//creez noul namespace pe care il voi folosi de peste tot din joc
namespace RTS {
    //un fel de Singleton Pattern
    public static class GameService {
        //orice membru si metoda va trebui sa fie statica acum

        public static float senzitivity = 20;

        //dimensiunea de unde incepe camera sa se miste
        //de la marginea ecranului
        public static int ScrollDim { get { return 10; }}
        public static int RotateSpeed { get { return 100; }}
        public static int RotateDim { get { return 15; }}

        public static float GetSenzitivity() {
            return senzitivity;
        }

        public static void SetSenzitivity(float korean) {
            senzitivity = korean;
        }

        public static float MinCameraHeight { get { return 10; } }
        public static float MaxCameraHeight { get { return 40; } }

    }
}
