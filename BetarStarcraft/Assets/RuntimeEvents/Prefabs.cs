using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Prefabs : MonoBehaviour
{
    public GameObject[] buildings;
    public GameObject[] vehicles;
    public GameObject[] worldObjects;
    public GameObject player;
    	
    private static bool exists = false;


    /*
    Unity calls Awake only once during the lifetime of the script instance. 
    A script's lifetime lasts until the Scene that contains it is unloaded. 
    If the Scene is loaded again, Unity loads the script instance again, so Awake will be called again.
    */
    void Awake() {
        if(!exists) {
            DontDestroyOnLoad(transform.gameObject);
            GameService.setCurrentObjects(this);
            exists = true;
        } 
        else 
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject extractBuilding(string name) {
        for(int i = 0; i < buildings.Length; i++) {
            Building building = buildings[i].GetComponent(typeof(Building)) as Building;
            if(building != null && building.name == name) 
                return buildings[i];
        }
        return null;
    }

    public GameObject extractVehicle(string name) {
        for(int i = 0; i < vehicles.Length; i++) {
            Vehicle vehicle = vehicles[i].GetComponent< Vehicle >();
            if(vehicle != null && vehicle.name == name) 
                return vehicles[i];
        }
        return null;
    }

    public GameObject extractWorldObject(string name) {
        foreach(GameObject worldObject in worldObjects) {
            //Debug.Log(worldObject.name);
            if(worldObject.name == name) return worldObject;
        }
        return null;
    }

    public Texture2D extractImage(string name){
        //Debug.Log(name);
        for(int i = 0; i < buildings.Length; i++) {
            Building building = buildings[i].GetComponent< Building >();
            if(building != null && building.name == name) 
                return building.image;
        }
        for(int i = 0; i < vehicles.Length; i++) {
            Vehicle vehicle = vehicles[i].GetComponent< Vehicle >();
            //Debug.Log(vehicle.name);
            if(vehicle != null && vehicle.name == name) 
                return vehicle.image;
        }
        return null;
    }
 


}
