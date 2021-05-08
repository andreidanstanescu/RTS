using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class HUD : MonoBehaviour
{
    //skinurile afisate pe ecran
    public GUISkin resourceSkin, ordersSkin, selectIcon, mouseSkin;

    public Texture2D currentTexture;
    public Texture2D attackCursor, moveCursor, selectCursor;

    private CursorMode cursorMode = CursorMode.Auto;

    public void UpdateMouse() {
        if(!InMouse() && GameService.tipCursor != "misca")
            Cursor.visible = true;
        else{
            Cursor.visible = false;
            GUI.skin = mouseSkin;
            GUI.BeginGroup(new Rect(0, 0, Screen.width - ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
            Vector3 mousePlacement = Input.mousePosition;
            SetCustomCursor();
            //Debug.Log(Input.mousePosition.x);
            float leftLabel = Input.mousePosition.x;
            float topLabel = Screen.height - Input.mousePosition.y;
            Debug.Log(topLabel);
            if(topLabel > 385){
                topLabel = 300;
                //Debug.Log(Screen.height);
            }
            if(leftLabel > 890){
                leftLabel = 700;
                //Debug.Log(Screen.height);
            }
            float rightLabel = currentTexture.width;
            print(leftLabel);
            float bottomLabel = currentTexture.height;
            GUI.Label(new Rect(leftLabel, topLabel, rightLabel, bottomLabel), currentTexture);
            GUI.EndGroup();
        }
    }


    public void SetCustomCursor(){
        switch(GameService.tipCursor){
            case "atac":
                currentTexture = attackCursor;
                break;
            case "misca":
                currentTexture = moveCursor;
                break;
            case "select":
                currentTexture = selectCursor;
                break;
            default:
                break;
        }
    }


    //CONSTANTE
    private const int ORDERS_BAR_WIDTH = 150, RESOURCE_BAR_HEIGHT = 40;

    //este acum componenta fiu al lui Player in ierarhie
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.GetComponent< Player >();
        GameService.setSkin(selectIcon);
        GameService.changeCursor("select");
        //Debug.Log(player.is_player);
    }

    // Update is called once per frame
    void OnGUI()
    {
        if(player.is_player) {
            //Debug.Log("da");
            DrawOrdersBar();
            DrawResourcesBar();
            UpdateMouse();
        }
    }

    private void DrawOrdersBar() {
        GUI.skin = ordersSkin;
        GUI.BeginGroup(new Rect(Screen.width-ORDERS_BAR_WIDTH,RESOURCE_BAR_HEIGHT,ORDERS_BAR_WIDTH,Screen.height-RESOURCE_BAR_HEIGHT));
        //testam daca facem bine selectarea
        string display = "Orders List:\n";
        if(player.SelectedObject){
            display += player.battletag;
            display += " ";
            display += player.SelectedObject.name;
        }
        GUI.Box(new Rect(0,0,ORDERS_BAR_WIDTH,Screen.height-RESOURCE_BAR_HEIGHT),display);
        GUI.EndGroup();
    }

    private void DrawResourcesBar() {
        GUI.skin = resourceSkin;
        GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));

        GUI.Box(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT),"Resurse curente:");
        GUI.EndGroup();
    }

    public bool InMouse() {
        //aflu daca coordonatele curente ale mouse-ului se afla in zona jocului
        float currx = UnityEngine.Input.mousePosition.x;
        float curry = UnityEngine.Input.mousePosition.y;
        return (currx >= 0 && currx <= Screen.width-ORDERS_BAR_WIDTH &&
        curry >= 0 && curry <= Screen.height-RESOURCE_BAR_HEIGHT);
    }
}
