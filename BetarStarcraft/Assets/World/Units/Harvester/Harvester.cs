using UnityEngine;
using RTS;
 
public class Harvester : Unit {
 
    public float capacity;
 
    private bool harvesting = false, emptying = false;
    private float currentLoad = 0.0f;
    private ResourceType harvestType;
    private Resource resourceDeposit;
 
    protected override void Start () {
        base.Start();
        harvestType = ResourceType.Unknown;
    }
 
    protected override void Update () {
        base.Update();
        if(!rotating && !moving) {
            if(harvesting || emptying) {
                Arms[] arms = GetComponentsInChildren< Arms >();
                foreach(Arms arm in arms) arm.renderer.enabled = true;
                if(harvesting) {
                    Collect();
                    if(currentLoad >= capacity || resourceDeposit.isEmpty()) {
                        currentLoad = Mathf.Floor(currentLoad);
                        harvesting = false;
                        emptying = true;
                        foreach(Arms arm in arms) arm.renderer.enabled = false;
                        StartMove (resourceStore.transform.position, resourceStore.gameObject);
                    }
                } else {
                    Deposit();
                    if(currentLoad <= 0) {
                        emptying = false;
                        foreach(Arms arm in arms) arm.renderer.enabled = false;
                        if(!resourceDeposit.isEmpty()) {
                            harvesting = true;
                            StartMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
                        }
                    }
                }
            }
        }
    }
 
    public override void SetHoverState(GameObject hoverObject) {
        base.SetHoverState(hoverObject);
        if(player && player.human && currentlySelected) {
            if(hoverObject.name != "Ground") {
                Resource resource = hoverObject.transform.parent.GetComponent< Resource >();
                if(resource && !resource.isEmpty()) player.hud.SetCursorState(CursorState.Harvest);
            }
        }
    }
 
    public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.MouseClick(hitObject, hitPoint, controller);
        if(player && player.human) {
            if(hitObject.name != "Ground") {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && !resource.isEmpty()) {
                    if(player.SelectedObject) player.SelectedObject.SetSelection(false, playingArea);
                    SetSelection(true, playingArea);
                    player.SelectedObject = this;
                    StartHarvest(resource);
                }
            } else StopHarvest();
        }
    }
 

    private void StartHarvest(Resource resource) {
        resourceDeposit = resource;
        StartMove(resource.transform.position, resource.gameObject);
        //we can only collect one resource at a time, other resources are lost
        if(harvestType == ResourceType.Unknown || harvestType != resource.GetResourceType()) {
            harvestType = resource.GetResourceType();
            currentLoad = 0.0f;
        }
        harvesting = true;
        emptying = false;
    }

    private void StopHarvest() {
 
    }
}