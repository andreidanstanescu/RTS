using UnityEngine;
using RTS;
using Newtonsoft.Json;
 
public class Resource : World {
 
    public float capacity;
 
    protected float amountLeft;
    protected string resourceType;
 
    protected override void Start () {
        base.Start();
        resourceType = "Unknown";
        if(loadedSavedValues) return;
        amountLeft = capacity;
    }
 
    public void Remove(float amount) {
        amountLeft -= amount;
        if(amountLeft < 0) amountLeft = 0;
    }
 
    public bool isEmpty() {
        return amountLeft <= 0;
    }
 
    public string GetResourceType() {
        return resourceType;
    }

    protected override void CalculateCurrentHealth (float lowSplit, float highSplit) {
        healthPercentage = amountLeft / capacity;
        healthStyle.normal.background = GameService.GetResourceHealthBar(resourceType);
    }

    public override void SaveDetails (JsonWriter writer) {
        base.SaveDetails (writer);
        SaveManager.WriteFloat(writer, "AmountLeft", amountLeft);
    }
    protected override void HandleLoadedProperty (JsonTextReader reader, string propertyName, object readValue) {
        base.HandleLoadedProperty (reader, propertyName, readValue);
        switch(propertyName) {
            case "AmountLeft": amountLeft = (float)(double)readValue; break;
            default: break;
        }
    }
}