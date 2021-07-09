using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    Effect effect;
    public SpellCard(string name, Effect effect)
    {
        this.name = name;
        this.effect = effect;
    }
}
