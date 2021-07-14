using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public string command;
    public object target;
    public List<UnitCard> units;
    public List<Battlefield.BattlePair> pairs;
    public Action(string command)
    {
        this.command = command;
    }
    public Action(string command, Card target)
    {
        this.command = command;
        this.target = target;
    }

    public Action(string command, Nexus target)
    {
        this.command = command;
        this.target = target;
    }

    public Action(string command, List<UnitCard> units)
    {
        this.command = command;
        this.units = units;
    }

    public Action(string command, List<Battlefield.BattlePair> pairs)
    {
        this.command = command;
        this.pairs = pairs;
    }
}