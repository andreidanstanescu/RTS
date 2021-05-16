using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : World
{
<<<<<<< Updated upstream
=======

    private Vector3 destination;
    private GameObject destinationTarget;
    protected bool moving;
    

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
=======

    public bool free(GameObject destObject){
        return (destObject.name == "Ground");
    }

    public void StartMove(Vector3 destination){
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
        Vector3 originalExtents = selectionBounds.extents;
        Vector3 normalExtents = originalExtents;
        normalExtents.Normalize();
        float numberOfExtents = originalExtents.x / normalExtents.x;
        int unitShift = Mathf.FloorToInt(numberOfExtents);
    
        WorldObject worldObject = destinationTarget.GetComponent< WorldObject >();

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

        destinationTarget = null
    }
>>>>>>> Stashed changes
}
