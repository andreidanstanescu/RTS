using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;
 
public class Menu : MonoBehaviour {
 
    public GUISkin mySkin;
    public Texture2D header;
    public AudioClip clickSound;
    public float clickVolume = 1.0f;
 
    private AudioElement audioElement;
 
    protected string[] buttons;
 
    protected virtual void Start () {
        SetButtons();
        if(clickVolume < 0.0f) clickVolume = 0.0f;
        if(clickVolume > 1.0f) clickVolume = 1.0f;
        List< AudioClip > sounds = new List< AudioClip >();
        List< float> volumes = new List< float >();
        sounds.Add(clickSound);
        volumes.Add (clickVolume);
        audioElement = new AudioElement(sounds, volumes, "Menu", null);
    }
 
    protected virtual void OnGUI() {
        DrawMenu();
    }
 
    protected virtual void DrawMenu() {
        //default implementation for a menu consisting of a vertical list of buttons
        GUI.skin = mySkin;
    
        float groupLeft = Screen.width / 2 - GameService.MenuWidth / 2;
        float groupTop = Screen.height / 2 - GameService.PauseMenuHeight / 2;
        GUI.BeginGroup(new Rect(groupLeft, groupTop, GameService.MenuWidth, GameService.PauseMenuHeight + 155));
    
        //background box
        GUI.Box(new Rect(0, 0, GameService.MenuWidth, GameService.PauseMenuHeight + 85), "");
        //header image
        GUI.DrawTexture(new Rect(GameService.Padding, GameService.Padding, GameService.HeaderWidth, GameService.HeaderHeight), header);
    
        //welcome message(dosent work)
        float leftPos = GameService.MenuWidth / 2 - GameService.ButtonWidth / 2;
        float topPos = 2 * GameService.Padding + GameService.HeaderHeight;
        GUI.Label(new Rect(leftPos, topPos, GameService.MenuWidth - 2 * GameService.Padding, GameService.TextHeight), "  Salut, " + PlayerManager.GetPlayerName());

    
        //menu buttons
        leftPos = GameService.MenuWidth / 2 - GameService.ButtonWidth / 2;
        topPos += GameService.TextHeight + GameService.Padding;

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
        if(audioElement != null) audioElement.Play(clickSound);
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
    protected void LoadGame() {
        HideCurrentMenu();
        LoadMenu loadMenu = GetComponent< LoadMenu >();
        if(loadMenu) {
            loadMenu.enabled = true;
            loadMenu.Activate();
        }
    }
    protected virtual void HideCurrentMenu() {
        //a child class needs to set this to hide itself when appropriate
    }
 
    protected void ExitGame() {
        Application.Quit();
    }
}