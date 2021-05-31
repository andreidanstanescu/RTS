using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Vehicle : World
{

    private Vector3 destination;
    private GameObject destinationTarget;
    protected bool moving;
    

    protected override void Awake() {
        base.Awake();
    }
    
    protected override void Start () {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (moving)
            MakeMove();
    }

    protected override void OnGUI() {
        base.OnGUI();
    }

    public virtual void SetBuilding(Building creator) {
    }

    public bool free(GameObject destObject){
        return (destObject.name == "Ground");
    }

    public virtual void StartMove(Vector3 destination){
        this.destination = destination;
        moving = true;
        destinationTarget = null;
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
        if(destinationTarget) CalculateTargetDestination();
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

    public void StartMove(Vector3 destination, GameObject destinationTarget) {
        StartMove(destination);
        this.destinationTarget = destinationTarget;
    }

    private void CalculateTargetDestination() {
        Vector3 originalExtents = selectionLimits.extents;
        Vector3 normalExtents = originalExtents;
        normalExtents.Normalize();
        float numberOfExtents = originalExtents.x / normalExtents.x;
        int unitShift = Mathf.FloorToInt(numberOfExtents);
    
        World worldObject = destinationTarget.GetComponent< World >();

        if(worldObject) 
            originalExtents = worldObject.GetSelectionBounds().extents;
        else 
            originalExtents = new Vector3(0.0f, 0.0f, 0.0f);

        normalExtents = originalExtents;
        normalExtents.Normalize();
        numberOfExtents = originalExtents.x / normalExtents.x;
        int targetShift = Mathf.FloorToInt(numberOfExtents);
    
        
        int shiftAmount = targetShift + unitShift;

        Vector3 origin = transform.position;
        Vector3 direction = new Vector3(destination.x - origin.x, 0.0f, destination.z - origin.z);
        direction.Normalize();
    

        for(int i = 0; i < shiftAmount; i++) 
            destination -= direction;
        destination.y = destinationTarget.transform.position.y;

        destinationTarget = null;
    }

    public override void SetFlick(GameObject hoverObject) {
        base.SetFlick(hoverObject);
        //only handle input if owned by a human player and currently selected
        if(player && player.is_player && currentlySelected) {
            bool moveHover = false;
            if(hoverObject.name == "Ground") {
                moveHover = true;
            } else {
                Resource resource = hoverObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) 
                    moveHover = true;
            }
            if(moveHover) {
                GameService.changeCursor("misca");
                player.hud.SetCustomCursor();
            }

        }
    }

    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.MouseClick(hitObject, hitPoint, controller);
        //only handle input if owned by a human player and currently selected
        if(player && player.is_player && currentlySelected) {
            bool clickedOnEmptyResource = false;
            if(hitObject.transform.parent) {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && resource.isEmpty()) {
                    clickedOnEmptyResource = true;
                    Debug.Log("resursa goala");
                }
            }
            if((hitObject.name == "Ground" || clickedOnEmptyResource) && hitPoint != GameService.OutOfBounds) {
                float x = hitPoint.x;
                //makes sure that the unit stays on top of the surface it is on
                float y = hitPoint.y + player.SelectedObject.transform.position.y;
                float z = hitPoint.z;
                Vector3 destination = new Vector3(x, y, z);
                StartMove(destination);
            }
        }
    }
}
