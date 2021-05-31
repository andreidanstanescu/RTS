using UnityEngine;
using RTS;
 
public class Resource : World {
 
    public float capacity;
 
    protected float amountLeft;
    protected string resourceType;
 
    protected override void Start () {
        base.Start();
        amountLeft = capacity;
        resourceType = "unknown";
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
        healthStyle.normal.background = ResourceManager.GetResourceHealthBar(resourceType);
    }
}