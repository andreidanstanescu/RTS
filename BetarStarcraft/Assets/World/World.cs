using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class World : MonoBehaviour
{
    public string name;
    public Texture2D image;
    public int cost, hitPoints, maxHitPoints;

    protected Player player;
    protected string[] actions = {};
    protected bool currentlySelected = false;
    protected Bounds selectionLimits;
    protected Rect mapArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    protected GUIStyle healthStyle = new GUIStyle();
    protected float healthPercentage = 1.0f;
    private List< Material > oldMaterials = new List< Material >();

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
        actions = new string[GameService.ACTION_LIMITS];
        SetPlayer();
    }
    
    protected virtual void Update () {
    
    }
    
    public void SetPlayer() {
        player = transform.root.GetComponentInChildren< Player >();
    }

    //update de fiecare data cand il selectez
    protected virtual void OnGUI() {
        if(currentlySelected){
            //Debug.Log("selectat obiect din lume");
            DrawWorldObject();
        }
    }

    public virtual void SetSelection(bool selected) {
        currentlySelected = selected;
    }

    public string[] GetActions() {
        return actions;
    }

    public bool IsOwnedBy(Player owner) {
        if(player && player.Equals(owner)) {
            return true;
        } else {
            return false;
        }
    }
    
    public virtual void PerformAction(string actionToPerform) {
        //it is up to children with specific actions to determine what to do with each of those actions
    }


    //obiectul deja selectat de player se va duce undeva
    public virtual void SelectedDo(GameObject destObject, Vector3 destPoint, Player parent){
        if(currentlySelected && destPoint != GameService.OutOfBounds && destObject.name != "Ground"){
            World worldObject = destObject.transform.parent.GetComponent< World >();
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
        //GUI.Box(selectBox, "selectat");
        DrawSelectionBox(selectBox);
        GUI.EndGroup();
    }

    protected virtual void DrawSelectionBox(Rect selectBox) {
        //Debug.Log("deseneaza bara health");
        GUI.Box(selectBox, "");
        CalculateCurrentHealth(0.35f, 0.65f);
        DrawHealthBar(selectBox, "");
    }

    public virtual void SetFlick(GameObject hoverObject) {
        if(player && player.is_player && currentlySelected) {
            if(hoverObject.name != "Ground") {
                GameService.changeCursor("select");
                player.hud.SetCustomCursor();
            }
        }
    }

    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        //Not sure about this change, fostul SetFlick
        if(currentlySelected && hitObject && hitObject.name != "Ground") {
            World worldObject = hitObject.transform.parent.GetComponent< World>();
            if(worldObject) {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) return;
                iCanChangeTheWorld(worldObject, controller);
            }
        }
    }

    public Bounds GetSelectionBounds() {
        return selectionLimits;
    }

    protected virtual void CalculateCurrentHealth(float lowSplit, float highSplit) {
        healthPercentage = (float)hitPoints / (float)maxHitPoints;
        if(healthPercentage > highSplit) healthStyle.normal.background = GameService.HealthyTexture;
        else if(healthPercentage > lowSplit) healthStyle.normal.background = GameService.DamagedTexture;
        else healthStyle.normal.background = GameService.CriticalTexture;
    }

    protected void DrawHealthBar(Rect selectBox, string label) {
        healthStyle.padding.top = -20;
        healthStyle.fontStyle = FontStyle.Bold;
        GUI.Label(new Rect(selectBox.x, selectBox.y - 7, selectBox.width * healthPercentage, 5), label, healthStyle);
    }

    public void SetColliders(bool enabled) {
    Collider[] colliders = GetComponentsInChildren< Collider >();
    foreach(Collider collider in colliders) collider.enabled = enabled;
    }
    
    public void SetTransparentMaterial(Material material, bool storeExistingMaterial) {
        if(storeExistingMaterial) oldMaterials.Clear();
        Renderer[] renderers = GetComponentsInChildren< Renderer >();
        foreach(Renderer renderer in renderers) {
            if(storeExistingMaterial) oldMaterials.Add(renderer.material);
            renderer.material = material;
        }
    }
    
    public void RestoreMaterials() {
        Renderer[] renderers = GetComponentsInChildren< Renderer >();
        if(oldMaterials.Count == renderers.Length) {
            for(int i = 0; i < renderers.Length; i++) {
                renderers[i].material = oldMaterials[i];
            }
        }
    }
    
    public void SetPlayingArea(Rect playingArea) {
        this.mapArea = playingArea;
    }
}
