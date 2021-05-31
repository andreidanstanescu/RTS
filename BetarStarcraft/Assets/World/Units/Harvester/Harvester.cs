using UnityEngine;
using RTS;
 
public class Harvester : Vehicle {
 
    public float capacity;
 
    private bool harvesting = false, emptying = false;
    private float currentLoad = 0.0f;
    private string harvestType;
    private Resource resourceDeposit;
    public Building resourceStore;
    public float collectionAmount, depositAmount;
    private float currentDeposit = 0.0f;
 
    protected override void Start () {
        base.Start();
        harvestType = "unknown";
    }

    public override void SetBuilding(Building creator) {
        base.SetBuilding(creator);
        resourceStore = creator;
    }

    private void Collect() {
        float collect = collectionAmount * Time.deltaTime;
        if(currentLoad + collect > capacity) 
            collect = capacity - currentLoad;
        resourceDeposit.Remove(collect);
        currentLoad += collect;
    }
    
    private void Deposit() {
        currentDeposit += depositAmount * Time.deltaTime;
        int deposit = Mathf.FloorToInt(currentDeposit);
        if(deposit >= 1) {
            if(deposit > currentLoad) deposit = Mathf.FloorToInt(currentLoad);
            currentDeposit -= deposit;
            currentLoad -= deposit;
            string depositType = harvestType;
            if(harvestType == "Ore") 
                depositType = "mana";
            player.addResurse(depositType, deposit);
        }
    }
 
    protected override void Update () {
        base.Update();
        if(!moving) {
            if(harvesting || emptying) {
                //Debug.Log("apar brate");
                Arms[] arms = GetComponentsInChildren< Arms >();
                foreach(Arms arm in arms) {
                    //arm.renderer.enabled = false;
                    Renderer r = arm.GetComponent< Renderer >();
                    r.enabled = true;
                }
                if(harvesting) {
                    Collect();
                    if(currentLoad >= capacity || resourceDeposit.isEmpty()) {
                        currentLoad = Mathf.Floor(currentLoad);
                        harvesting = false;
                        emptying = true;
                        foreach(Arms arm in arms) {
                            //arm.renderer.enabled = false;
                            Renderer r = arm.GetComponent< Renderer >();
                            r.enabled = false;
                        }
                        StartMove (resourceStore.transform.position, resourceStore.gameObject);
                    }
                } else {
                    Deposit();
                    if(currentLoad <= 0) {
                        emptying = false;
                        foreach(Arms arm in arms) {
                            //arm.renderer.enabled = false;
                            Renderer r = arm.GetComponent< Renderer >();
                            r.enabled = false;
                        }
                        if(!resourceDeposit.isEmpty()) {
                            harvesting = true;
                            StartMove (resourceDeposit.transform.position, resourceDeposit.gameObject);
                        }
                    }
                }
            }
        }
    }
 
    public override void SetFlick(GameObject hoverObject) {
        base.SetFlick(hoverObject);
        if(player && player.is_player && currentlySelected) {
            if(hoverObject.name != "Ground") {
                Resource resource = hoverObject.transform.parent.GetComponent< Resource >();
                if(resource && !resource.isEmpty()) {
                    //player.hud.SetCursorState(CursorState.Harvest);
                    GameService.changeCursor("harvest");
                    player.hud.SetCustomCursor();
                }
            }
        }
    }
 
    public override void SelectedDo(GameObject hitObject, Vector3 hitPoint, Player controller) {
        base.SelectedDo(hitObject, hitPoint, controller);
        if(player && player.is_player) {
            if(hitObject.name != "Ground") {
                Resource resource = hitObject.transform.parent.GetComponent< Resource >();
                if(resource && !resource.isEmpty()) {
                    if(player.SelectedObject) 
                        player.SelectedObject.SetSelection(false);
                    SetSelection(true);
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
        if(harvestType == "unknown" || harvestType != resource.GetResourceType()) {
            harvestType = resource.GetResourceType();
            currentLoad = 0.0f;
        }
        harvesting = true;
        emptying = false;
    }

    private void StopHarvest() {
 
    }

    protected override void DrawSelectionBox (Rect selectBox) {
        base.DrawSelectionBox(selectBox);
        float percentFull = currentLoad / capacity;
        float maxHeight = selectBox.height - 4;
        float height = maxHeight * percentFull;
        float leftPos = selectBox.x + selectBox.width - 7;
        float topPos = selectBox.y + 2 + (maxHeight - height);
        float width = 5;
        Texture2D resourceBar = GameService.GetResourceHealthBar(harvestType);
        if(resourceBar) 
            GUI.DrawTexture(new Rect(leftPos, topPos, width, height), resourceBar);
    }
}