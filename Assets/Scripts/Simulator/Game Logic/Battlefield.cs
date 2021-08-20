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

    public class BattlePair
    {
        public UnitCard attacker;
        public UnitCard blocker;

        public BattlePair(UnitCard attacker, UnitCard blocker)
        {
            this.attacker = attacker;
            this.blocker = blocker;
        }

        public override string ToString()
        {
            if (this.blocker != null)
            {
                return attacker.ToString() + " -> " + blocker.ToString();
            }
            return attacker.ToString() + " -> ";
        }
    }

    /// <summary>
    /// Adds the attacker as a new battle pair.
    /// </summary>
    public void DeclareAttacker(UnitCard unit)
    {
        battlingUnits.Add(new BattlePair(unit, null));
    }

    /// <summary>
    /// Adds the blocker to the attacker's battle pair.
    /// </summary>
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

    public void CheckUnitDeath()
    {
        foreach (BattlePair pair in battlingUnits)
        {
            UnitCard attacker = pair.attacker;
            UnitCard blocker = pair.blocker;
            if (attacker != null && attacker.IsDead())
            {
                pair.attacker = null;
            }
            if (blocker != null && blocker.IsDead())
            {
                pair.blocker = UnitCard.DummyCard();
            }
        }
    }

    /// <summary>
    /// Returns the attacking card at the specified index.
    /// </summary>
    public UnitCard AttackerAt(int index)
    {
        if (index > battlingUnits.Count - 1) return null;
        return battlingUnits[index].attacker;
    }

    /// <summary>
    /// Returns the blocking card at the specified index.
    /// </summary>
    public UnitCard BlockerAt(int index)
    {
        if (index > battlingUnits.Count - 1) return null;
        return battlingUnits[index].blocker;
    }

    /// <summary>
    /// Clears all battle pairs.
    /// </summary>
    public void ClearField()
    {
        battlingUnits = new List<BattlePair>();
    }

    public override string ToString()
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
}