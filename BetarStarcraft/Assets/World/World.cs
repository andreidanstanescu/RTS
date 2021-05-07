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
    protected Bounds selectionLimits;
    protected Rect mapArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    // protected virtual = cuvant cheie pentru mostenire

    //Awake este o functie mostenita din MonoBehaviour
    //si este apelata la construirea obiectului
    protected virtual void Awake() {
        //atunci cand obiectul se instantiaza, vom afla limitele sale
        selectionLimits = GameService.NotInBounds;
        getLimits();
    }
    
    protected virtual void Start () {
        player = transform.root.GetComponentInChildren< Player >();
    }
    
    protected virtual void Update () {
    
    }
    
    //update de fiecare data cand il selectez
    protected virtual void OnGUI() {
        if(currentlySelected){
            //Debug.Log("selectat obiect din lume");
            DrawWorldObject();
        }
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

    public void getLimits() {
        //il initializez cu centrul obiectului
        selectionLimits = new Bounds(transform.position, Vector3.zero);
        //Encapsulate = Expand the bounds by increasing its size by amount along each side.
        foreach(Renderer r in GetComponentsInChildren< Renderer >()) {
            //adaug toate limitele din copiii obiectului curent
            //practic ii fac conturul
            selectionLimits.Encapsulate(r.bounds);
        }
    }

    protected virtual void DrawWorldObject(){
        mapArea = new Rect(0, GameService.RESOURCE_BAR_HEIGHT, Screen.width - GameService.ORDERS_BAR_WIDTH, Screen.height - GameService.RESOURCE_BAR_HEIGHT);
        GUI.skin = GameService.selectIcon;
        //testam daca facem bine selectarea
        Rect selectBox = GameService.getLimits(selectionLimits, mapArea);
        //Draw the selection box around the currently selected object, within the bounds of the playing area
        GUI.BeginGroup(mapArea);
        GUI.Box(selectBox, "selectat");
        GUI.EndGroup();
    }

}
