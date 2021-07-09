using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Battlefield
{
    public List<BattlePair> battlingUnits;

    public Battlefield()
    {
        battlingUnits = new List<BattlePair>();
    }

    public void DeclareAttacker(UnitCard unit)
    {
        battlingUnits.Add(new BattlePair(unit, null));
    }

    public void DeclareBlocker(UnitCard unit, UnitCard attacker)
    {
        foreach (BattlePair pair in battlingUnits)
        {
            if (pair.attacker == attacker)
            {
                pair.blocker = unit;
            }
        }
    }

    public void ClearField()
    {
        battlingUnits = new List<BattlePair>();
    }

    public string ToString()
    {
        if (battlingUnits.Count == 0)
        {
            return "No units battling.";
        }

        string battlefieldString = "";
        foreach (BattlePair pair in battlingUnits)
        {
            battlefieldString += pair.ToString() + "\n";
        }
        return battlefieldString;
    }

    public class BattlePair
    {
        public UnitCard attacker;
        public UnitCard blocker;

        public BattlePair(UnitCard attacker, UnitCard blocker)
        {
            this.attacker = attacker;
            this.blocker = blocker;
        }

        public string ToString()
        {
            if (this.blocker != null)
            {
                return attacker.ToString() + " -> " + blocker.ToString();
            }
            return attacker.ToString() + " -> ";
        }
    }

}