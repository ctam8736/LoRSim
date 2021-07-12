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

    public class SpellStack
    {

    }

    //Sets up decks and draws starting hands (todo: mulligan state).
    public void Initialize(Deck playerOneDeck, Deck playerTwoDeck)
    {
        playerOneSide.SetDeck(playerOneDeck);
        playerTwoSide.SetDeck(playerTwoDeck);

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

    //Plays a unit card from the active player's hand to the bench.
    public bool PlayUnit(Card card)
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

    //Increments round number and updates state.
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

    //Commits a set of units to an attack.
    public void DeclareAttack(List<UnitCard> attackingUnits)
    {
        attackingPlayer = activePlayer;

        if (attackingPlayer == 1)
        {
            playerOneSide.removeAttackToken();
        }
        else
        {
            playerTwoSide.removeAttackToken();
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

    //Commits a set of blockers in response to an attack.
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

    //Assigns a blocker to an attacker, but doesn't commit.
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

    //Commits blocks.
    public void ConfirmBlocks()
    {
        blocked = true;
        SwitchActivePlayer();
    }

    //Assigns unit and nexus damage and exits combat.
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

        int totalNexusDamage = 0;

        //resolve pairs
        foreach (Battlefield.BattlePair pair in battlefield.battlingUnits)
        {
            if (pair.attacker != null)
            {
                if (pair.blocker == null)
                {
                    //reduce nexus health
                    defendingNexus.health -= pair.attacker.power;
                    totalNexusDamage += pair.attacker.power;
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
                        defendingNexus.health -= Math.Abs(pair.blocker.health);
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
    }

    //Passes player activity (no action).
    public void Pass()
    {
        if (inCombat)
        {
            ResolveBattle();
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

    //Changes active player to the other player.
    public void SwitchActivePlayer()
    {
        activePlayer = 3 - activePlayer;
    }
}
