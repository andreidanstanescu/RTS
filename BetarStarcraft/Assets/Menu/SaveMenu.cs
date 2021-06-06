using UnityEngine;
using RTS;
 
public class SaveMenu : MonoBehaviour {
     
    public GUISkin mySkin, selectionSkin;
     
    private string saveName = "NewGame";
    private ConfirmDialog confirmDialog = new ConfirmDialog();
     
    void Start () {
        Activate();
    }
     
    void Update () {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(confirmDialog.IsConfirming()) confirmDialog.EndConfirmation();
            else CancelSave();
        }
        if(Input.GetKeyDown(KeyCode.Return) && confirmDialog.IsConfirming()) {
            confirmDialog.EndConfirmation();
            SaveGame();
        }
    }
     
    void OnGUI() {
        if(confirmDialog.IsConfirming()) {
            string message = "\"" + saveName + "\" deja apare. Sigur vrei sa continui?";
            confirmDialog.Show(message, mySkin);
        } else if(confirmDialog.MadeChoice()) {
            if(confirmDialog.ClickedYes()) SaveGame();
            confirmDialog.EndConfirmation();
        } else {
            if(SelectionList.MouseDoubleClick()) {
                saveName = SelectionList.GetCurrentEntry();
                StartSave();
            }
            GUI.skin = mySkin;
            DrawMenu();
            //handle enter being hit when typing in the text field
            if(Event.current.keyCode == KeyCode.Return) StartSave();
        }
    }
     
    public void Activate() {
        if(GameService.LevelName != null && GameService.LevelName != "") saveName = GameService.LevelName;
        SelectionList.LoadEntries(PlayerManager.GetSavedGames());
    }
     
    private void DrawMenu() {
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
        if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), "Save Game")) {
            StartSave();
        }
        leftPos += GameService.ButtonWidth + GameService.Padding;
        if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), "Cancel")) {
            CancelSave();
        }
        //text area for player to type new name
        float textTop = menuHeight - 2 * GameService.Padding - GameService.ButtonHeight - GameService.TextHeight;
        float textWidth = GameService.MenuWidth - 2 * GameService.Padding;
        saveName = GUI.TextField(new Rect(GameService.Padding, textTop, textWidth, GameService.TextHeight), saveName, 60);
        SelectionList.SetCurrentEntry(saveName);
        GUI.EndGroup();
         
        //selection list, needs to be called outside of the group for the menu
        string prevSelection = SelectionList.GetCurrentEntry();
        float selectionLeft = groupRect.x + GameService.Padding;
        float selectionTop = groupRect.y + GameService.Padding;
        float selectionWidth = groupRect.width - 2 * GameService.Padding;
        float selectionHeight = groupRect.height - GetMenuItemsHeight() - GameService.Padding;
        SelectionList.Draw(selectionLeft,selectionTop,selectionWidth,selectionHeight,selectionSkin);
        string newSelection = SelectionList.GetCurrentEntry();
        //set saveName to be name selected in list if selection has changed
        if(prevSelection != newSelection) saveName = newSelection;
    }
     
    private float GetMenuHeight() {
        return 250 + GetMenuItemsHeight();
    }
     
    private float GetMenuItemsHeight() {
        return GameService.ButtonHeight + GameService.TextHeight + 3 * GameService.Padding;
    }
     
    private void StartSave() {
        //prompt for override of name if necessary
        if(SelectionList.Contains(saveName)) confirmDialog.StartConfirmation();
        else SaveGame();
    }

    private void CancelSave() {
        GetComponent< SaveMenu >().enabled = false;
        PauseMenu pause = GetComponent< PauseMenu >();
        if(pause) pause.enabled = true;
    }
     
    private void SaveGame() {
        SaveManager.SaveGame(saveName);
        GameService.LevelName = saveName;
        GetComponent< SaveMenu >().enabled = false;
        PauseMenu pause = GetComponent< PauseMenu >();
        if(pause) pause.enabled = true;
    }
}