﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Bench represents all units on a player's backrow.
/// </summary>
public class Bench
{
    public List<UnitCard> units;
    public int maxBenchSize = 6;

    public Bench()
    {
        units = new List<UnitCard>();
    }

    /// <summary>
    /// Adds the unit to the bench if room.
    /// </summary>
    public void Play(UnitCard card)
    {
        if (units.Count < maxBenchSize)
        {
            units.Add(card);
        }
        else
        {
            //don't allow - actually should try replace
            Debug.Log(card.ToString() + " cannot be played to a full bench.");
        }
    }

    /// <summary>
    /// Adds the unit to the bench if room.
    /// </summary>
    public void Add(UnitCard card)
    {
        //note: needs to detect summon effects
        if (units.Count < maxBenchSize)
        {
            units.Add(card);
        }
    }

    /// <summary>
    /// Removes the unit from bench.
    /// </summary>
    public void MoveToCombat(UnitCard card)
    {
        units.RemoveAt(units.FindIndex(0, units.Count, x => x == card));
    }

    /// <summary>
    /// Removes the unit from bench.
    /// </summary>
    public void Remove(UnitCard card)
    {
        units.RemoveAt(units.FindIndex(0, units.Count, x => x == card));
    }

    /// <summary>
    /// Removes all dead units from bench.
    /// </summary>
    public void RemoveDeadUnits()
    {
        units.RemoveAll(unit => unit.health <= 0);
    }

    /// <summary>
    /// Returns true if bench is full.
    /// </summary>
    public bool IsFull()
    {
        return units.Count == maxBenchSize;
    }

    /// <summary>
    /// Returns true if bench is empty.
    /// </summary>
    public bool IsEmpty()
    {
        return units.Count == 0;
    }

    /// <summary>
    /// Returns true if bench contains the unit.
    /// </summary>
    public bool Contains(UnitCard unit)
    {
        return units.Contains(unit);
    }

    public override string ToString()
    {
        string benchString = "";
        foreach (UnitCard unit in units)
        {
            benchString += unit.ToString() + "\n";
        }
        return benchString;
    }

    public void CheckUnitDeath()
    {
        units.RemoveAll(unit => unit.IsDead());
    }

    public void RevertRoundBuffs()
    {
        foreach (UnitCard unit in units)
        {
            unit.RevertRoundBuff();
        }
    }
}
