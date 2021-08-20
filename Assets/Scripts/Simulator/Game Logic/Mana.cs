using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mana
{
    public int maxMana;
    public int manaGems;
    public int spellMana;

    public Mana()
    {
        maxMana = 0;
        manaGems = 0;
        spellMana = 0;
    }

    public override string ToString()
    {
        return "Mana: " + manaGems + "/" + maxMana + "\nSpell Mana: " + spellMana + "/3";
    }
}
