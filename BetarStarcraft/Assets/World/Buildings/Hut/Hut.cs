using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hut : Building
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        actions[0] = "Tureta";
        actions[1] = "Tureta";
        actions[2] = "Tureta";
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void PerformAction(string actionToPerform) {
        //Debug.Log(actionToPerform);
        base.PerformAction(actionToPerform);
        //Debug.Log(buildQueue.Count);
        CreateUnit(actionToPerform);
        //Debug.Log(buildQueue.Count);
    }
}
