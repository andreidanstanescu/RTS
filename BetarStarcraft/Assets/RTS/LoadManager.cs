using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
 
namespace RTS {
    public static class LoadManager {
         
        public static void LoadGame(string filename) {
            char separator = Path.DirectorySeparatorChar;
            string path = "SavedGames" + separator + PlayerManager.GetPlayerName() + separator + filename + ".json";
            if(!File.Exists(path)) {
                Debug.Log("Unable to find " + path + ". Loading will crash, so aborting.");
                return;
            }
            string input;
            using(StreamReader sr = new StreamReader(path)) {
                input = sr.ReadToEnd();
            }
            if(input != null) {
                //parse contents of file
                using(JsonTextReader reader = new JsonTextReader(new StringReader(input))) {
                    while(reader.Read()) {
                        if(reader.Value!=null) {
                            if(reader.TokenType == JsonToken.PropertyName) {
                                string property = (string)reader.Value;
                                switch(property) {
                                    case "Sun": LoadLighting(reader); break;
                                    case "Ground": LoadTerrain(reader); break;
                                    case "Camera": LoadCamera(reader); break;
                                    case "Resources": LoadResources(reader); break;
                                    //case "Players": LoadPlayers(reader); break;
                                    default: break;
                                }
                            }
                        }
                    }
                }
            }
        }
         
        private static void LoadLighting(JsonTextReader reader) {
            if(reader == null) return;
            Vector3 position = new Vector3(0,0,0), scale = new Vector3(1,1,1);
            Quaternion rotation = new Quaternion(0,0,0,0);
            while(reader.Read()) {
                if(reader.Value != null) {
                    if(reader.TokenType == JsonToken.PropertyName) {
                        if((string)reader.Value == "Position") position = LoadVector(reader);
                        else if((string)reader.Value == "Rotation") rotation = LoadQuaternion(reader);
                        else if((string)reader.Value == "Scale") scale = LoadVector(reader);
                    }
                } else if(reader.TokenType == JsonToken.EndObject) {
                    GameObject sun = (GameObject)GameObject.Instantiate(GameService.extractWorldObject("Sun"), position, rotation);
                    sun.transform.localScale = scale;
                    return;
                }
            } 
        }
         
        private static void LoadTerrain(JsonTextReader reader) {
            if(reader == null) return;
            Vector3 position = new Vector3(0,0,0), scale = new Vector3(1,1,1);
            Quaternion rotation = new Quaternion(0,0,0,0);
            while(reader.Read()) {
                if(reader.Value != null) {
                    if(reader.TokenType == JsonToken.PropertyName) {
                        if((string)reader.Value == "Position") position = LoadVector(reader);
                        else if((string)reader.Value == "Rotation") rotation = LoadQuaternion(reader);
                        else if((string)reader.Value == "Scale") scale = LoadVector(reader);
                    }
                } else if(reader.TokenType == JsonToken.EndObject) {
                    GameObject ground = (GameObject)GameObject.Instantiate(GameService.extractWorldObject("Ground"), position, rotation);
                    ground.transform.localScale = scale;
                    return;
                }
            }
        }
         
       private static void LoadCamera(JsonTextReader reader) {
        if(reader == null) return;
        Vector3 position = new Vector3(0,0,0), scale = new Vector3(1,1,1);
        Quaternion rotation = new Quaternion(0,0,0,0);
        while(reader.Read()) {
            if(reader.Value != null) {
                if(reader.TokenType == JsonToken.PropertyName) {
                    if((string)reader.Value == "Position") position = LoadVector(reader);
                    else if((string)reader.Value == "Rotation") rotation = LoadQuaternion(reader);
                    else if((string)reader.Value == "Scale") scale = LoadVector(reader);
                }
            } else if(reader.TokenType == JsonToken.EndObject) {
                GameObject camera = Camera.main.gameObject;
                camera.transform.localPosition = position;
                camera.transform.localRotation = rotation;
                camera.transform.localScale = scale;
                return;
            }
        }
    }
         
        private static void LoadResources(JsonTextReader reader) {
            if(reader == null) return;
            string currValue = "", type = "";
            while(reader.Read()) {
                if(reader.Value != null) {
                    if(reader.TokenType == JsonToken.PropertyName) currValue = (string)reader.Value;
                    else if(currValue == "Type") {
                        type = (string)reader.Value;
                        GameObject newObject = (GameObject)GameObject.Instantiate(GameService.extractWorldObject(type));
                        Resource resource = newObject.GetComponent< Resource >();
                        resource.LoadDetails(reader);
                    }
                }
                else if(reader.TokenType==JsonToken.EndArray) return;
            }
        }
         
        public static Vector3 LoadVector(JsonTextReader reader) {
            Vector3 position = new Vector3(0,0,0);
            if(reader == null) return position;
            string currVal = "";
            while(reader.Read()) {
                if(reader.Value!=null) {
                    if(reader.TokenType == JsonToken.PropertyName) currVal = (string)reader.Value;
                    else {
                        switch(currVal) {
                            case "x": position.x = (float)(double)reader.Value; break;
                            case "y": position.y = (float)(double)reader.Value; break;
                            case "z": position.z = (float)(double)reader.Value; break;
                            default: break;
                        }
                    }
                } else if(reader.TokenType == JsonToken.EndObject) return position;
            }
            return position;
        }
        
        public static Quaternion LoadQuaternion(JsonTextReader reader) {
            Quaternion rotation = new Quaternion(0,0,0,0);
            if(reader == null) return rotation;
            string currVal = "";
            while(reader.Read()) {
                if(reader.Value!=null) {
                    if(reader.TokenType == JsonToken.PropertyName) currVal = (string)reader.Value;
                    else {
                        switch(currVal) {
                            case "x": rotation.x = (float)(double)reader.Value; break;
                            case "y": rotation.y = (float)(double)reader.Value; break;
                            case "z": rotation.z = (float)(double)reader.Value; break;
                            case "w": rotation.w = (float)(double)reader.Value; break;
                            default: break;
                        }
                    }
                } else if(reader.TokenType == JsonToken.EndObject) return rotation;
            }
            return rotation;
        }
    }
    
}