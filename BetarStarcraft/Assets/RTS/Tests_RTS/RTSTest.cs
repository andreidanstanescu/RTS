using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;



public class RTSTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void RTSTestSimplePasses()
    {

        Assert.AreEqual(GameService.Origin, new Vector3(0f,0f,0f));
    }

    [Test]
    public void RTSTestSimplePasses2()
    {

        Assert.AreEqual(GameService.getSpeedMovement, 8.0f);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator RTSTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}


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

public static float getSpeedMovement { get { return 8; }}

public static int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;

}