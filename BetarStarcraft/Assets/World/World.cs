using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using RTS;

public class World : MonoBehaviour
{
    //public string name;
    public string objectName = "WorldObject";
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
    public int ObjectId { get; set; }
    protected bool loadedSavedValues = false;
    private int loadedTargetId = -1;
    public AudioClip attackSound, selectSound, useWeaponSound;
    public float attackVolume = 1.0f, selectVolume = 1.0f, useWeaponVolume = 1.0f;
    
    protected AudioElement audioElement;

    //attack stuff
    protected World target = null;
    protected bool attacking = false;
    public float weaponRange = 10.0f;
    protected bool movingIntoPosition = false;
    protected bool aiming = false;
    public float weaponRechargeTime = 1.0f;
    private float currentWeaponChargeTime;
    public float weaponAimSpeed = 1.0f;
    //

    // protected virtual = cuvant cheie pentru mostenire

    //Awake este o functie mostenita din MonoBehaviour
    //si este apelata la construirea obiectului
    protected virtual void Awake() {
        //atunci cand obiectul se instantiaza, vom afla limitele sale
        selectionLimits = GameService.NotInBounds;
        getLimits();
    }
    
    protected virtual void Start () {
        SetPlayer();
        if(player) {
            SetTeamColor();
            if(loadedSavedValues && loadedTargetId >= 0) target = player.GetObjectForId(loadedTargetId);
        }
        InitialiseAudio();
    }

    protected void SetTeamColor() {
        TeamColor[] teamColors = GetComponentsInChildren< TeamColor >();
        foreach(TeamColor teamColor in teamColors) {
            Renderer r = teamColor.GetComponent< Renderer >();
            r.material.color = player.teamColor;
        }
    }
    
    protected virtual void Update () {
        currentWeaponChargeTime += Time.deltaTime;
        if(attacking && !movingIntoPosition && !aiming)
            PerformAttack();
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
        if(audioElement != null) audioElement.Play(selectSound);
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
    public virtual void SelectedDo(GameObject hitObject, Vector3 hitPoint, Player controller){
                //Not sure about this change, fostul SetFlick - Sirbu
        //Debug.Log("ceva");
        if(currentlySelected && hitObject && hitObject.name != "Ground") {
            /*World worldObject = hitObject.transform.parent.GetComponent< World>();
            if(worldObject) {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) return;
                iCanChangeTheWorld(worldObject, controller);
            }*/
             World worldObject = hitObject.transform.parent.GetComponent< World >();
            //clicked on another selectable object
            if(worldObject) {
                //Debug.Log(worldObject.name);
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) 
                    return;
                Player owner = hitObject.transform.root.GetComponent< Player >();
                if(owner) { //the object is controlled by a player
                    //Debug.Log(owner.battletag);
                    if(player && player.is_player) { //this object is controlled by a human player
                        //start attack if object is not owned by the same player and this object can attack, else select
                        if(player.battletag != owner.battletag && CanAttack()) {
                            Debug.Log("ataca cladirea");
                            BeginAttack(worldObject);
                        }
                        //daca nu, atunci player-ul din parametru selecteaza normal obiectul acesta in locul celui curent
                        else iCanChangeTheWorld(worldObject, controller);
                    } else iCanChangeTheWorld(worldObject, controller);
                } else iCanChangeTheWorld(worldObject, controller);
            }
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
                
                Player owner = hoverObject.transform.root.GetComponent< Player >();
                Vehicle unit = hoverObject.transform.parent.GetComponent< Vehicle >();
                Building building = hoverObject.transform.parent.GetComponent< Building >();
                //Debug.Log(CanAttack());
                if(owner) { //the object is owned by a player
                    if(owner.battletag == player.battletag) {
                        GameService.changeCursor("select");
                        player.hud.SetCustomCursor();
                    }
                    else if(CanAttack()) {
                        GameService.changeCursor("atac");
                        player.hud.SetCustomCursor();
                    }
                    else {
                        GameService.changeCursor("select");
                        player.hud.SetCustomCursor();
                    }
                } else if( (unit || building) && CanAttack()) {
                    GameService.changeCursor("atac");
                    player.hud.SetCustomCursor();
                }
                else {
                    GameService.changeCursor("select");
                    player.hud.SetCustomCursor();
                }
            }
        }
    }

    public virtual bool CanAttack() {
        //default behaviour needs to be overidden by children
        return false;
    }

    //respecta MVC
    public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        //Not sure about this change, fostul SetFlick - Sirbu
        //Debug.Log("ceva");
        if(currentlySelected && hitObject && hitObject.name != "Ground") {
            /*World worldObject = hitObject.transform.parent.GetComponent< World>();
            if(worldObject) {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) return;
                iCanChangeTheWorld(worldObject, controller);
            }*/
             World worldObject = hitObject.transform.parent.GetComponent< World >();
            //clicked on another selectable object
            if(worldObject) {
                Debug.Log(worldObject.name);
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) 
                    return;
                Player owner = hitObject.transform.root.GetComponent< Player >();
                if(owner) { //the object is controlled by a player
                    Debug.Log(owner.battletag);
                    if(player && player.is_player) { //this object is controlled by a human player
                        //start attack if object is not owned by the same player and this object can attack, else select
                        if(player.battletag != owner.battletag && CanAttack()) {
                            Debug.Log("ataca cladirea");
                            BeginAttack(worldObject);
                        }
                        //daca nu, atunci player-ul din parametru selecteaza normal obiectul acesta in locul celui curent
                        else iCanChangeTheWorld(worldObject, controller);
                    } else iCanChangeTheWorld(worldObject, controller);
                } else iCanChangeTheWorld(worldObject, controller);
            }
        }
    }

    private bool TargetInRange() {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        if(direction.sqrMagnitude < weaponRange * weaponRange) {
            return true;
        }
        return false;
    }

    private Vector3 FindNearestAttackPosition() {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        float targetDistance = direction.magnitude;
        float distanceToTravel = targetDistance - (0.9f * weaponRange);
        return Vector3.Lerp(transform.position, targetLocation, distanceToTravel / targetDistance);
    }

    private void AdjustPosition() {
        Vehicle self = this as Vehicle;
        //conditia asta ne asigura ca nu pot ataca decat vehiculele(deocamdata, am doar turete)
        if(self) {
            movingIntoPosition = true;
            Vector3 attackPosition = FindNearestAttackPosition();
            self.StartMove(attackPosition);
            attacking = true;
        } else attacking = false;
    }

    protected virtual void BeginAttack(World target) {
        if(audioElement != null) audioElement.Play(attackSound);
        this.target = target;
        if(TargetInRange()) {
            attacking = true;
            PerformAttack();
        } 
        else 
            AdjustPosition();
    }

    private void PerformAttack() {
        if(!target) {
            attacking = false;
            return;
        }
        if(!TargetInRange()) 
            AdjustPosition();
        else if(!TargetInFrontOfWeapon())  
            AimAtTarget();
        else if(ReadyToFire()) 
            UseWeapon();
    }

    private bool TargetInFrontOfWeapon() {
        Vector3 targetLocation = target.transform.position;
        Vector3 direction = targetLocation - transform.position;
        if(direction.normalized == transform.forward.normalized) 
            return true;
        else 
            return false;
    }

    protected virtual void AimAtTarget() {
        aiming = true;
        //this behaviour needs to be specified by a specific object
    }

    private bool ReadyToFire() {
        //Debug.Log(currentWeaponChargeTime);
        if(currentWeaponChargeTime >= weaponRechargeTime) return true;
        return false;
    }

    protected virtual void UseWeapon() {
        if(audioElement != null) audioElement.Play(useWeaponSound);
        currentWeaponChargeTime = 0.0f;
        //this behaviour needs to be specified by a specific object
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

    public void TakeDamage(int damage) {
        hitPoints -= damage;
        if(hitPoints<=0) Destroy(gameObject);
    }

    public virtual void SaveDetails(JsonWriter writer) {
        SaveManager.WriteString(writer, "Type", name);
        SaveManager.WriteString(writer, "Name", objectName);
        SaveManager.WriteInt(writer, "Id", ObjectId);
        SaveManager.WriteVector(writer, "Position", transform.position);
        SaveManager.WriteQuaternion(writer, "Rotation", transform.rotation);
        SaveManager.WriteVector(writer, "Scale", transform.localScale);
        SaveManager.WriteInt(writer, "HitPoints", hitPoints);
        SaveManager.WriteBoolean(writer, "Attacking", attacking);
        SaveManager.WriteBoolean(writer, "MovingIntoPosition", movingIntoPosition);
        SaveManager.WriteBoolean(writer, "Aiming", aiming);
        if(attacking) {
            //only save if attacking so that we do not end up storing massive numbers for no reason
            SaveManager.WriteFloat(writer, "CurrentWeaponChargeTime", currentWeaponChargeTime);
        }
        if(target != null) SaveManager.WriteInt(writer, "TargetId", target.ObjectId);
    }

    public void LoadDetails(JsonTextReader reader) {
        while(reader.Read()) {
            if(reader.Value != null) {
                if(reader.TokenType == JsonToken.PropertyName) {
                    string propertyName = (string)reader.Value;
                    reader.Read();
                    HandleLoadedProperty(reader, propertyName, reader.Value);
                }
            } else if(reader.TokenType == JsonToken.EndObject) {
                selectionLimits = GameService.NotInBounds;
                getLimits();
                loadedSavedValues = true;
                return;
            }
        }
        selectionLimits = GameService.NotInBounds;
        getLimits();
        loadedSavedValues = true;
    }

    protected virtual void HandleLoadedProperty(JsonTextReader reader, string propertyName, object readValue) {
        switch(propertyName) {
            case "Name": objectName = (string)readValue; break;
            case "Id": ObjectId = (int)(System.Int64)readValue; break;
            case "Position": transform.localPosition = LoadManager.LoadVector(reader); break;
            case "Rotation": transform.localRotation = LoadManager.LoadQuaternion(reader); break;
            case "Scale": transform.localScale = LoadManager.LoadVector(reader); break;
            case "HitPoints": hitPoints = (int)(System.Int64)readValue; break;
            case "Attacking": attacking = (bool)readValue; break;
            case "MovingIntoPosition": movingIntoPosition = (bool)readValue; break;
            case "Aiming": aiming = (bool)readValue; break;
            case "CurrentWeaponChargeTime": currentWeaponChargeTime = (float)(double)readValue; break;
            case "TargetId": loadedTargetId = (int)(System.Int64)readValue; break;
            default: break;
        }
    }
    protected virtual void InitialiseAudio() {
        List< AudioClip > sounds = new List< AudioClip >();
        List< float > volumes = new List< float >();
        if(attackVolume < 0.0f) attackVolume = 0.0f;
        if(attackVolume > 1.0f) attackVolume = 1.0f;
        sounds.Add(attackSound);
        volumes.Add(attackVolume);
        if(selectVolume < 0.0f) selectVolume = 0.0f;
        if(selectVolume > 1.0f) selectVolume = 1.0f;
        sounds.Add(selectSound);
        volumes.Add(selectVolume);
        if(useWeaponVolume < 0.0f) useWeaponVolume = 0.0f;
        if(useWeaponVolume > 1.0f) useWeaponVolume = 1.0f;
        sounds.Add(useWeaponSound);
        volumes.Add(useWeaponVolume);
        audioElement = new AudioElement(sounds, volumes, objectName + ObjectId, this.transform);
    }
}
