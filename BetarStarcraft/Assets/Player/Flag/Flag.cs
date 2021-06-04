using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public void Enable () {
        Renderer[] renderers = GetComponentsInChildren< Renderer >();
        foreach(Renderer renderer in renderers) 
            renderer.enabled = true;
        //this.SetActive(true);
        //Debug.Log("apare");
    }
 
    public void Disable () {
        Renderer[] renderers = GetComponentsInChildren< Renderer >();
        foreach(Renderer renderer in renderers) 
            renderer.enabled = false;
        Debug.Log("dispare");
    }
}
