using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


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
        public static int ACTION_LIMITS { get { return 100; }}

        public static Vector3 PU = new Vector3(-1e5f, -1e5f, -1e5f);
        public static Vector3 Origin = new Vector3(0f,0f,0f);

        //Bounds este definit in constructor de 2 vectori: punctul minim si maxim(centrul si o margine)
        public static Bounds BPU = new Bounds(PU, Origin);
        public static Vector3 OutOfBounds { get { return PU; } }
        public static Bounds NotInBounds { get { return BPU; }}

        public static int BuildSpeed { get { return 2; } }

        public static float getSpeedMovement { get { return 5; }}

        public static int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;

        public static Prefabs playObjects = null;

        private static Dictionary< string, Texture2D > resourceHealthBarTextures;
        //END_CONSTANTE

        // Textures for HP bar

        private static Texture2D healthyTexture, damagedTexture, criticalTexture;
        public static Texture2D HealthyTexture { get { return healthyTexture; } }
        public static Texture2D DamagedTexture { get { return damagedTexture; } }
        public static Texture2D CriticalTexture { get { return criticalTexture; } }

        //END_TEXTURES

        public static GUISkin selectIcon;
        public static void setSkin(GUISkin other){
            selectIcon = other;
        }

        public static string tipCursor;
        public static void changeCursor(string other){
            tipCursor = other;
        }

        /*public struct TipCursor{
            public TipCursor(double x, double y)
            {
                X = x;
                Y = y;
            }

            public double X { get; }
            public double Y { get; }

            public override string ToString() => $"({X}, {Y})";

        }*/

        public static float GetSenzitivity() {
            return senzitivity;
        }

        public static void SetSenzitivity(float korean) {
            senzitivity = korean;
        }

        public static void setCurrentObjects(Prefabs other){
            playObjects = other;
        }

        public static GameObject extractBuilding(string name) {
            return playObjects.extractBuilding(name);
        }
        
        public static GameObject extractVehicle(string name) {
            return playObjects.extractVehicle(name);
        }
        
        public static GameObject extractWorldObject(string name) {
            return playObjects.extractWorldObject(name);
        }
        
        public static Texture2D extractImage(string name) {
            //Debug.Log(playObjects.exists);
            return playObjects.extractImage(name);
        }


        public static Rect getLimits(Bounds selectionLimits, Rect mapArea){
            //returneaza exact conturul obiectului, in limita ecranului vizibil
            //TO DO:
            //De cautat in unity API o functie care sa transforme din coordonatele reale in cele de pe ecran(probabil ceva din Camera)

            List<Vector3> totalBounds = new List<Vector3>();

            float raza_x = selectionLimits.center.x + selectionLimits.extents.x;
            float raza_y = selectionLimits.center.y + selectionLimits.extents.y;
            float raza_z = selectionLimits.center.z + selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x - selectionLimits.extents.x;
            raza_y = selectionLimits.center.y + selectionLimits.extents.y;
            raza_z = selectionLimits.center.z + selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x + selectionLimits.extents.x;
            raza_y = selectionLimits.center.y - selectionLimits.extents.y;
            raza_z = selectionLimits.center.z + selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x + selectionLimits.extents.x;
            raza_y = selectionLimits.center.y + selectionLimits.extents.y;
            raza_z = selectionLimits.center.z - selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x - selectionLimits.extents.x;
            raza_y = selectionLimits.center.y - selectionLimits.extents.y;
            raza_z = selectionLimits.center.z + selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x - selectionLimits.extents.x;
            raza_y = selectionLimits.center.y + selectionLimits.extents.y;
            raza_z = selectionLimits.center.z - selectionLimits.extents.z;
            //totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(Math.Max(0,raza_x), raza_y, raza_z)));
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x - selectionLimits.extents.x;
            raza_y = selectionLimits.center.y + selectionLimits.extents.y;
            raza_z = selectionLimits.center.z + selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));
            raza_x = selectionLimits.center.x - selectionLimits.extents.x;
            raza_y = selectionLimits.center.y - selectionLimits.extents.y;
            raza_z = selectionLimits.center.z - selectionLimits.extents.z;
            totalBounds.Add(Camera.main.WorldToScreenPoint(new Vector3(raza_x, raza_y, raza_z)));

            Bounds contur = new Bounds(totalBounds[0], new Vector3(0, 0, 0));
            for(int i=1;i<8;++i)
                contur.Encapsulate(totalBounds[i]);

            float selectBoxTop = mapArea.height - (contur.center.y + contur.extents.y);
            float selectBoxLeft = contur.center.x - contur.extents.x;
            float selectBoxWidth = 2 * contur.extents.x;
            float selectBoxHeight = 2 * contur.extents.y;

            //Debug.Log(selectBoxLeft);
            //Debug.Log(selectBoxTop);
            //Debug.Log(selectBoxWidth);
            //Debug.Log(selectBoxHeight);
                    
            return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
        }

        public static void StoreSelectBoxItems(GUISkin skin, Texture2D healthy, Texture2D damaged, Texture2D critical) {
            selectIcon = skin;
            healthyTexture = healthy;
            damagedTexture = damaged;
            criticalTexture = critical;
        }

        public static void SetResourceHealthBarTextures(Dictionary< string, Texture2D > images) {
            resourceHealthBarTextures = images;
        }

        public static Texture2D GetResourceHealthBar(string resourceType) {
            if(resourceHealthBarTextures != null && resourceHealthBarTextures.ContainsKey(resourceType)) 
                return resourceHealthBarTextures[resourceType];
            return null;
        }
        public static GameObject FindHitObject(Vector3 origin) {
            Ray ray = Camera.main.ScreenPointToRay(origin);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
            return null;
        }

        public static Vector3 FindHitPoint(Vector3 origin) {
            Ray ray = Camera.main.ScreenPointToRay(origin);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) return hit.point;
            return GameService.OutOfBounds;
        }

        public static Vector3 FindHitPoint() {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) 
                return hit.point;
            return GameService.OutOfBounds;
        }

        public static GameObject FindHitObject(){
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) 
                return hit.collider.gameObject;
            return null;
        }
    }
}
