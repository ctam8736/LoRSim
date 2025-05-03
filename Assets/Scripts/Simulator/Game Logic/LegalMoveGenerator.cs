using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A LegalMoveGenerator contains functions that provide move options for any given state of a board state.
/// </summary>
public class LegalMoveGenerator
{
    public LoRBoard board;
    List<GameAction> legalMoves = new List<GameAction>();
    LoRBoardSide activeSide = null;
    LoRBoardSide opposingSide = null;

    public LegalMoveGenerator(LoRBoard board)
    {
        this.board = board;
    }

    /// <summary>
    /// Returns all legal actions for the given board state, assuming the role of the active player.
    /// </summary>
    public List<GameAction> LegalMoves()
    {
        legalMoves = new List<GameAction>();
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
            //Debug.Log("(Is Targeting)");
            AddLegalTargets();
            return legalMoves; //no pass allowed
        }

        else if (board.declaringAttacks)
        {
            //Debug.Log("(Is Attacking)");
            AddLegalAttacks();
            AddResponseSpellcasts();
        }

        else if (board.declaringBlocks || (board.inCombat && !board.blocked))
        {
            //Debug.Log("(Is Blocking)");
            AddLegalBlocks();
            AddResponseSpellcasts();
        }

        else if (board.SpellsAreActive() || board.inCombat || board.casting)
        {
            //Debug.Log("(Only Spells Active)");
            AddResponseSpellcasts();
        }

        else
        {

            //Debug.Log("(Default)");
            if (activeSide.hasAttackToken)
            {
                AddLegalAttacks();
            }
            AddSlowPlays();

        }

        AddPass();
        return legalMoves;
    }

    public void AddLegalTargets()
    {
        switch (board.activeSpell.nextTargetType)
        {
            case TargetType.AlliedUnit:
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                        {
                            legalMoves.Add(new GameAction("Target", pair.attacker));
                        }
                    }
                    else
                    {
                        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                        {
                            if (pair.blocker != null)
                            {
                                legalMoves.Add(new GameAction("Target", pair.blocker));
                            }
                        }
                    }
                }
                foreach (UnitCard unit in activeSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                break;

            case TargetType.EnemyUnit:
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                        {
                            if (pair.blocker != null)
                            {
                                legalMoves.Add(new GameAction("Target", pair.blocker));
                            }
                        }
                    }
                    else
                    {
                        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                        {
                            legalMoves.Add(new GameAction("Target", pair.attacker));
                        }
                    }
                }
                foreach (UnitCard unit in opposingSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                break;

            case TargetType.AlliedUnitOrNexus: // can easily add combat
                foreach (UnitCard unit in activeSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                legalMoves.Add(new GameAction("Target", activeSide.nexus));
                break;

            case TargetType.AlliedNexus:
                legalMoves.Add(new GameAction("Target", activeSide.nexus));
                break;

            case TargetType.EnemyUnitOrNexus: // can easily add combat units
                foreach (UnitCard unit in opposingSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                legalMoves.Add(new GameAction("Target", opposingSide.nexus));
                break;

            case TargetType.EnemyNexus:
                legalMoves.Add(new GameAction("Target", opposingSide.nexus));
                break;

            case TargetType.Anything: // can easily add combat units
                foreach (UnitCard unit in activeSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                foreach (UnitCard unit in opposingSide.bench.units)
                {
                    legalMoves.Add(new GameAction("Target", unit));
                }
                legalMoves.Add(new GameAction("Target", activeSide.nexus));
                legalMoves.Add(new GameAction("Target", opposingSide.nexus));
                break;
        }
    }

    public bool SpellIsLegal(SpellCard spell)
    {
        if (!HandleSpellPreconditions(spell)) return false;
        if (board.inCombat && (spell.spellType == SpellType.Slow || spell.spellType == SpellType.Focus)) return false;
        if (board.SpellsAreActive() && (spell.spellType == SpellType.Slow || spell.spellType == SpellType.Focus)) return false;
        //note: doesn't work for multi-targeted spells
        switch (spell.nextTargetType)
        {
            case TargetType.AlliedUnit:
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        return !activeSide.bench.IsEmpty() || board.battlefield.AttackersExist();
                    }
                    return !activeSide.bench.IsEmpty() || board.battlefield.BlockersExist();
                }
                return !activeSide.bench.IsEmpty();

            case TargetType.EnemyUnit:
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        return !activeSide.bench.IsEmpty() || board.battlefield.BlockersExist();
                    }
                    return !activeSide.bench.IsEmpty() || board.battlefield.AttackersExist();
                }
                return !activeSide.bench.IsEmpty() || board.battlefield.AttackersExist();

            case TargetType.AlliedUnitOrNexus:
                return true;

            case TargetType.AlliedNexus:
                return true;

            case TargetType.EnemyUnitOrNexus:
                return true;

            case TargetType.EnemyNexus:
                return true;

            case TargetType.Anything:
                return true;

            default:
                return true;
        }
    }

    private bool HandleSpellPreconditions(SpellCard spell)
    {
        switch (spell.name)
        {
            case "Stand Alone":
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        return activeSide.bench.units.Count + board.battlefield.AttackerCount() == 1;
                    }
                    return activeSide.bench.units.Count + board.battlefield.BlockerCount() == 1;
                }
                return activeSide.bench.units.Count == 1;

            case "Single Combat":
                bool alliedUnitsExist = false;
                bool enemyUnitsExist = false;
                if (board.inCombat)
                {
                    if (activeSide.isAttacking)
                    {
                        if (board.battlefield.AttackerCount() > 0) { alliedUnitsExist = true; }
                        if (board.battlefield.BlockerCount() > 0) { enemyUnitsExist = true; }
                    }
                    else
                    {
                        if (board.battlefield.AttackerCount() > 0) { enemyUnitsExist = true; }
                        if (board.battlefield.BlockerCount() > 0) { alliedUnitsExist = true; }
                    }
                }
                if (activeSide.bench.units.Count > 0) { alliedUnitsExist = true; }
                if (opposingSide.bench.units.Count > 0) { enemyUnitsExist = true; }
                return alliedUnitsExist && enemyUnitsExist;

            default:
                return true;
        }
    }

    public void AddLegalAttacks()
    {
        foreach (UnitCard unit in activeSide.bench.units) //any unit on bench attack check
        {
            if (!(unit.HasKeyword(Keyword.CantAttack) || unit.HasKeyword(Keyword.Stunned))) //not disabled or stunned
            {
                legalMoves.Add(new GameAction("Attack", unit));
                if (unit.HasKeyword(Keyword.Challenger)) //challenger
                {
                    foreach (UnitCard dragTarget in opposingSide.bench.units)
                    {
                        legalMoves.Add(new GameAction("Challenge", unit, dragTarget));
                    }
                }
            }
        }
    }

    public void AddLegalBlocks()
    {
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
                    legalMoves.Add(new GameAction("Block", pair.attacker, unit));
                }
            }
        }
    }

    public void AddResponseSpellcasts()
    {
        foreach (Card card in activeSide.hand.cards)
        {
            if (card is SpellCard)
            {
                SpellCard spell = (SpellCard)card;
                if (spell.cost <= activeSide.mana.TotalMana() && SpellIsLegal(spell) && (spell.spellType != SpellType.Slow))
                {
                    legalMoves.Add(new GameAction("Play", card));
                }
            }
        }
    }

    public void AddSlowPlays()
    {
        foreach (Card card in activeSide.hand.cards)
        {
            if (card is SpellCard)
            {
                if (card.cost <= activeSide.mana.TotalMana() && SpellIsLegal((SpellCard)card))
                {
                    legalMoves.Add(new GameAction("Play", card));
                }
            }
            else
            {
                if (card.cost <= activeSide.mana.manaGems && !activeSide.bench.IsFull()) // this is incorrect until I implement overwriting
                {
                    legalMoves.Add(new GameAction("Play", card));
                }
            }
        }

    }

    public void AddPass()
    {
        legalMoves.Add(new GameAction("Pass"));
    }
}