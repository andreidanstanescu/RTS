using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour
{
    public string battletag;
    public bool is_player;
    public HUD hud;
    public World SelectedObject { get; set;}
    //public int mana, abilityPower;
    public Dictionary <string, int> resurse;

    // Start is called before the first frame update
    void Start()
    {
        hud = transform.root.GetComponentInChildren< HUD >();
        resurse = new Dictionary<string, int>();
        resurse.Add("mana", 100);
        resurse.Add("AP", 0);
        resurse.Add("AD", 100);
        resurse.Add("max mana", 3000);
        resurse.Add("max AP", 500);
        resurse.Add("max AD", 1000);
        /*Debug.Log("add flag to player");
        Flag f = GetComponentInChildren< Flag >();
        GameObject newUnit = (GameObject)Instantiate(GameService.extractVehicle(unitName), spawnPoint, rotation);
        newUnit.transform.parent = player.transform;*/
    }

    public void addResurse(string tip, int val)
    {
        try{
            resurse[tip] += val;
            if(resurse[tip] > resurse["max " + tip])
                resurse[tip] = resurse["max " + tip];
        }
        catch(UnassignedReferenceException e){
            Debug.Log("tip invalid");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(is_player){
            this.hud.updateResources(resurse);
        }
    }

    public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation) {
        Debug.Log("add " + unitName + " to player");
        PrefabVehicle v = GetComponentInChildren< PrefabVehicle >();
        GameObject newUnit = (GameObject)Instantiate(GameService.extractVehicle(unitName), spawnPoint, rotation);
        newUnit.transform.parent = v.transform;
        Vehicle unitObject = newUnit.GetComponent< Vehicle >();
        if(unitObject && spawnPoint != rallyPoint) {
            Vector3 baza;
            baza.x = rallyPoint.x + 4.7f;
            baza.y = rallyPoint.y;
            baza.z = rallyPoint.z + 4.7f;
            unitObject.StartMove(baza);
            //Debug.Log("adaug vehicul");
        }
            
    }
}
