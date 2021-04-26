using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class World : MonoBehaviour
{
    public string name;
    public Texture2D image;
    public int cost, sellValue, hitPoints, maxHitPoints;

    protected Player player;
    protected string[] actions = {};
    protected bool currentlySelected = false;

    // protected virtual = cuvant cheie pentru mostenire

    protected virtual void Awake() {
    
    }
    
    protected virtual void Start () {
        player = transform.root.GetComponentInChildren< Player >();
    }
    
    protected virtual void Update () {
    
    }
    
    protected virtual void OnGUI() {
    }

    public void SetSelection(bool selected) {
        currentlySelected = selected;
    }

    public string[] GetActions() {
        return actions;
    }
    
    public virtual void PerformAction(string actionToPerform) {
        //it is up to children with specific actions to determine what to do with each of those actions
    }


    //obiectul deja selectat de player se va duce undeva
    public virtual void SelectedDo(GameObject destObject, Vector3 destPoint, Player parent){
        if(currentlySelected && destPoint != GameService.OutOfBounds && destObject.name != "Ground"){
            World worldObject = destObject.transform.root.GetComponent< World >();
            if(worldObject)
                iCanChangeTheWorld(worldObject, parent);
        }
    }

    public virtual void iCanChangeTheWorld(World worldObject, Player parent){
        SetSelection(false);
        //parent.SelectedObject.SetSelection(false);
        parent.SelectedObject = worldObject;
        worldObject.SetSelection(true);
    }

}
