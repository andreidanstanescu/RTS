using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using RTS;

public class Building : World
{
    public float maxBuildProgress = 10;
    private float frameTime = 0.01f;
    protected Queue< string > buildQueue;
    private float currentBuildProgress = 0.0f;
    private bool needsBuilding = false;
    private Vector3 spawnPoint;
    private Vector3 initialPoint;
    private float dx = 1.0f;
    public bool created = false;
    protected Vector3 flagPosition;
    public int sellValue = 100;
    public Texture2D sellImage;
    public Texture2D rallyPointImage;
    public AudioClip finishedJobSound;
    public float finishedJobVolume = 1.0f;

    protected override void Awake() {
        base.Awake();
        buildQueue = new Queue<string>();
        float spawnX = selectionLimits.center.x + transform.forward.x * selectionLimits.extents.x + transform.forward.x * 10;
        float spawnZ = selectionLimits.center.z + transform.forward.z + selectionLimits.extents.z + transform.forward.z * 10;
        spawnPoint = new Vector3(spawnX, 0.0f, spawnZ);
        flagPosition = new Vector3(selectionLimits.center.x, 0.0f, selectionLimits.center.z);
    }

    private void UpdateSpawnPoint(){
        spawnPoint.x += dx;
    }
    
    protected override void Start () {
        base.Start();
    }
    
    protected override void Update () {
        base.Update();
        //CreateUnit("Tureta");
        //Debug.Log(buildQueue.Count);
        //Debug.Log(created);
        if(buildQueue.Count > 0)
            ProcessBuildQueue();
    }
    
    protected override void OnGUI() {
        base.OnGUI();
        if(needsBuilding) DrawBuildProgress();
    }

    public override void SetSelection(bool selected) {
        base.SetSelection(selected);
        if(player) {
            Flag flag = player.GetComponentInChildren< Flag >();
            //Debug.Log(selected);
            if(selected) {
                //Debug.Log(player.is_player);
                if(flag && player.is_player && spawnPoint != GameService.OutOfBounds && flagPosition != GameService.OutOfBounds) {
                    
                    Debug.Log(flagPosition.z);
                    //flag.transform.position = Camera.main.WorldToScreenPoint(flagPosition);
                    flag.transform.position = flagPosition;
                    flag.transform.forward = transform.forward;
                    //flag.SetActive(true);
                    flag.Enable();
                }
            } else {
                if(flag && player.is_player) 
                    flag.Disable();
            }
        }
    }

    public override void SetFlick(GameObject hoverObject) {
        if(player && player.is_player && currentlySelected) {
            if(hoverObject.name != "Ground") {
                GameService.changeCursor("atac");
                if(player.hud.GetPreviousCursorState() == "flag") {
                    GameService.changeCursor("flag");
                    //player.hud.SetCustomCursor();
                }
                player.hud.SetCustomCursor();
            }
        }
    }

    public bool free(GameObject destObject){
        return (destObject.name == "Ground");
    }

    public bool hasSpawnPoint() {
        return spawnPoint != GameService.OutOfBounds && flagPosition != GameService.OutOfBounds;
    }


    public void SetRallyPoint(Vector3 position) {
        flagPosition = position;
        if(player && player.is_player && currentlySelected) {
            Flag flag = player.GetComponentInChildren< Flag >();
            if(flag) flag.transform.position = flagPosition;
        }
    }

    public override void SelectedDo(GameObject destObject, Vector3 hitObject, Player player){
        base.SelectedDo(destObject, hitObject, player);
        //mutam cladirea altundeva
        /*if(currentlySelected && destPoint != GameService.OutOfBounds && free(destObject)){
            float x = destPoint.x;
            //makes sure that the unit stays on top of the surface it is on
            float y = destPoint.y + player.SelectedObject.transform.position.y;
            float z = destPoint.z;
            Vector3 destination = new Vector3(x, y, z);
            bool moving = true;
            while(moving){
                player.SelectedObject.transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * GameService.getSpeedMovement);
                if(player.SelectedObject.transform.position == destination)
                    moving = false;
            }
            base.getLimits();
        }*/

        if(player && player.is_player && currentlySelected) {
            if(destObject.name == "Ground") {
                if((GameService.tipCursor == "flag" || player.hud.GetPreviousCursorState() == "flag") && hitObject != GameService.OutOfBounds) {
                    SetRallyPoint(hitObject);
                }
            }
        }

    }

    protected void CreateUnit(string unitName){
        //Debug.Log(buildQueue.Count);
        buildQueue.Enqueue(unitName);
        //Debug.Log(getBuildQueueValues());
        created = true;
    }

    protected void ProcessBuildQueue() {
        //Debug.Log(buildQueue.Count);
        //Debug.Log("produce");
        currentBuildProgress += frameTime * GameService.BuildSpeed;
        if (currentBuildProgress > maxBuildProgress) {
            Debug.Log("produce");
            if (player) {
                if(audioElement != null) audioElement.Play(finishedJobSound);
                player.AddUnit(buildQueue.Dequeue(), spawnPoint, flagPosition, transform.rotation, this);
                UpdateSpawnPoint();
            }
            currentBuildProgress = 0.0f;
        }
    }

    public string[] getBuildQueueValues() {
        string[] values = new string[buildQueue.Count];
        int pos = 0;
        foreach (string unit in buildQueue) 
            values[pos++] = unit;
        return values;
    }

    public float getBuildPercentage() {
        return currentBuildProgress / maxBuildProgress;
    }

    public void Sell() {
        if(player) player.addResurse("mana", sellValue);
        if(currentlySelected) 
            SetSelection(false);
        Destroy(this.gameObject);
    }
    public void StartConstruction() {
        getLimits();
        needsBuilding = true;
        hitPoints = 0;
    }
    private void DrawBuildProgress() {
        GUI.skin = GameService.selectIcon;
        Rect selectBox = GameService.getLimits(selectionLimits, mapArea);
        
        GUI.BeginGroup(mapArea);
        CalculateCurrentHealth(0.5f, 0.99f);
        DrawHealthBar(selectBox, "Building ...");
        GUI.EndGroup();
    }
        public bool UnderConstruction() {
        return needsBuilding;
    }
    
    public void Construct(int amount) {
        hitPoints += amount;
        if(hitPoints >= maxHitPoints) {
            hitPoints = maxHitPoints;
            needsBuilding = false;
            RestoreMaterials();
            SetTeamColor();
        }
    }
    public override void SaveDetails (JsonWriter writer) {
        base.SaveDetails (writer);
        SaveManager.WriteBoolean(writer, "NeedsBuilding", needsBuilding);
        SaveManager.WriteVector(writer, "SpawnPoint", spawnPoint);
        SaveManager.WriteVector(writer, "RallyPoint", flagPosition);
        SaveManager.WriteFloat(writer, "BuildProgress", currentBuildProgress);
        SaveManager.WriteStringArray(writer, "BuildQueue", buildQueue.ToArray());
    }
    protected override void InitialiseAudio () {
        base.InitialiseAudio ();
        if(finishedJobVolume < 0.0f) finishedJobVolume = 0.0f;
        if(finishedJobVolume > 1.0f) finishedJobVolume = 1.0f;
        List< AudioClip > sounds = new List< AudioClip >();
        List< float > volumes = new List< float >();
        sounds.Add(finishedJobSound);
        volumes.Add (finishedJobVolume);
        audioElement.Add(sounds, volumes);
    }
}
