using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class CardEffects
{
    public LoRBoard board;

    /**
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
                case "Mystic Shot":
                    if (card.targets[0] is UnitCard)
                    {
                        MysticShot((UnitCard)card.targets[0]);
                    }
                    else
                    {
                        MysticShot((Nexus)card.targets[0]);
                    }
                    break;
                case "Decimate":
                    Decimate((Nexus)card.targets[0]);
                    break;
                default:
                    Debug.Log("Spell not found.");
                    break;
            }
        }
        **/
}