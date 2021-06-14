using UnityEngine;
 
public class Refinery : Building {
 

    //Create a rafinery in Unity (I forgot)
    protected override void Start () {
        base.Start();
        actions = new string[] {"Harvester"};
    }
 
    public override void PerformAction(string actionToPerform) {
        base.PerformAction(actionToPerform);
        CreateUnit(actionToPerform);
    }
    
    protected override bool ShouldMakeDecision () {
        return false;
    }
}