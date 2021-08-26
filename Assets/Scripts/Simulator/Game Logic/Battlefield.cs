using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the combat area of the board, split into an attacking side and a defending side.
/// </summary>
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
                if (pair.attacker.HasKeyword(Keyword.Elusive) && !unit.HasKeyword(Keyword.Elusive)) //check elusive, does not handle sharpsight yet :P
                {
                    Debug.Log("Illegal: " + pair.attacker.name + " cannot be blocked by " + unit.name);
                }
                else
                {
                    pair.blocker = unit;
                }
                break;
            }
        }
    }

    /// <summary>
    /// Returns true if the battlefield contains the unit.
    /// </summary>
    public bool Contains(UnitCard unit)
    {
        foreach (Battlefield.BattlePair pair in battlingUnits)
        {
            if (pair.attacker == unit || pair.blocker == unit)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes dead units from the battlefield.
    /// </summary>
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

    public bool AttackersExist()
    {
        return battlingUnits.Count > 0;
    }

    public int AttackerCount()
    {
        return battlingUnits.Count;
    }

    public bool BlockersExist()
    {
        foreach (Battlefield.BattlePair pair in battlingUnits)
        {
            if (pair.blocker != null) return true;
        }
        return false;
    }

    public int BlockerCount()
    {
        int count = 0;
        foreach (Battlefield.BattlePair pair in battlingUnits)
        {
            if (pair.blocker != null) count += 1;
        }
        return count;
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