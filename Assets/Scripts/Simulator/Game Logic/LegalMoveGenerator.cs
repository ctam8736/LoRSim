using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A LegalMoveGenerator contains functions that provide move options for any given state of a board state.
/// </summary>
public class LegalMoveGenerator
{
    public LoRBoard board;

    public LegalMoveGenerator(LoRBoard board)
    {
        this.board = board;
    }

    /// <summary>
    /// Returns all legal actions for the given board state, assuming the role of the active player.
    /// </summary>
    public List<Action> LegalMoves()
    {
        //Debug.Log(board.activePlayer);
        List<Action> legalMoves = new List<Action>();
        LoRBoardSide activeSide = null;
        LoRBoardSide opposingSide = null;
        if (board.activePlayer == 1)
        {
            activeSide = board.playerOneSide;
            opposingSide = board.playerTwoSide;
        }
        else
        {
            activeSide = board.playerTwoSide;
            opposingSide = board.playerOneSide;
        }

        if (board.targeting)
        {
            Debug.Log("(Is Targeting)");
            switch (board.activeSpell.nextTargetType)
            {
                case TargetType.AlliedUnit:
                    foreach (UnitCard unit in activeSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    break;

                case TargetType.EnemyUnit:
                    foreach (UnitCard unit in opposingSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    break;

                case TargetType.AlliedUnitOrNexus:
                    foreach (UnitCard unit in activeSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    legalMoves.Add(new Action("Target", activeSide.nexus));
                    break;

                case TargetType.AlliedNexus:
                    legalMoves.Add(new Action("Target", activeSide.nexus));
                    break;

                case TargetType.EnemyUnitOrNexus:
                    foreach (UnitCard unit in opposingSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    legalMoves.Add(new Action("Target", opposingSide.nexus));
                    break;

                case TargetType.EnemyNexus:
                    legalMoves.Add(new Action("Target", opposingSide.nexus));
                    break;

                case TargetType.Anything:
                    foreach (UnitCard unit in activeSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    foreach (UnitCard unit in opposingSide.bench.units)
                    {
                        legalMoves.Add(new Action("Target", unit));
                    }
                    legalMoves.Add(new Action("Target", activeSide.nexus));
                    legalMoves.Add(new Action("Target", opposingSide.nexus));
                    break;

            }
            //can only target things of the specified type of activeSpell
            return legalMoves;
        }

        if (board.declaringAttacks)
        {
            Debug.Log("(Is Attacking)");
            foreach (UnitCard unit in activeSide.bench.units) //any unit on bench attack check
            {
                if (!(unit.HasKeyword(Keyword.CantAttack) || unit.HasKeyword(Keyword.Stunned))) //not disabled or stunned
                {
                    legalMoves.Add(new Action("Attack", unit));
                }
                if (unit.HasKeyword(Keyword.Challenger)) //not disabled or stunned
                {
                    foreach (UnitCard dragTarget in opposingSide.bench.units)
                    {
                        legalMoves.Add(new Action("Challange", unit, dragTarget));
                    }
                }
            }
            foreach (Card card in activeSide.hand.cards)
            {
                if (card is SpellCard)
                {
                    SpellCard spell = (SpellCard)card;
                    if (spell.cost <= activeSide.mana.TotalMana() && (spell.spellType != SpellType.Slow))
                    {
                        legalMoves.Add(new Action("Play", card));
                    }
                }
            }
            legalMoves.Add(new Action("Pass"));
            return legalMoves;
        }

        if (board.declaringBlocks || (board.inCombat && !board.blocked))
        {
            Debug.Log("(Is Blocking)");
            foreach (UnitCard unit in activeSide.bench.units) //add all block combinations
            {
                foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                {
                    if (
                            pair.blocker == null && //not already blocked
                            !(pair.attacker.HasKeyword(Keyword.Elusive) && !unit.HasKeyword(Keyword.Elusive)) && //elusives only block each other
                            !unit.HasKeyword(Keyword.CantBlock) //can block
                        )
                    {
                        legalMoves.Add(new Action("Block", pair.attacker, unit));
                    }
                }
            }
            foreach (Card card in activeSide.hand.cards)
            {
                if (card is SpellCard)
                {
                    SpellCard spell = (SpellCard)card;
                    if (spell.cost <= activeSide.mana.TotalMana() && (spell.spellType == SpellType.Fast || spell.spellType == SpellType.Burst))
                    {
                        legalMoves.Add(new Action("Play", card));
                    }
                }
            }
            legalMoves.Add(new Action("Pass"));
            return legalMoves;
        }

        if (board.SpellsAreActive())
        {
            Debug.Log("(Spell Stack Active)");
            foreach (Card card in activeSide.hand.cards)
            {
                if (card is SpellCard)
                {
                    SpellCard spell = (SpellCard)card;
                    if (spell.cost <= activeSide.mana.TotalMana() && (spell.spellType == SpellType.Fast || spell.spellType == SpellType.Burst))
                    {
                        legalMoves.Add(new Action("Play", card));
                    }
                }
            }
        }

        //otherwise...

        if (board.casting)
        {
            Debug.Log("(Is Casting)");
            //cast more spells or pass
            foreach (Card card in activeSide.hand.cards)
            {
                if (card is SpellCard)
                {
                    SpellCard spell = (SpellCard)card;
                    if (spell.cost <= activeSide.mana.TotalMana() && (spell.spellType == SpellType.Fast || spell.spellType == SpellType.Burst))
                    {
                        legalMoves.Add(new Action("Play", card));
                    }
                }
            }
            legalMoves.Add(new Action("Pass"));
            return legalMoves;
        }

        Debug.Log("(Default)");
        if (activeSide.hasAttackToken)
        {
            foreach (UnitCard unit in activeSide.bench.units) //any unit on bench attack check
            {
                if (!(unit.HasKeyword(Keyword.CantAttack) || unit.HasKeyword(Keyword.Stunned))) //not disabled or stunned
                {
                    legalMoves.Add(new Action("Attack", unit));
                }
                if (unit.HasKeyword(Keyword.Challenger)) //not disabled or stunned
                {
                    foreach (UnitCard dragTarget in opposingSide.bench.units)
                    {
                        legalMoves.Add(new Action("Challange", unit, dragTarget));
                    }
                }
            }
        }

        foreach (Card card in activeSide.hand.cards)
        {
            if (card is SpellCard)
            {
                if (card.cost <= activeSide.mana.TotalMana())
                {
                    legalMoves.Add(new Action("Play", card));
                }
            }
            else
            {
                if (card.cost <= activeSide.mana.manaGems)
                {
                    legalMoves.Add(new Action("Play", card));
                }
            }
        }

        legalMoves.Add(new Action("Pass"));

        return legalMoves;
    }
}