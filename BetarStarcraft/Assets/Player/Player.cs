using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string battletag;
    public bool is_player;
    public HUD hud;
    public World SelectedObject { get; set;}

    // Start is called before the first frame update
    void Start()
    {
        hud = transform.root.GetComponentInChildren< HUD >();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
