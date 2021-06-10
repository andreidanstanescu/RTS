using UnityEngine;
using RTS;
 
public class LoadMenu : MonoBehaviour {
 
    public GUISkin mainSkin, selectionSkin;
 
    void Start() {
        Activate();
    }
     
    void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) CancelLoad();
    }
     
    void OnGUI() {
        if(SelectionList.MouseDoubleClick()) StartLoad();
         
        GUI.skin = mainSkin;
        float menuHeight = GetMenuHeight();
        float groupLeft = Screen.width / 2 - GameService.MenuWidth / 2;
        float groupTop = Screen.height / 2 - menuHeight / 2;
        Rect groupRect = new Rect(groupLeft, groupTop, GameService.MenuWidth, menuHeight);
        GUI.BeginGroup(groupRect);
        //background box
        GUI.Box(new Rect(0, 0, GameService.MenuWidth, menuHeight), "");
        //menu buttons
        float leftPos = GameService.Padding;
        float topPos = menuHeight - GameService.Padding - GameService.ButtonHeight;
        if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), "Load Game")) {
            StartLoad();
        }
        leftPos += GameService.ButtonWidth + GameService.Padding;
        if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), "Cancel")) {
            CancelLoad();
        }
        GUI.EndGroup();
         
        //selection list, needs to be called outside of the group for the menu
        float selectionLeft = groupRect.x + GameService.Padding;
        float selectionTop = groupRect.y + GameService.Padding;
        float selectionWidth = groupRect.width - 2 * GameService.Padding;
        float selectionHeight = groupRect.height - GetMenuItemsHeight() - GameService.Padding;
        SelectionList.Draw(selectionLeft,selectionTop,selectionWidth,selectionHeight,selectionSkin);
    }
     
    private float GetMenuHeight() {
        return 250 + GetMenuItemsHeight();
    }
     
    private float GetMenuItemsHeight() {
        return GameService.ButtonHeight + 2 * GameService.Padding;
    }
     
    public void Activate() {
        SelectionList.LoadEntries(PlayerManager.GetSavedGames());
    }
     
    private void StartLoad() {
        string newLevel = SelectionList.GetCurrentEntry();
        if(newLevel!="") {
            GameService.LevelName = newLevel;
            if(Application.loadedLevelName != "BlankMap1") Application.LoadLevel("BlankMap1");
            else if(Application.loadedLevelName != "BlankMap2") Application.LoadLevel("BlankMap2");
            //makes sure that the loaded level runs at normal speed
            Time.timeScale = 1.0f;
        }
    }
     
    private void CancelLoad() {
        GetComponent< LoadMenu >().enabled = false;
        PauseMenu pause = GetComponent< PauseMenu >();
        if(pause) pause.enabled = true;
        else {
            MaineMenu main = GetComponent< MaineMenu >();
            if(main) main.enabled = true;
        }
    }
 
}