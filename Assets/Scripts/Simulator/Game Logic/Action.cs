using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An Action is a discrete move by a player that impacts the state of the game.
/// </summary>
public class Action
{
    public string command;
    public object target;
    public List<UnitCard> units;
    public UnitCard attacker;
    public UnitCard blocker;
    public List<Battlefield.BattlePair> pairs;

    private string argumentString;

    //base
    public Action(string command)
    {
        this.command = command;
    }

    /// <summary>
    /// Overload for targeting or single attacks.
    /// </summary>
    public Action(string command, Card target)
    {
        this.command = command;
        argumentString = target.ToString();
        if (command == "Attack")
        {
            this.attacker = (UnitCard)target;
        }
        else
        {
            this.target = target;
        }
    }

    /// <summary>
    /// Overload for targeting a nexus.
    /// </summary>
    public Action(string command, Nexus target)
    {
        this.command = command;
        argumentString = target.ToString();
        this.target = target;
    }

    /// <summary>
    /// Overload for a series of attacks.
    /// </summary>
    public Action(string command, List<UnitCard> units)
    {
        this.command = command;
        argumentString = units.ToString();
        this.units = units;
    }

    /// <summary>
    /// Overload for a series of blocks.
    /// </summary>
    public Action(string command, List<Battlefield.BattlePair> pairs)
    {
        this.command = command;
        argumentString = pairs.ToString();
        this.pairs = pairs;
    }

    /// <summary>
    /// Overload for a single block or challenge.
    /// </summary>
    public Action(string command, UnitCard attacker, UnitCard blocker)
    {
        this.command = command;
        argumentString = attacker.ToString() + ", " + blocker.ToString();
        this.attacker = attacker;
        this.blocker = blocker;
    }

    /// <summary>
    /// Overload for a single block or challenge.
    /// </summary>
    public Action(string command, Battlefield.BattlePair pair)
    {
        this.command = command;
        argumentString = pair.ToString();
        this.attacker = pair.attacker;
        this.blocker = pair.blocker;
    }

    public override string ToString()
    {
        if (command == "Pass") return command;
        return command + ": " + argumentString;
    }
}