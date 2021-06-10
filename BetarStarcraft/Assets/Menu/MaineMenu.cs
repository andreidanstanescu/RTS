using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RTS;

public class MaineMenu : Menu
{
    public void Update(){
        //Cursor.visible = true;
    }
    
    protected override void SetButtons () {
        buttons = new string[] {"New Game", "Load Game", "Change Player", "Quit Game"};
    }
 
    protected override void HandleButton (string text) {
        base.HandleButton(text);
        switch(text) {
            case "New Game": NewGame(); break;
            case "Load Game": LoadGame(); break;
            case "Change Player": ChangePlayer(); break;
            case "Quit Game": ExitGame(); break;
            default: break;
        }
    }
 
    private void NewGame() {
        GameService.MenuOpen = false;
        Application.LoadLevel("Test_units");
        //makes sure that the loaded level runs at normal speed
        Time.timeScale = 1.0f;
    }

    void OnLevelWasLoaded() {
        Cursor.visible = true;
        if(PlayerManager.GetPlayerName() == "") {
            //no player yet selected so enable SetPlayerMenu
            GetComponent< MaineMenu >().enabled = false;
            GetComponent< SelectPlayerMenu >().enabled = true;
        } else {
            //player selected so enable MainMenu
            GetComponent< MaineMenu >().enabled = true;
            GetComponent< SelectPlayerMenu >().enabled = false;
        }
    }
    private void ChangePlayer() {
        GetComponent< MaineMenu >().enabled = false;
        GetComponent< SelectPlayerMenu >().enabled = true;
        SelectionList.LoadEntries(PlayerManager.GetPlayerNames());
    }
    protected override void HideCurrentMenu () {
        GetComponent< MaineMenu >().enabled = false;
    }
}
