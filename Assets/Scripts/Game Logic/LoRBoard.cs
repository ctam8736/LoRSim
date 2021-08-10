using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoRBoard
{

    public LoRBoardSide playerOneSide = new LoRBoardSide(1);
    public LoRBoardSide playerTwoSide = new LoRBoardSide(2);

    public Battlefield battlefield = new Battlefield();
    public SpellStack spellStack;
    public int roundNumber;
    public int activePlayer;
    public int passCount;
    public int attackingPlayer;
    public bool inCombat;
    public bool blocked;

    public SpellCard activeSpell;
    public UnitCard bufferedUnit;
    public bool targeting;
    public int gameResult = -1;

    /// <summary>
    /// Sets up decks and draws starting hands (todo: mulligan state).
    /// </summary>
    public void Initialize(Deck playerOneDeck, Deck playerTwoDeck)
    {
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

        if (activePlayer == 1 && !playerOneSide.CanPlayUnit(card)) return false;
        if (activePlayer == 2 && !playerTwoSide.CanPlayUnit(card)) return false;

        if (card.onPlay != null)
        {
            SpellCard effect = card.onPlay;
            passCount = 0;

            bool skipTargets = false;
            //a spell is on the stack that needs targets
            if (effect.NeedsTargets())
            {
                if (effect.nextTargetType == TargetType.AlliedUnit)
                {
                    if (activePlayer == 1 && playerOneSide.bench.units.Count == 0)
                    {
                        Debug.Log("targeting skipped");
                        skipTargets = true;
                    }
                    if (activePlayer == 2 && playerTwoSide.bench.units.Count == 0)
                    {
                        Debug.Log("targeting skipped");
                        skipTargets = true;
                    }
                }

                if (!skipTargets)
                {
                    Debug.Log(effect.name + " is active...");
                    activeSpell = effect;
                    targeting = true;
                    bufferedUnit = card;
                    return true;
                }
            }

            if (!skipTargets)
            {
                //pass priority if fast or slow
                if (effect.spellType != SpellType.Burst)
                {
                    if (spellStack.spells.Count == 0)
                    {
                        spellStack.playerWithFirstCast = activePlayer;
                    }
                }
                spellStack.Add(effect, activePlayer);
            }
        }

        if (activePlayer == 1)
        {
            playerOneSide.PlayUnit(card);
        }
        else if (activePlayer == 2)
        {
            playerTwoSide.PlayUnit(card);
        }

        if (card.onSummon != null)
        {
            SpellCard effect = card.onSummon;

            spellStack.Add(effect, activePlayer);
        }

        passCount = 0;
        SwitchActivePlayer();

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

            //pass priority if fast or slow
            else if (card.spellType != SpellType.Burst && card.spellType != SpellType.Focus)
            {
                if (spellStack.spells.Count == 0)
                {
                    spellStack.playerWithFirstCast = activePlayer;
                }
                spellStack.Add(card, activePlayer);

                if (card.spellType == SpellType.Slow)
                {
                    SwitchActivePlayer();
                }
            }
            else
            {
                spellStack.Add(card, activePlayer);
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
            spellStack.Add(activeSpell, activePlayer);

            //was the result of a unit play
            if (bufferedUnit != null)
            {
                spellStack.playerWithFirstCast = activePlayer;
                if (activePlayer == 1)
                {
                    playerOneSide.PlayUnit(bufferedUnit);
                    passCount = 0;
                    SwitchActivePlayer();
                }
                else if (activePlayer == 2)
                {
                    playerTwoSide.PlayUnit(bufferedUnit);
                    passCount = 0;
                    SwitchActivePlayer();
                }
            }
            else if (activeSpell.spellType != SpellType.Burst && activeSpell.spellType != SpellType.Focus)
            {
                if (spellStack.spells.Count == 1)
                {
                    spellStack.playerWithFirstCast = activePlayer;
                }
                if (activeSpell.spellType == SpellType.Slow)
                {
                    SwitchActivePlayer();
                }
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
        activeSpell.AssignNextTarget(target);

        if (!activeSpell.NeedsTargets())
        {
            spellStack.Add(activeSpell, activePlayer);

            //was the result of a unit play
            if (bufferedUnit != null)
            {
                spellStack.playerWithFirstCast = activePlayer;
                if (activePlayer == 1)
                {
                    playerOneSide.PlayUnit(bufferedUnit);
                    passCount = 0;
                    SwitchActivePlayer();
                }
                else if (activePlayer == 2)
                {
                    playerTwoSide.PlayUnit(bufferedUnit);
                    passCount = 0;
                    SwitchActivePlayer();
                }
            }
            else if (activeSpell.spellType != SpellType.Burst && activeSpell.spellType != SpellType.Focus)
            {
                if (spellStack.spells.Count == 1)
                {
                    spellStack.playerWithFirstCast = activePlayer;
                }
                if (activeSpell.spellType == SpellType.Slow)
                {
                    SwitchActivePlayer();
                }
            }

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
        if ((attackingUnits == null || attackingUnits.Count == 0) && battlefield.battlingUnits.Count == 0)
        {
            Debug.Log("Cannot declare attack with no units.");
            return;
        }

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

        ConfirmAttacks();
    }

    /// <summary>
    /// Sets an attacker to challenge a specific blocker in a pair.
    /// </summary>
    public void DeclareChallenge(Battlefield.BattlePair pair)
    {
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
        attackingBench.MoveToCombat(pair.attacker);
        battlefield.DeclareAttacker(pair.attacker);

        //add blocker
        defendingBench.MoveToCombat(pair.blocker);
        battlefield.DeclareBlocker(pair.blocker, pair.attacker);
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
    /// Commits all attacks.
    /// </summary>
    public void ConfirmAttacks()
    {
        passCount = 0;
        SwitchActivePlayer();
        inCombat = true;
    }

    /// <summary>
    /// Commits all blocks.
    /// </summary>
    public void ConfirmBlocks()
    {
        blocked = true;
        passCount = 1; //need this so defending player doesn't get chance to respond after an attacker pass
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
                    defendingNexus.TakeDamage(pair.attacker.power);
                    //totalNexusDamage += pair.attacker.power;
                    attackingBench.Add(pair.attacker);
                }
                else if (pair.blocker.name == "Dummy")
                {
                    //blocker is dead

                    //handle overwhelm damage
                    if (pair.attacker.HasKeyword(Keyword.Overwhelm))
                    {
                        defendingNexus.TakeDamage(pair.attacker.power);
                    }

                    if (pair.attacker.health > 0)
                    {
                        attackingBench.Add(pair.attacker);
                    }
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
                        defendingNexus.TakeDamage(Math.Abs(pair.blocker.health));
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
            else if (pair.blocker != null)
            {
                defendingBench.Add(pair.blocker);
            }
        }

        //Debug.Log("Player " + activePlayer + " took " + totalNexusDamage + " damage from combat.");

        CheckGameTermination();
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

    public void CheckUnitDeath()
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

        //win by decking
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
        /**
        if (!blocked || passCount == 1) //no blocks or a player has passed
        {
            if (SpellsAreActive())
            {
                ResolveSpellStack();
            }
            ResolveBattle();
        }
        else
        {
            passCount += 1;
            SwitchActivePlayer();
        }
        **/

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
                ResolveBattle();
                //Debug.Log("Ending combat...");
            }
        }

        else if (SpellsAreActive())
        {
            passCount = 0; //resolving spell stack does not count as pass for turn
            activePlayer = 3 - spellStack.playerWithFirstCast;
            ResolveSpellStack();
            //Debug.Log("Resolving all spells...");
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
