using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class Player : MonoBehaviour
{
    public string battletag;
    public bool is_player;
    public HUD hud;
    public World SelectedObject { get; set;}
    //public int mana, abilityPower;
    public Dictionary <string, int> resurse;
    public Material notAllowedMaterial, allowedMaterial;
 
    private Building tempBuilding;
    private Unit tempCreator;
    private bool findingPlacement = false;

    // Start is called before the first frame update
    void Start()
    {
        hud = transform.root.GetComponentInChildren< HUD >();
        resurse = new Dictionary<string, int>();
        resurse.Add("mana", 100);
        resurse.Add("AP", 0);
        resurse.Add("AD", 100);
        resurse.Add("max mana", 3000);
        resurse.Add("max AP", 500);
        resurse.Add("max AD", 1000);
        /*Debug.Log("add flag to player");
        Flag f = GetComponentInChildren< Flag >();
        GameObject newUnit = (GameObject)Instantiate(GameService.extractVehicle(unitName), spawnPoint, rotation);
        newUnit.transform.parent = player.transform;*/
    }

    public void addResurse(string tip, int val)
    {
        try{
            resurse[tip] += val;
            if(resurse[tip] > resurse["max " + tip])
                resurse[tip] = resurse["max " + tip];
        }
        catch(UnassignedReferenceException e){
            Debug.Log("tip invalid");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(is_player){
            this.hud.updateResources(resurse);
        }
        if(findingPlacement) {
            tempBuilding.CalculateBounds();
            if(CanPlaceBuilding()) tempBuilding.SetTransparentMaterial(allowedMaterial, false);
            else tempBuilding.SetTransparentMaterial(notAllowedMaterial, false);
        }
    }

    public void AddUnit(string unitName, Vector3 spawnPoint, Vector3 rallyPoint, Quaternion rotation, Building creator) {
        Debug.Log("add " + unitName + " to player");
        PrefabVehicle v = GetComponentInChildren< PrefabVehicle >();
        GameObject newUnit = (GameObject)Instantiate(GameService.extractVehicle(unitName), spawnPoint, rotation);
        newUnit.transform.parent = v.transform;
        Vehicle unitObject = newUnit.GetComponent< Vehicle >();
        if(unitObject && spawnPoint != rallyPoint) {
            Vector3 baza;
            baza.x = rallyPoint.x + 4.7f;
            baza.y = rallyPoint.y;
            baza.z = rallyPoint.z + 4.7f;
            unitObject.StartMove(baza);
            //Debug.Log("adaug vehicul");
        }
        if(unitObject) {
            unitObject.SetBuilding(creator);
            if(spawnPoint != rallyPoint) 
                unitObject.StartMove(rallyPoint);
        }   
    }
    public void CreateBuilding(string buildingName, Vector3 buildPoint, Unit creator, Rect playingArea) {
        GameObject newBuilding = (GameObject)Instantiate(ResourceManager.GetBuilding(buildingName), buildPoint, new Quaternion());
        tempBuilding = newBuilding.GetComponent< Building >();
        if (tempBuilding) {
            tempCreator = creator;
            findingPlacement = true;
            tempBuilding.SetTransparentMaterial(notAllowedMaterial, true);
            tempBuilding.SetColliders(false);
            tempBuilding.SetPlayingArea(playingArea);
        } else Destroy(newBuilding);
    }
    public bool IsFindingBuildingLocation() {
        return findingPlacement;
    }
    
    public void FindBuildingLocation() {
        Vector3 newLocation = WorkManager.FindHitPoint();
        newLocation.y = 0;
        tempBuilding.transform.position = newLocation;
    }

    public bool CanPlaceBuilding() {
        bool canPlace = true;
    
        Bounds placeBounds = tempBuilding.GetSelectionBounds();

        float cx = placeBounds.center.x;
        float cy = placeBounds.center.y;
        float cz = placeBounds.center.z;

        float ex = placeBounds.extents.x;
        float ey = placeBounds.extents.y;
        float ez = placeBounds.extents.z;
    

        List< Vector3 > corners = new List< Vector3 >();
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz+ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy+ey,cz-ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz+ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz+ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx+ex,cy-ey,cz-ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz+ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy+ey,cz-ez)));
        corners.Add(Camera.mainCamera.WorldToScreenPoint(new Vector3(cx-ex,cy-ey,cz-ez)));
    
        foreach(Vector3 corner in corners) {
            GameObject hitObject = WorkManager.FindHitObject(corner);
            if(hitObject && hitObject.name != "Ground") {
                WorldObject worldObject = hitObject.transform.parent.GetComponent< WorldObject >();
                if(worldObject && placeBounds.Intersects(worldObject.GetSelectionBounds())) canPlace = false;
            }
        }
        return canPlace;
    }

    public void StartConstruction() {
        findingPlacement = false;
        Buildings buildings = GetComponentInChildren< Buildings >();
        if(buildings) tempBuilding.transform.parent = buildings.transform;
        tempBuilding.SetPlayer();
        tempBuilding.SetColliders(true);
        tempCreator.SetBuilding(tempBuilding);
        tempBuilding.StartConstruction();
    }
        public void CancelBuildingPlacement() {
        findingPlacement = false;
        Destroy(tempBuilding.gameObject);
        tempBuilding = null;
        tempCreator = null;
    }
}
