using UnityEngine;
using System.Collections;
 
public class Survival : VictoryCondition {
     
    public int minutes = 5;
     
    private float timeLeft = 0.0f;
     
    void Awake() {
        timeLeft = minutes * 60;
    }
     
    void Update() {
        //Debug.Log(timeLeft);
        timeLeft -= Time.deltaTime;
    }
     
    public override string GetDescription () {
        return "Survival";
    }
 
    public override bool GameFinished () {
        foreach(Player player in players) {
            if(player && player.is_player && player.IsDead()) return true;
        }
        return timeLeft < 0;
    }
     
    public override bool PlayerMeetsConditions (Player player) {
        //Debug.Log(player && player.is_player && !player.IsDead());
        return player && player.is_player && !player.IsDead();
    }
}