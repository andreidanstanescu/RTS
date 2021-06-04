using UnityEngine;
using System.Collections;
using RTS;
 
public class Menu : MonoBehaviour {
 
    public GUISkin mySkin;
    public Texture2D header;
 
    protected string[] buttons;
 
    protected virtual void Start () {
        SetButtons();
    }
 
    protected virtual void OnGUI() {
        DrawMenu();
    }
 
    protected virtual void DrawMenu() {
        //default implementation for a menu consisting of a vertical list of buttons
        GUI.skin = mySkin;
    
        float groupLeft = Screen.width / 2 - GameService.MenuWidth / 2;
        float groupTop = Screen.height / 2 - GameService.PauseMenuHeight / 2;
        GUI.BeginGroup(new Rect(groupLeft, groupTop, GameService.MenuWidth, GameService.PauseMenuHeight));
    
        //background box
        GUI.Box(new Rect(0, 0, GameService.MenuWidth, GameService.PauseMenuHeight), "");
        //header image
        GUI.DrawTexture(new Rect(GameService.Padding, GameService.Padding, GameService.HeaderWidth, GameService.HeaderHeight), header);
    
        //welcome message(dosent work)
        //float leftPos = GameService.MenuWidth / 2 - GameService.ButtonWidth / 2;
        //float topPos = 2 * GameService.Padding + header.height;
        //GUI.Label(new Rect(leftPos, topPos, GameService.MenuWidth - 2 * GameService.Padding, GameService.TextHeight), "Salut! " + PlayerManager.GetPlayerName());

    
        //menu buttons
        float leftPos = GameService.MenuWidth / 2 - GameService.ButtonWidth / 2;
        float topPos = 2 * GameService.Padding + GameService.HeaderHeight;

        for(int i = 0; i < buttons.Length; i++) {
            //Debug.Log(buttons[i]);
            if(i > 0) topPos += GameService.ButtonHeight + GameService.Padding;
            if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), buttons[i])) {
                    HandleButton(buttons[i]);
            }
        
        }
    
        GUI.EndGroup();
    }
 
    protected virtual void SetButtons() {
        //a child class needs to set this for buttons to appear
    }
 
    protected virtual void HandleButton(string text) {
        //a child class needs to set this to handle button clicks
    }
 
    protected virtual float GetMenuHeight() {
        float messageHeight = GameService.TextHeight + GameService.Padding;
        float buttonHeight = 0;
        if(buttons != null) buttonHeight = buttons.Length * GameService.ButtonHeight;
        float paddingHeight = 2 * GameService.Padding;
        if(buttons != null) paddingHeight += buttons.Length * GameService.Padding;
        return GameService.HeaderHeight + buttonHeight + paddingHeight + messageHeight;
    }
 
    protected void ExitGame() {
        Application.Quit();
    }
}