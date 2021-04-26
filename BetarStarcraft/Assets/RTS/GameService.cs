using System.Collections;
using UnityEngine;

//creez noul namespace pe care il voi folosi de peste tot din joc
namespace RTS {
    //Singleton Pattern
    public sealed class GameService {

        private GameService(){

        }

        private static readonly object padlock = new object();  
        private static GameService instance = null;  
        public static GameService getInstance  
        {  
            get  
            {  
                lock (padlock)  
                {  
                    if (instance == null)  
                    {  
                        instance = new GameService();  
                    }  
                    return instance;  
                }  
            }  
        } 

        //orice membru si metoda va trebui sa fie statica acum

        public static float senzitivity = 2;

        //dimensiunea de unde incepe camera sa se miste
        //de la marginea ecranului

        //CONSTANTE
        public static int SCROLL_DIM { get { return 50; }}
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

        public static Vector3 PU = new Vector3(-1e5f, -1e5f, -1e5f);
        public static Vector3 OutOfBounds { get { return PU; } }

        //END_CONSTANTE


        public static float GetSenzitivity() {
            return senzitivity;
        }

        public static void SetSenzitivity(float korean) {
            senzitivity = korean;
        }

        

    }
}
