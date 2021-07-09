using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nexus
{
    public int health;
    //for player buffs
    //public List<Effect> effects;

    public Nexus()
    {
        health = 20;
    }

    public string ToString()
    {
        return health.ToString();
    }
}