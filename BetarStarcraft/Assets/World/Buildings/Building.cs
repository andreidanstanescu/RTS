using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Building : World
{
    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start () {
        base.Start();
    }
    
    protected override void Update () {
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

}
