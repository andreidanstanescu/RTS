using UnityEngine;
using System.Collections.Generic;
using RTS;
 
public class SelectPlayerMenu : MonoBehaviour {
     
    public GUISkin mySkin, selectionSkin;
     
    private string playerName = "NewPlayer";
    public Texture2D[] avatars;
    private int avatarIndex = -1;

    public AudioClip clickSound;
    public float clickVolume = 1.0f;
    
    private AudioElement audioElement;
        
    void Start(){
        PlayerManager.Load();
        if(avatars.Length > 0) avatarIndex = 0;
        PlayerManager.SetAvatarTextures(avatars);
        SelectionList.LoadEntries(PlayerManager.GetPlayerNames());
        if(clickVolume < 0.0f) clickVolume = 0.0f;
        if(clickVolume > 1.0f) clickVolume = 1.0f;
        List< AudioClip > sounds = new List< AudioClip >();
        List< float > volumes = new List< float >();
        sounds.Add(clickSound);
        volumes.Add (clickVolume);
        audioElement = new AudioElement(sounds, volumes, "SelectPlayerMenu", null);
    }

    void OnGUI() {
         
        GUI.skin = mySkin;
         
        float menuHeight = GetMenuHeight();
        float groupLeft = Screen.width / 2 - GameService.MenuWidth / 2;
        float groupTop = Screen.height / 2 - menuHeight / 2;
        Rect groupRect = new Rect(groupLeft, groupTop, GameService.MenuWidth, menuHeight);
         
        GUI.BeginGroup(groupRect);
        //background box
        GUI.Box(new Rect(0, 0, GameService.MenuWidth, menuHeight), "");
        //menu buttons
        float leftPos = GameService.MenuWidth / 2 - GameService.ButtonWidth / 2;
        float topPos = menuHeight - GameService.Padding - GameService.ButtonHeight;

        if(SelectionList.MouseDoubleClick()) {
            playerName = SelectionList.GetCurrentEntry();
            SelectPlayer();
            PlayClick();
        }

        if(GUI.Button(new Rect(leftPos, topPos, GameService.ButtonWidth, GameService.ButtonHeight), "Select")) {
            SelectPlayer();
            PlayClick();
        }
        //text area for player to type new name
        float textTop = menuHeight - 2 * GameService.Padding - GameService.ButtonHeight - GameService.TextHeight;
        float textWidth = GameService.MenuWidth - 2 * GameService.Padding;
        playerName = GUI.TextField(new Rect(GameService.Padding, textTop, textWidth, GameService.TextHeight), playerName, 32);
        SelectionList.SetCurrentEntry(playerName);
        if(avatarIndex >= 0) {
            float avatarLeft = GameService.MenuWidth / 2 - avatars[avatarIndex].width / 2;
            float avatarTop = textTop - GameService.Padding - avatars[avatarIndex].height;
            float avatarWidth = avatars[avatarIndex].width;
            float avatarHeight = avatars[avatarIndex].height;
            GUI.DrawTexture(new Rect(avatarLeft, avatarTop, avatarWidth, avatarHeight), avatars[avatarIndex]);
            float buttonTop = textTop - GameService.Padding - GameService.ButtonHeight;
            float buttonLeft = GameService.Padding;
            if(GUI.Button(new Rect(buttonLeft, buttonTop, GameService.ButtonHeight, GameService.ButtonHeight), "<")) {
                avatarIndex -= 1;
                if(avatarIndex < 0) avatarIndex = avatars.Length - 1;
                PlayClick();
            }
            buttonLeft = GameService.MenuWidth - GameService.Padding - GameService.ButtonHeight;
            if(GUI.Button(new Rect(buttonLeft, buttonTop, GameService.ButtonHeight, GameService.ButtonHeight), ">")) {
                avatarIndex = (avatarIndex+1) % avatars.Length;
                PlayClick();
            }
        }
       
        GUI.EndGroup();

        //selection list, needs to be called outside of the group for the menu
        string prevSelection = SelectionList.GetCurrentEntry();
        float selectionLeft = groupRect.x + GameService.Padding;
        float selectionTop = groupRect.y + GameService.Padding;
        float selectionWidth = groupRect.width - 2 * GameService.Padding;
        float selectionHeight = groupRect.height - GetMenuItemsHeight() - GameService.Padding;
        SelectionList.Draw(selectionLeft, selectionTop, selectionWidth, selectionHeight, selectionSkin);
        string newSelection = SelectionList.GetCurrentEntry();
        //set saveName to be name selected in list if selection has changed
        if(prevSelection != newSelection) {
            playerName = newSelection;
            avatarIndex = PlayerManager.GetAvatar(playerName);
        }
    }
     
   private float GetMenuHeight() {
		return 250 + GetMenuItemsHeight();
	}
	
	private float GetMenuItemsHeight() {
		float avatarHeight = 0;
		if(avatars.Length > 0) avatarHeight = avatars[0].height + 2 * GameService.Padding;
		return avatarHeight + GameService.ButtonHeight + GameService.TextHeight + 3 * GameService.Padding;
	}

    private void SelectPlayer() {
        PlayerManager.SelectPlayer(playerName, avatarIndex);
        GetComponent< SelectPlayerMenu >().enabled = false;
        MaineMenu main = GetComponent< MaineMenu >();
        if(main) main.enabled = true;
    }
    private void PlayClick() {
        if(audioElement != null) audioElement.Play(clickSound);
    }
}