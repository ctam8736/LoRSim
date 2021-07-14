using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpellEffects
{
    public LoRBoard board;

    public void ResolveEffect(string cardName, UnitCard target)
    {

    }
    public void HealthPotion(UnitCard target)
    {
        target.Heal(3);
    }

    public void HealthPotion(Nexus target)
    {
        target.Heal(3);
    }
}