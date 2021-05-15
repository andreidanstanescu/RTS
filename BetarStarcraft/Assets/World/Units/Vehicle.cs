using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Vehicle : World
{

    private Vector3 destination;
    protected bool moving;

    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start() {
        base.Start();
    }
    
    protected override void Update() {
        base.Update();
        if(moving)
            MakeMove();
    }
    
    protected override void OnGUI() {
        base.OnGUI();
    }

    public bool free(GameObject destObject){
        return (destObject.name == "Ground");
    }

    public void StartMove(Vector3 destination){
        this.destination = destination;
        moving = true;
        //Debug.Log("ma misc");
    }

    public void MakeMove(){
        /*bool moving = true;
        while(moving){
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * GameService.getSpeedMovement);
            if(transform.position == destination)
                moving = false;
        }*/
        Debug.Log("ma misc");
        transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * GameService.getSpeedMovement);
        if(transform.position == destination) 
            moving = false;
        base.getLimits();
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
            StartMove(destination);
            /*bool moving = true;
            while(moving){
                player.SelectedObject.transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * GameService.getSpeedMovement);
                if(player.SelectedObject.transform.position == destination)
                    moving = false;
            }
            base.getLimits();*/
        }
    }
}
