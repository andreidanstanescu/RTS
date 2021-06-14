using UnityEngine;
using System.Collections;
using RTS;
 
public class RichWin: VictoryCondition {
     
    public int prag = 5000;
     
    private string type = "mana";
     
    public override string GetDescription () {
        return "Collecting resources";
    }
     
    public override bool PlayerMeetsConditions (Player player) {
        return player && !player.IsDead() && player.getResurse(type) >= prag;
    }
}