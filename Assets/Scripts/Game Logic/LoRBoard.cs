using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoRBoard
{

    public LoRBoardSide playerOneSide = new LoRBoardSide(1);
    public LoRBoardSide playerTwoSide = new LoRBoardSide(2);

    public Battlefield battlefield = new Battlefield();
    public SpellStack spellStack = new SpellStack();
    public int roundNumber;
    public int activePlayer;
    public int passCount;
    public int attackingPlayer;
    public bool inCombat;
    public bool blocked;

    public SpellCard activeSpell;
    public bool targeting;

    /// <summary>
    /// Sets up decks and draws starting hands (todo: mulligan state).
    /// </summary>
    public void Initialize(Deck playerOneDeck, Deck playerTwoDeck)
    {
        playerOneSide.SetDeck(playerOneDeck);
        playerTwoSide.SetDeck(playerTwoDeck);
        playerOneSide.opposingSide = playerTwoSide;
        playerTwoSide.opposingSide = playerOneSide;

        passCount = 0;
        attackingPlayer = 0;

        //each player draws four
        for (int i = 0; i < 4; i++)
        {
            playerOneSide.Draw();
            playerTwoSide.Draw();
        }

        roundNumber = 0;
        AdvanceRound();
    }

    /// <summary>
    /// Plays a unit card from the active player's hand to the bench.
    /// </summary>
    public bool PlayUnit(UnitCard card)
    {
        bool succeeded = true;

        if (activePlayer == 1)
        {
            succeeded = playerOneSide.PlayUnit(card);
        }
        else
        {
            succeeded = playerTwoSide.PlayUnit(card);
        }

        if (succeeded)
        {
            passCount = 0;
            SwitchActivePlayer();
        }

        return succeeded;
    }

    /// <summary>
    /// Plays a spell card from the active player's hand to the spell stack.
    /// </summary>
    public bool PlaySpell(SpellCard card)
    {
        bool succeeded = true;

        if (activePlayer == 1)
        {
            succeeded = playerOneSide.PlaySpell(card);
        }
        else
        {
            succeeded = playerTwoSide.PlaySpell(card);
        }

        if (succeeded)
        {
            passCount = 0;
            //a spell is on the stack that needs targets
            if (card.NeedsTargets())
            {
                activeSpell = card;
                targeting = true;
            }

            //pass priority if fast or slow
            else if (card.spellType != SpellType.Burst && card.spellType != SpellType.Focus)
            {
                if (spellStack.spells.Count == 0)
                {
                    spellStack.playerWithFirstCast = activePlayer;
                }
                spellStack.Add(card);
                SwitchActivePlayer();
            }
        }

        return succeeded;
    }

    /// <summary>
    /// Assigns the proper target (unit card) for a spell and adds it to the stack if all targets are set.
    /// </summary>
    public void AssignTarget(UnitCard target)
    {
        activeSpell.AssignNextTarget(target);
        if (!activeSpell.NeedsTargets())
        {
            if (activeSpell.spellType != SpellType.Burst && activeSpell.spellType != SpellType.Focus)
            {
                SwitchActivePlayer();
            }
            if (spellStack.spells.Count == 0)
            {
                spellStack.playerWithFirstCast = activePlayer;
            }
            spellStack.Add(activeSpell);
            activeSpell = null;
            targeting = false;
        }
    }

    /// <summary>
    /// Assigns the proper target (nexus) for a spell and adds it to the stack if all targets are set.
    /// </summary>
    public void AssignTarget(Nexus target)
    {
        activeSpell.AssignNextTarget(target);
        if (!activeSpell.NeedsTargets())
        {
            if (activeSpell.spellType != SpellType.Burst && activeSpell.spellType != SpellType.Focus)
            {
                SwitchActivePlayer();
            }

            if (spellStack.spells.Count == 0)
            {
                spellStack.playerWithFirstCast = activePlayer;
            }
            spellStack.Add(activeSpell);
            activeSpell = null;
            targeting = false;
        }
    }

    /// <summary>
    /// Increments round number and updates state.
    /// </summary>
    public void AdvanceRound()
    {
        roundNumber += 1;

        //Debug.Log("Advanced to round " + roundNumber + ".");

        playerOneSide.UpdateRound(roundNumber);
        playerTwoSide.UpdateRound(roundNumber);

        if (playerOneSide.hasAttackToken)
        {
            activePlayer = 1;
        }
        else
        {
            activePlayer = 2;
        }

        passCount = 0;
    }

    /// <summary>
    /// Commits a set of units to an attack.
    /// </summary>
    public void DeclareAttack(List<UnitCard> attackingUnits)
    {
        attackingPlayer = activePlayer;

        if (attackingPlayer == 1)
        {
            playerOneSide.RemoveAttackToken();
        }
        else
        {
            playerTwoSide.RemoveAttackToken();
        }

        Bench attackingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            attackingBench = playerOneSide.bench;
        }
        else
        {
            attackingBench = playerTwoSide.bench;
        }

        //add attackers
        List<UnitCard> attackingUnitsCopy = new List<UnitCard>(attackingUnits);
        foreach (UnitCard unit in attackingUnitsCopy)
        {
            attackingBench.MoveToCombat(unit);
            battlefield.DeclareAttacker(unit);
        }

        passCount = 0;
        SwitchActivePlayer();
        inCombat = true;
    }

    /// <summary>
    /// Commits a set of blockers in response to an attack.
    /// </summary>
    public void DeclareBlock(List<Battlefield.BattlePair> blockPairs)
    {
        Bench defendingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            defendingBench = playerTwoSide.bench;
        }
        else
        {
            defendingBench = playerOneSide.bench;
        }

        //assign blocker to attacker
        foreach (Battlefield.BattlePair pair in blockPairs)
        {
            defendingBench.MoveToCombat(pair.blocker);
            battlefield.DeclareBlocker(pair.blocker, pair.attacker);
        }

        ConfirmBlocks();
    }

    /// <summary>
    /// Assigns a blocker to an attacker, but doesn't commit it.
    /// </summary>
    public void DeclareSingleBlock(Battlefield.BattlePair pair)
    {
        Bench defendingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            defendingBench = playerTwoSide.bench;
        }
        else
        {
            defendingBench = playerOneSide.bench;
        }

        defendingBench.MoveToCombat(pair.blocker);
        battlefield.DeclareBlocker(pair.blocker, pair.attacker);
    }

    /// <summary>
    /// Commits all blocks.
    /// </summary>
    public void ConfirmBlocks()
    {
        blocked = true;
        passCount = 1; //need this so defending player doesn't get chance to respond after pass
        SwitchActivePlayer();
    }


    /// <summary>
    /// Assigns unit and nexus damage and exits combat.
    /// </summary>
    public void ResolveBattle()
    {
        Nexus defendingNexus = null;
        Bench attackingBench = null;
        Bench defendingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            defendingNexus = playerTwoSide.nexus;
            attackingBench = playerOneSide.bench;
            defendingBench = playerTwoSide.bench;
        }
        else
        {
            defendingNexus = playerOneSide.nexus;
            attackingBench = playerTwoSide.bench;
            defendingBench = playerOneSide.bench;
        }

        //int totalNexusDamage = 0;

        //resolve pairs
        foreach (Battlefield.BattlePair pair in battlefield.battlingUnits)
        {
            if (pair.attacker != null)
            {
                if (pair.blocker == null)
                {
                    //reduce nexus health
                    defendingNexus.Damage(pair.attacker.power);
                    //totalNexusDamage += pair.attacker.power;
                    attackingBench.Add(pair.attacker);
                }
                else
                {
                    //resolve damage
                    pair.attacker.Strike(pair.blocker);

                    //blocker damage, handle quick attack
                    if (!pair.attacker.HasKeyword(Keyword.QuickAttack) || pair.blocker.health > 0)
                    {
                        pair.blocker.Strike(pair.attacker);
                    }

                    //handle overwhelm damage
                    if (pair.attacker.HasKeyword(Keyword.Overwhelm) && pair.blocker.health < 0)
                    {
                        defendingNexus.Damage(Math.Abs(pair.blocker.health));
                    }

                    if (pair.attacker.health > 0)
                    {
                        attackingBench.Add(pair.attacker);
                    }
                    else
                    {
                        //Debug.Log("Player " + attackingPlayer + "'s " + pair.attacker.ToString() + " died in combat.");
                    }
                    if (pair.blocker.health > 0)
                    {
                        defendingBench.Add(pair.blocker);
                    }
                    else
                    {
                        //Debug.Log("Player " + (3 - attackingPlayer) + "'s " + pair.blocker.ToString() + " died in combat.");
                    }
                }
            }
        }

        //Debug.Log("Player " + activePlayer + " took " + totalNexusDamage + " damage from combat.");

        battlefield.ClearField();
        inCombat = false;
        blocked = false;

        attackingPlayer = 0;
        SwitchActivePlayer();
        passCount = 0;
    }

    public bool SpellsAreActive()
    {
        return spellStack.spells.Count > 0;
    }

    /// <summary>
    /// Passes player priority (no action).
    /// </summary>
    public void Pass()
    {
        if (inCombat)
        {
            if (!blocked || passCount == 1) //no blocks or a player has passed
            {
                if (SpellsAreActive())
                {
                    spellStack.Resolve();
                }
                ResolveBattle();
            }
            else
            {
                passCount += 1;
                SwitchActivePlayer();
            }
        }
        else if (SpellsAreActive())
        {
            passCount = 0;
            activePlayer = 3 - spellStack.playerWithFirstCast;
            spellStack.Resolve();
        }
        else
        {
            passCount += 1;
            SwitchActivePlayer();

            if (passCount == 2)
            {
                AdvanceRound();
            }
        }
    }

    /// <summary>
    /// Changes active player to the other player.
    /// </summary>
    public void SwitchActivePlayer()
    {
        activePlayer = 3 - activePlayer;
    }
}
