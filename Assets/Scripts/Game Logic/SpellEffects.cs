using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpellEffects
{
    public LoRBoard board;

    public void Resolve(SpellCard card)
    {
        switch (card.name)
        {
            case "Health Potion":
                if (card.targets[0] is UnitCard)
                {
                    HealthPotion((UnitCard)card.targets[0]);
                }
                else
                {
                    HealthPotion((Nexus)card.targets[0]);
                }
                break;
        }
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