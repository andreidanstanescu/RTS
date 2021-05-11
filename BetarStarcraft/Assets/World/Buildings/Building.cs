using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Building : World
{
    public float maxBuildProgress = 10;
    protected Queue<string> buildQueue;
    private float currentBuildProgress = 0.0f;
    private Vector3 spawnPoint;


    protected override void Awake() {
        buildQueue = new Queue<string>();
        float spawnX = selectionLimits.center.x + transform.forward.x * selectionLimits.extents.x + transform.forward.x * 10;
        float spawnZ = selectionLimits.center.z + transform.forward.z + selectionLimits.extents.z + transform.forward.z * 10;
        spawnPoint = new Vector3(spawnX, 0.0f, spawnZ);
        base.Awake();
    }
    
    protected override void Start () {
        base.Start();
    }
    
    protected override void Update () {
        ProcessBuildQueue();
        base.Update();
    }
    
    protected override void OnGUI() {
        base.OnGUI();
    }

    public override void SetFlick(GameObject hoverObject) {
        if(player && player.is_player && currentlySelected) {
            if(hoverObject.name != "Ground") {
                GameService.changeCursor("atac");
                player.hud.SetCustomCursor();
            }
        }
    }

    public bool free(GameObject destObject){
        return (destObject.name == "Ground");
    }

    public override void SelectedDo(GameObject destObject, Vector3 destPoint, Player parent){
        base.SelectedDo(destObject, destPoint, parent);
        //mutam cladirea altundeva
        if(currentlySelected && destPoint != GameService.OutOfBounds && free(destObject)){
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
        }
    }

    protected void CreateUnit(string unitName){
        buildQueue.Enqueue(unitName);
    }

    protected void ProcessBuildQueue() {
        if (buildQueue.Count > 0) {
            currentBuildProgress += Time.deltaTime * GameService.BuildSpeed;
            if (currentBuildProgress > maxBuildProgress) {
                if (player) player.AddUnit(buildQueue.Dequeue(), spawnPoint, transform.rotation);
                currentBuildProgress = 0.0f;
            }
        }
    }

    public string[] getBuildQueueValues() {
        string[] values = new string[buildQueue.Count];
        int pos = 0;
        foreach (string unit in buildQueue) values[pos++] = unit;
        return values;
    }

    public float getBuildPercentage() {
        return currentBuildProgress / maxBuildProgress;
    }

}
