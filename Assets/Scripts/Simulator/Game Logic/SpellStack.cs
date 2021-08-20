using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpellStack
{
    public List<SpellCard> spells = new List<SpellCard>();
    public List<int> castingOrder = new List<int>();
    public SpellEffects spellEffects;
    int maxSpells = 9;
    public int playerWithFirstCast;

    private int gameResult;

    public SpellStack(LoRBoard board)
    {
        spellEffects = new SpellEffects(board);
    }

    /// <summary>
    /// Adds a spell effect to the stack.
    /// </summary>
    public bool Add(SpellCard card, int castingPlayer)
    {
        switch (card.spellType)
        {
            case SpellType.Burst:
                spellEffects.Resolve(card, castingPlayer);
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
                    castingOrder.Insert(0, castingPlayer);
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
                    castingOrder.Insert(0, castingPlayer);
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
                    spellEffects.Resolve(card, castingPlayer);

                    return true;
                }

            default:
                return false;
        }
    }

    /// <summary>
    /// Resolves all effects on stack in reverse order.
    /// </summary>
    public void Resolve()
    {
        spellEffects.Resolve(spells[0], castingOrder[0]);
        spells.RemoveAt(0);
        castingOrder.RemoveAt(0);
    }

    public override string ToString()
    {
        string spellStackString = "";
        foreach (Card card in spells)
        {
            spellStackString += card.ToString() + "\n";
        }
        return spellStackString;
    }
}