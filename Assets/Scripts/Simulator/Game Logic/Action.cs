using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string command;
    public object target;
    public List<UnitCard> units;
    public UnitCard attacker;
    public UnitCard blocker;
    public List<Battlefield.BattlePair> pairs;

    //base
    public Action(string command)
    {
        this.command = command;
    }

    //for targeting cards (or single attack)
    public Action(string command, Card target)
    {
        this.command = command;
        if (command == "Attack")
        {
            this.attacker = (UnitCard)target;
        }
        else
        {
            this.target = target;
        }
    }

    //for a nexus target
    public Action(string command, Nexus target)
    {
        this.command = command;
        this.target = target;
    }

    //for a series of attacks
    public Action(string command, List<UnitCard> units)
    {
        this.command = command;
        this.units = units;
    }

    //for a series of blocks
    public Action(string command, List<Battlefield.BattlePair> pairs)
    {
        this.command = command;
        this.pairs = pairs;
    }

    //for a single block or challenge
    public Action(string command, UnitCard attacker, UnitCard blocker)
    {
        this.command = command;
        this.attacker = attacker;
        this.blocker = blocker;
    }

    //for a single block or challenge
    public Action(string command, Battlefield.BattlePair pair)
    {
        this.command = command;
        this.attacker = pair.attacker;
        this.blocker = pair.blocker;
    }
}