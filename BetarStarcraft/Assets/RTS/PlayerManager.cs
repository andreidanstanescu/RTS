using UnityEngine;
using System.Collections.Generic;
 
namespace RTS {
    public static class PlayerManager {
        private struct PlayerDetails {
        private string name;
        private int avatar;
        public PlayerDetails(string name, int avatar) {
            this.name = name;
            this.avatar = avatar;
        }
        public string Name { get { return name; } }
        public int Avatar {get {return avatar; }}
    }

        private static List< PlayerDetails > players = new List< PlayerDetails >();
        private static PlayerDetails currentPlayer;
        private static Texture2D[] avatars;
        
        public static void SelectPlayer(string name, int avatar) {
            //check player doesnt already exist
            bool playerExists = false;
            foreach(PlayerDetails player in players) {
                if(player.Name == name) {
                    currentPlayer = player;
                    playerExists = true;
                }
            }
            if(!playerExists) {
                PlayerDetails newPlayer = new PlayerDetails(name ,avatar);
                players.Add(newPlayer);
                currentPlayer = newPlayer;
            }
        }
        
        public static string GetPlayerName() {
            return currentPlayer.Name == "" ? "Unknown" : currentPlayer.Name;
        }
        public static void SetAvatarTextures(Texture2D[] avatarTextures) {
            avatars = avatarTextures;
        }
        public static Texture2D GetPlayerAvatar() {
            if(currentPlayer.Avatar >= 0 && currentPlayer.Avatar < avatars.Length) return avatars[currentPlayer.Avatar];
            return null;
        }
    }
}