using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    void addResurse(string tip, int val)
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
}
