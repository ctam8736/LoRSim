using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoRBoard
{
    /**
    public Hand playerOneHand = new Hand();
    public Hand playerTwoHand = new Hand();
    public Bench playerOneBench = new Bench();
    public Bench playerTwoBench = new Bench();
    public Mana playerOneMana = new Mana();
    public Mana playerTwoMana = new Mana();
    public Deck playerOneDeck;
    public Deck playerTwoDeck;
    public bool playerOneAttackToken;
    public bool playerTwoAttackToken;
    public Nexus playerOneNexus = new Nexus();
    public Nexus playerTwoNexus = new Nexus();
    **/

    public LoRBoardSide playerOneSide = new LoRBoardSide(1);
    public LoRBoardSide playerTwoSide = new LoRBoardSide(2);

    //---

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
            switchActivePlayer();
        }

        return succeeded;
    }

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
        switchActivePlayer();
        inCombat = true;
    }

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

    public void ConfirmBlocks()
    {
        blocked = true;
        switchActivePlayer();
    }

    public void resolveBattle()
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

    public void Pass()
    {
        if (inCombat)
        {
            resolveBattle();
        }
        else
        {
            passCount += 1;
            switchActivePlayer();

            if (passCount == 2)
            {
                AdvanceRound();
            }
        }
    }

    public void switchActivePlayer()
    {
        activePlayer = 3 - activePlayer;
    }

    public string ToString()
    {
        string boardString = "";
        boardString += "Round Number: " + roundNumber + "\n\n";
        boardString += "Player One Hand: \n" + playerOneSide.hand.ToString();
        boardString += "Player Two Hand: \n" + playerTwoSide.hand.ToString();
        return boardString;
    }
}
