using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellStack
{
    public List<SpellCard> spells = new List<SpellCard>();
    public SpellEffects spellEffects = new SpellEffects();
    int maxSpells = 9;

    /// <summary>
    /// Adds a spell effect to the stack.
    /// </summary>
    public bool Add(SpellCard card)
    {
        switch (card.spellType)
        {
            case SpellType.Burst:
                spellEffects.Resolve(card);
                return true;

            case SpellType.Fast:
                if (spells.Count == maxSpells)
                {
                    Debug.Log("No more spells can be played.");
                    return false;
                }
                else
                {
                    spells.Insert(0, card);
                    return true;
                }

            case SpellType.Slow:
                if (spells.Count > 0)
                {
                    Debug.Log("Slow spells cannot be played onto the stack.");
                    return false;
                }
                else
                {
                    spells.Insert(0, card);
                    return true;
                }

            case SpellType.Focus:
                if (spells.Count > 0)
                {
                    Debug.Log("Focus spells cannot be played onto the stack.");
                    return false;
                }
                else
                {
                    //SpellEffects.Resolve(card);
                    return true;
                }

            default:
                return false;
        }
    }

    /// <summary>
    /// Resolves all effects on stack.
    /// </summary>
    public void Resolve()
    {
        while (spells.Count > 0)
        {
            //SpellEffects.Resolve(spells[0]);
            spells.RemoveAt(0);
        }
    }
}