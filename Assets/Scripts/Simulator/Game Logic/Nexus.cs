using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A Nexus represents a player's health.
/// </summary>
public class Nexus
{
    public int health;
    //for player buffs
    //public List<Effect> effects;

    public Nexus()
    {
        health = 20;
    }

    public void Heal(int amount)
    {
        health = Math.Min(health + amount, 20);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
    }

    public override string ToString()
    {
        return health.ToString();
    }
}