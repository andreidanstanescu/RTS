using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class HUD : MonoBehaviour
{
    //CONSTANTE
    private const int ORDERS_BAR_WIDTH = 170, RESOURCE_BAR_HEIGHT = 40;
    private const int ICON_WIDTH = 32, ICON_HEIGHT = 32, TEXT_WIDTH = 100, TEXT_HEIGHT = 32;
    private const int BUILD_IMAGE_WIDTH = 64, BUILD_IMAGE_HEIGHT = 64;
    private const int START_ORDERS_WIDTH = 64;
    private const int BUILD_IMAGE_PADDING = 2;
    private const int ACTION_IMAGE_WIDTH = 0, ACTION_IMAGE_HEIGHT = 1;
    private const int BUTTON_SPACING = 7;
    private const int SCROLL_BAR_WIDTH = 22;
    private const int SELECTION_NAME_HEIGHT = 15;
    private int buildAreaHeight = 0;
    //skinurile afisate pe ecran
    public GUISkin resourceSkin, ordersSkin, selectIcon, mouseSkin;

    public Dictionary<string, int> resurse;
    public Dictionary<string, Texture2D> resourceTextures;

    public Texture2D currentTexture;
    public Texture2D attackCursor, moveCursor, selectCursor;
    public Texture2D manaTexture, APTexture, ADTexture;
    public Texture2D buttonHover, buttonClick;
    public Texture2D buildFrame, buildMask;
    public Texture2D smallButtonHover, smallButtonClick;

    private CursorMode cursorMode = CursorMode.Auto;

    private World lastSelection;
    private float sliderValue;

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
            //Debug.Log(topLabel);
            if(topLabel > 385){
                topLabel = 300;
                //Debug.Log(Screen.height);
            }
            if(leftLabel > 890){
                leftLabel = 700;
                //Debug.Log(Screen.height);
            }
            float rightLabel = currentTexture.width;
            //print(leftLabel);
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


    //este acum componenta fiu al lui Player in ierarhie
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = transform.root.GetComponent< Player >();
        GameService.setSkin(selectIcon);
        GameService.changeCursor("select");
        resurse = new Dictionary<string, int>();
        resourceTextures = new Dictionary<string, Texture2D>();
        resurse.Add("mana", 100);
        resurse.Add("AP", 0);
        resurse.Add("AD", 100);
        resurse.Add("max mana", 1000);
        resurse.Add("max AP", 500);
        resurse.Add("max AD", 1000);
        //Debug.Log(player.is_player);
        getResourceTextures();
        buildAreaHeight = Screen.height - RESOURCE_BAR_HEIGHT - SELECTION_NAME_HEIGHT - 2 * BUTTON_SPACING;
    }

    void getResourceTextures()
    {
        foreach(string r in resurse.Keys){
            switch(r){
            case "mana":
                resourceTextures.Add("mana", manaTexture);
                break;
            case "AP":
                resourceTextures.Add("AP", APTexture);
                break;
            case "AD":
                resourceTextures.Add("AD", ADTexture);
                break;
            default:
                break;
            }
        }
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
        GUI.BeginGroup(new Rect(Screen.width - ORDERS_BAR_WIDTH - BUILD_IMAGE_WIDTH, RESOURCE_BAR_HEIGHT, ORDERS_BAR_WIDTH + BUILD_IMAGE_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT));
        //testam daca facem bine selectarea
        string display = "Orders List:\n", displaySelected = "";
        //GUI.Box(new Rect(0,0,ORDERS_BAR_WIDTH,Screen.height-RESOURCE_BAR_HEIGHT),display);
        GUI.Box(new Rect(START_ORDERS_WIDTH, 0, ORDERS_BAR_WIDTH, Screen.height - RESOURCE_BAR_HEIGHT),display);
        if(player.SelectedObject){
            display += player.battletag;
            display += " ";
            display += player.SelectedObject.name;
            displaySelected = player.SelectedObject.name;
            if(player.SelectedObject.IsOwnedBy(player)) {
                //reset slider value if the selected object has changed
                if(lastSelection && lastSelection != player.SelectedObject) 
                    sliderValue = 0.0f;
                DrawActions(player.SelectedObject.GetActions());
                //store the current selection
                lastSelection = player.SelectedObject;
                Building selectedBuilding = lastSelection.GetComponent< Building >();
                if(selectedBuilding) {
                    DrawStandardBuildingOptions(selectedBuilding);
                    DrawBuildQueue(selectedBuilding.getBuildQueueValues(), selectedBuilding.getBuildPercentage());
                }
            }
        }
        
        if(!display.Equals("Orders List:\n")) {
            int leftPos = BUILD_IMAGE_WIDTH + SCROLL_BAR_WIDTH / 2;
            int topPos = buildAreaHeight + BUTTON_SPACING;
            GUI.Label(new Rect(leftPos, topPos, ORDERS_BAR_WIDTH, SELECTION_NAME_HEIGHT), player.SelectedObject.name);
        }
        //GUI.BeginGroup(new Rect(Screen.width-ORDERS_BAR_WIDTH,RESOURCE_BAR_HEIGHT,ORDERS_BAR_WIDTH,Screen.height-RESOURCE_BAR_HEIGHT));
        GUI.EndGroup();
    }

    private void DrawBuildQueue(string[] buildQueue, float buildPercentage) {
        for(int i = 0; i < buildQueue.Length; i++) {
            float topPos = i * BUILD_IMAGE_HEIGHT + (i+1) * BUILD_IMAGE_PADDING;
            Rect buildPos = new Rect(BUILD_IMAGE_PADDING, topPos, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
            GUI.DrawTexture(buildPos, GameService.extractImage(buildQueue[i]));
            GUI.DrawTexture(buildPos, buildFrame);
            //GUI.DrawTexture(buildPos, buildMask);
            topPos -= BUILD_IMAGE_PADDING;
            float width = BUILD_IMAGE_WIDTH + BUILD_IMAGE_PADDING;
            float height = BUILD_IMAGE_HEIGHT + BUILD_IMAGE_PADDING;
            if(i==0) {
                //shrink the build mask on the item currently being built to give an idea of progress
                topPos += height * buildPercentage;
                height *= (1 - buildPercentage);
            }
            GUI.DrawTexture(new Rect(BUILD_IMAGE_PADDING, topPos, width, height), buildMask);
        }
    }

    private void DrawResourceIcon(string tip, int iconLeft, int textLeft, int topPos) {
        Texture2D icon = resourceTextures[tip];
        string text = resurse[tip].ToString() + "/" + resurse["max " + tip].ToString();
        GUI.DrawTexture(new Rect(iconLeft, topPos, ICON_WIDTH, ICON_HEIGHT), icon);
        GUI.Label (new Rect(textLeft, topPos, TEXT_WIDTH, TEXT_HEIGHT), text);
    }

    private void DrawResourcesBar() {
        GUI.skin = resourceSkin;
        GUI.Box(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT),"Resurse curente:");
        GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));
        int topPos = 4, iconLeft = 4, textLeft = 20;
        DrawResourceIcon("mana", iconLeft, textLeft, topPos);
        iconLeft += TEXT_WIDTH;
        textLeft += TEXT_WIDTH;
        DrawResourceIcon("AP", iconLeft, textLeft, topPos);
        iconLeft += TEXT_WIDTH;
        textLeft += TEXT_WIDTH;
        DrawResourceIcon("AD", iconLeft, textLeft, topPos);
        GUI.EndGroup();
    }

    private int MaxNumRows(int areaHeight) {
        return areaHeight / BUILD_IMAGE_HEIGHT;
    }
    
    private Rect GetButtonPos(int row, int column) {
        int left = SCROLL_BAR_WIDTH + column * BUILD_IMAGE_WIDTH;
        float top = row * BUILD_IMAGE_HEIGHT - sliderValue * BUILD_IMAGE_HEIGHT;
        return new Rect(left, top, BUILD_IMAGE_WIDTH, BUILD_IMAGE_HEIGHT);
    }
    
    private void DrawSlider(int groupHeight, float numRows) {
        //slider goes from 0 to the number of rows that do not fit on screen
        sliderValue = GUI.VerticalSlider(GetScrollPos(groupHeight), sliderValue, 0.0f, numRows - MaxNumRows(groupHeight));
    }

    private Rect GetScrollPos(int groupHeight) {
        return new Rect(BUTTON_SPACING, BUTTON_SPACING, SCROLL_BAR_WIDTH, groupHeight - 2 * BUTTON_SPACING);
    }

    private void DrawActions(string[] actions) {
        /*GUIStyle buttons = new GUIStyle();
        buttons.hover.background = buttonHover;
        buttons.active.background = buttonClick;
        GUI.skin.button = buttons;*/
        int numActions = actions.Length;
        //define the area to draw the actions inside
        GUI.BeginGroup(new Rect(BUILD_IMAGE_WIDTH, 0, ORDERS_BAR_WIDTH, buildAreaHeight));
        //draw scroll bar for the list of actions if need be
        if(numActions >= MaxNumRows(buildAreaHeight)) 
            DrawSlider(buildAreaHeight, numActions / 2.0f);
        //display possible actions as buttons and handle the button click for each
        for(int i = 0; i < numActions; i++) {
            int column = i % 2 + ACTION_IMAGE_WIDTH;
            int row = i / 2 + ACTION_IMAGE_HEIGHT;
            Rect pos = GetButtonPos(row, column);
            Texture2D action = GameService.extractImage(actions[i]);
            if(action) {
                //create the button and handle the click of that button
                if(GUI.Button(pos, action)) {
                    if(player.SelectedObject) 
                        player.SelectedObject.PerformAction(actions[i]);
                }
            }
        }
        GUI.EndGroup();
    }

    private void DrawStandardBuildingOptions(Building building){
        GUIStyle buttons = new GUIStyle();
		buttons.hover.background = smallButtonHover;
		buttons.active.background = smallButtonClick;
		GUI.skin.button = buttons;
		int leftPos = BUILD_IMAGE_WIDTH + SCROLL_BAR_WIDTH + BUTTON_SPACING;
		int topPos = buildAreaHeight - BUILD_IMAGE_HEIGHT / 2;
		int width = BUILD_IMAGE_WIDTH / 2;
		int height = BUILD_IMAGE_HEIGHT / 2;
        leftPos += width + BUTTON_SPACING;
        Debug.Log("buton");
        if(GUI.Button(new Rect(leftPos, topPos, width, height), building.sellImage)) {
            building.Sell();
        }

    }

    public bool InMouse() {
        //aflu daca coordonatele curente ale mouse-ului se afla in zona jocului
        float currx = UnityEngine.Input.mousePosition.x;
        float curry = UnityEngine.Input.mousePosition.y;
        return (currx >= 0 && currx <= Screen.width-ORDERS_BAR_WIDTH &&
        curry >= 0 && curry <= Screen.height-RESOURCE_BAR_HEIGHT);
    }

    public void updateResources(Dictionary<string, int> r){
        this.resurse = r;
    }


}
