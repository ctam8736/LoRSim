using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    SpellType spellType;

    public SpellCard(string name, SpellType spellType)
    {
        this.name = name;
        this.spellType = spellType;
    }
}

public enum SpellType
{
    Burst,
    Fast,
    Slow,
    Focus
}
