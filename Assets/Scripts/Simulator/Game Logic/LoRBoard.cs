﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A LoRBoard encapsulates an entire game state, including decks, hands, boards, and all the cards they consist of.
/// </summary>
public class LoRBoard
{
    // game data structures
    public LoRBoardSide playerOneSide = new LoRBoardSide(1);
    public LoRBoardSide playerTwoSide = new LoRBoardSide(2);

    public Battlefield battlefield = new Battlefield();
    public SpellStack spellStack;

    //state variables
    public int roundNumber;
    public int activePlayer;
    public int passCount;
    public int attackingPlayer;
    public bool inCombat;
    public bool blocked;
    public bool targeting;
    public bool declaringAttacks;
    public bool declaringBlocks;
    public bool casting;

    //public bool mulliganing;
    public int gameResult = -1; //-1 if undetermined, 0 if draw, 1 if player 1 won, 2 if player two won

    //saved cards
    public SpellCard activeSpell;
    public UnitCard bufferedUnit;

    bool deckTerminationDisabled = false;

    /// <summary>
    /// Sets up decks and draws starting hands (todo: mulligan state).
    /// </summary>
    public void Initialize(Deck playerOneDeck, Deck playerTwoDeck, bool deckTerminationDisabled = false)
    {
        this.deckTerminationDisabled = deckTerminationDisabled;

        playerOneSide.SetDeck(playerOneDeck);
        playerTwoSide.SetDeck(playerTwoDeck);
        playerOneSide.opposingSide = playerTwoSide;
        playerTwoSide.opposingSide = playerOneSide;

        spellStack = new SpellStack(this);

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
        //check if player can afford unit and has board space
        if (activePlayer == 1 && !playerOneSide.CanPlayUnit(card)) return false;
        if (activePlayer == 2 && !playerTwoSide.CanPlayUnit(card)) return false;

        //trigger card on play effect
        if (card.onPlay != null)
        {
            SpellCard effect = card.onPlay;
            passCount = 0;

            bool skipTargets = false;
            //a spell is on the stack that needs targets
            if (effect.NeedsTargets())
            {
                //no valid targets
                if (effect.nextTargetType == TargetType.AlliedUnit)
                {
                    if (activePlayer == 1 && playerOneSide.bench.units.Count == 0)
                    {
                        //Debug.Log("targeting skipped");
                        skipTargets = true;
                    }
                    if (activePlayer == 2 && playerTwoSide.bench.units.Count == 0)
                    {
                        //Debug.Log("targeting skipped");
                        skipTargets = true;
                    }
                }

                if (!skipTargets)
                {
                    //Debug.Log(effect.name + " is active...");
                    activeSpell = effect;
                    targeting = true;
                    bufferedUnit = card;
                    return true;
                }
            }

            if (!skipTargets)
            {
                //begin stack if fast or slow
                AddToSpellStack(effect);
            }
        }

        //play the unit
        if (activePlayer == 1)
        {
            playerOneSide.PlayUnit(card);
        }
        else if (activePlayer == 2)
        {
            playerTwoSide.PlayUnit(card);
        }

        //trigger on summon effect (this is always burst speed with no targets)
        if (card.onSummon != null)
        {
            SpellCard effect = card.onSummon;

            spellStack.Add(effect, activePlayer);
        }

        PassPriority();

        return true;
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

            //start the spell stack
            else
            {
                AddToSpellStack(card);
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
            AddToSpellStack(activeSpell);

            //play unit if spell came from its play effect
            if (bufferedUnit != null)
            {
                spellStack.playerWithFirstCast = activePlayer;
                if (activePlayer == 1)
                {
                    playerOneSide.PlayUnit(bufferedUnit);
                }
                else if (activePlayer == 2)
                {
                    playerTwoSide.PlayUnit(bufferedUnit);
                }
                bufferedUnit = null;
                PassPriority();
            }

            activeSpell = null;
            targeting = false;
        }
    }

    /// <summary>
    /// Assigns the proper target (nexus) for a spell and adds it to the stack if all targets are set.
    /// </summary>
    public void AssignTarget(Nexus target)
    {
        //exact same as above...
        activeSpell.AssignNextTarget(target);

        if (!activeSpell.NeedsTargets())
        {
            AddToSpellStack(activeSpell);

            //play unit if spell came from its play effect
            if (bufferedUnit != null)
            {
                spellStack.playerWithFirstCast = activePlayer;
                if (activePlayer == 1)
                {
                    playerOneSide.PlayUnit(bufferedUnit);
                }
                else if (activePlayer == 2)
                {
                    playerTwoSide.PlayUnit(bufferedUnit);
                }
                bufferedUnit = null;
                PassPriority();
            }

            activeSpell = null;
            targeting = false;
        }
    }

    public void AddToSpellStack(SpellCard spell)
    {
        spellStack.Add(spell, activePlayer);
        if (spell.spellType == SpellType.Fast || spell.spellType == SpellType.Slow)
        {
            casting = true;

            //if first spell on the stack, remember active player
            if (spellStack.spells.Count == 1)
            {
                spellStack.playerWithFirstCast = activePlayer;
            }
        }
    }

    /// <summary>
    /// Commits all casted spells.
    /// </summary>
    private void ConfirmSpellCasts()
    {
        casting = false;
        PassPriority();
    }

    /// <summary>
    /// Increments round number and updates state.
    /// </summary>
    private void AdvanceRound()
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
        if ((attackingUnits == null || attackingUnits.Count == 0) && battlefield.battlingUnits.Count == 0)
        {
            Debug.Log("Cannot declare attack with no units.");
            return;
        }

        //add attackers
        List<UnitCard> attackingUnitsCopy = new List<UnitCard>(attackingUnits);
        foreach (UnitCard unit in attackingUnitsCopy)
        {
            DeclareSingleAttack(unit);
        }

        ConfirmAttacks();
    }

    /// <summary>
    /// Commits a set of units to an attack.
    /// </summary>
    public void DeclareSingleAttack(UnitCard unit)
    {
        declaringAttacks = true;

        attackingPlayer = activePlayer;
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

        attackingBench.MoveToCombat(unit);
        battlefield.DeclareAttacker(unit);
    }

    /// <summary>
    /// Commits a set of blockers in response to an attack.
    /// </summary>
    public void DeclareBlock(List<Battlefield.BattlePair> blockPairs)
    {

        //assign blocker to attacker
        foreach (Battlefield.BattlePair pair in blockPairs)
        {
            DeclareSingleBlock(pair.attacker, pair.blocker);
        }

        ConfirmBlocks();
    }

    /// <summary>
    /// Assigns a blocker to an attacker, but doesn't commit it.
    /// </summary>
    public void DeclareSingleBlock(UnitCard attacker, UnitCard blocker)
    {
        declaringBlocks = true;

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

        defendingBench.MoveToCombat(blocker);
        battlefield.DeclareBlocker(blocker, attacker);
    }

    /// <summary>
    /// Sets an attacker to challenge a specific blocker in a pair.
    /// </summary>
    public void DeclareChallenge(UnitCard attacker, UnitCard blocker)
    {
        declaringAttacks = true;
        attackingPlayer = activePlayer;

        Bench attackingBench = null;
        Bench defendingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            attackingBench = playerOneSide.bench;
            defendingBench = playerTwoSide.bench;
        }
        else
        {
            attackingBench = playerTwoSide.bench;
            defendingBench = playerOneSide.bench;
        }

        //add attacker
        attackingBench.MoveToCombat(attacker);
        battlefield.DeclareAttacker(attacker);

        //add blocker
        defendingBench.MoveToCombat(blocker);
        battlefield.DeclareBlocker(blocker, attacker);
    }

    /// <summary>
    /// Commits all attacks.
    /// </summary>
    public void ConfirmAttacks()
    {

        //remove attack token
        attackingPlayer = activePlayer;
        if (attackingPlayer == 1)
        {
            playerOneSide.RemoveAttackToken();
            playerOneSide.isAttacking = true;
        }
        else
        {
            playerTwoSide.RemoveAttackToken();
            playerTwoSide.isAttacking = true;
        }

        //begin combat and pass priority
        declaringAttacks = false;
        //declaringBlocks = true;
        passCount = 1; //need this so combat ends if blocker pass
        SwitchActivePlayer();
        inCombat = true;
    }

    /// <summary>
    /// Commits all blocks.
    /// </summary>
    private void ConfirmBlocks()
    {
        declaringBlocks = false;
        passCount = 1; //need this so defending player doesn't get chance to respond after an attacker pass
        SwitchActivePlayer();
        blocked = true;
    }


    /// <summary>
    /// Assigns unit and nexus damage and exits combat.
    /// </summary>
    private void ResolveBattle()
    {
        LoRBoardSide attackingSide = null;
        LoRBoardSide defendingSide = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            attackingSide = playerOneSide;
            defendingSide = playerTwoSide;
        }
        else
        {
            attackingSide = playerTwoSide;
            defendingSide = playerOneSide;
        }

        //int totalNexusDamage = 0;

        //resolve pairs
        foreach (Battlefield.BattlePair pair in battlefield.battlingUnits)
        {
            if (pair.attacker != null && pair.attacker.name != "Dummy")
            {
                if (pair.blocker == null)
                {
                    //reduce nexus health
                    defendingSide.nexus.TakeDamage(pair.attacker.power);

                    //return to bench (should not happen if ephemeral)
                    attackingSide.bench.Add(pair.attacker);
                }
                else if (pair.blocker.name == "Dummy")
                {
                    //blocker is dead

                    //handle overwhelm damage
                    if (pair.attacker.HasKeyword(Keyword.Overwhelm))
                    {
                        defendingSide.nexus.TakeDamage(pair.attacker.power);
                    }

                    if (pair.attacker.health > 0)
                    {
                        attackingSide.bench.Add(pair.attacker);
                    }
                }
                else
                {
                    //resolve damage
                    pair.attacker.Strike(pair.blocker);

                    //blocker damage, handle quick attack and blocker killed
                    if (!pair.attacker.HasKeyword(Keyword.QuickAttack) || pair.blocker.health > 0)
                    {
                        pair.blocker.Strike(pair.attacker);
                    }

                    //handle overwhelm damage
                    if (pair.attacker.HasKeyword(Keyword.Overwhelm) && pair.blocker.health < 0)
                    {
                        defendingSide.nexus.TakeDamage(Math.Abs(pair.blocker.health)); //note - doesn't work with tough i think
                    }

                    if (pair.attacker.health > 0)
                    {
                        attackingSide.bench.Add(pair.attacker);
                    }
                    else
                    {
                        //Debug.Log("Player " + attackingPlayer + "'s " + pair.attacker.ToString() + " died in combat.");
                    }
                    if (pair.blocker.health > 0)
                    {
                        defendingSide.bench.Add(pair.blocker);
                    }
                    else
                    {
                        //Debug.Log("Player " + (3 - attackingPlayer) + "'s " + pair.blocker.ToString() + " died in combat.");
                    }
                }
            }
            else if (pair.blocker != null && pair.blocker.name != "Dummy")
            {
                defendingSide.bench.Add(pair.blocker);
            }
        }

        //Debug.Log("Player " + activePlayer + " took " + totalNexusDamage + " damage from combat.");

        CheckGameTermination();
        battlefield.ClearField();
        attackingSide.isAttacking = false;
        inCombat = false;
        blocked = false;

        passCount = 0;
        activePlayer = 3 - attackingPlayer;
        attackingPlayer = 0;
    }

    /// <summary>
    // Returns true if there are spells on the stack.
    /// </summary>
    public bool SpellsAreActive()
    {
        return spellStack.spells.Count > 0;
    }

    /// <summary>
    // Resolves all spells on the stack and gives turn to other player.
    /// </summary>
    public void ResolveSpellStack()
    {
        while (spellStack.spells.Count > 0)
        {
            spellStack.Resolve();
            CheckUnitDeath();
            CheckGameTermination();
        }
        spellStack.playerWithFirstCast = 0;
    }

    /// <summary>
    // Updates the board to eliminate all dead units.
    /// </summary>
    private void CheckUnitDeath()
    {
        playerOneSide.bench.CheckUnitDeath();
        playerTwoSide.bench.CheckUnitDeath();
        battlefield.CheckUnitDeath();
    }

    /// <summary>
    //Returns 1 if Player 1 won, 2 if Player 2 won, 0 if tie, and -1 if unterminated.
    /// </summary>
    public void CheckGameTermination()
    {
        //game already decided
        if (gameResult != -1) return;

        //win by nexus health
        if (playerOneSide.nexus.health <= 0 && playerTwoSide.nexus.health <= 0)
        {
            gameResult = 0;
        }
        else if (playerOneSide.nexus.health <= 0)
        {
            gameResult = 2;
        }
        else if (playerTwoSide.nexus.health <= 0)
        {
            gameResult = 1;
        }

        //if decking is being ignored (usually for testing)
        if (deckTerminationDisabled) return;

        //win by decking (out of cards)
        else if (playerOneSide.deck.cards.Count == 0 && playerTwoSide.deck.cards.Count == 0)
        {
            gameResult = 0;
        }
        else if (playerOneSide.deck.cards.Count == 0)
        {
            gameResult = 2;
        }
        else if (playerTwoSide.deck.cards.Count == 0)
        {
            gameResult = 1;
        }
    }

    /// <summary>
    /// Passes player priority (no action).
    /// </summary>
    public void Pass()
    {

        if (declaringAttacks)
        {
            if (casting) //spells can be cast alongside an attack
            {
                ConfirmSpellCasts();
                PassPriority(); //since confirmation passes twice
            }
            ConfirmAttacks();
            return;
        }

        if (declaringBlocks)
        {
            if (casting) //spells can be cast alongside an block
            {
                ConfirmSpellCasts();
                PassPriority(); //since confirmation passes twice
            }
            ConfirmBlocks(); //if no blocks, how to detect?
            return;
        }

        if (casting)
        {
            ConfirmSpellCasts();
            return;
        }

        if (inCombat)
        {
            if (SpellsAreActive())
            {
                if (passCount == 1)
                {
                    ResolveSpellStack();
                    ResolveBattle();
                    //Debug.Log("Resolving all spells and ending combat...");
                }
                else if (passCount == 0)
                {
                    passCount += 1;
                    SwitchActivePlayer();
                    //Debug.Log("Passing priority...");
                }
            }
            else
            {
                if (passCount == 1)
                {
                    ResolveBattle();
                    //Debug.Log("Ending combat...");
                }
                else if (passCount == 0)
                {
                    passCount += 1;
                    SwitchActivePlayer();
                    //Debug.Log("Passing priority...");
                }
                //Debug.Log("Ending combat...");
            }
            return;
        }

        if (SpellsAreActive())
        {
            passCount = 0; //resolving spell stack does not count as pass for turn
            activePlayer = 3 - spellStack.playerWithFirstCast;
            ResolveSpellStack();
            //Debug.Log("Resolving all spells...");
            return;
        }

        passCount += 1;
        SwitchActivePlayer();

        if (passCount == 2)
        {
            AdvanceRound();
        }
    }

    /// <summary>
    /// Changes active player to the other player and resets pass count.
    /// </summary>
    private void PassPriority()
    {
        passCount = 0;
        activePlayer = 3 - activePlayer;
    }

    /// <summary>
    /// Changes active player to the other player.
    /// </summary>
    private void SwitchActivePlayer()
    {
        activePlayer = 3 - activePlayer;
    }
}
