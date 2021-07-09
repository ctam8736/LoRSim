using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoRBoard3
{
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

    //public LoRBoardSide playerOneSide = new LoRBoardSide();
    //public LoRBoardSide playerTwoSide = new LoRBoardSide();

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
        //init board state variables
        this.playerOneDeck = playerOneDeck;
        this.playerTwoDeck = playerTwoDeck;
        this.playerOneDeck.Shuffle();
        this.playerTwoDeck.Shuffle();

        playerOneAttackToken = true;
        playerTwoAttackToken = false;

        passCount = 0;
        attackingPlayer = 0;

        //each player draws four
        for (int i = 0; i < 4; i++)
        {
            playerOneHand.Add(playerOneDeck.Draw());
            playerTwoHand.Add(playerTwoDeck.Draw());
        }

        roundNumber = 0;
        AdvanceRound();
    }

    public bool PlayUnit(Card card)
    {
        if (activePlayer == 1)
        {
            if (playerOneBench.IsFull())
            {
                Debug.Log(card.name + " cannot be played to a full bench.");
                return false;
            }
            if (playerOneMana.manaGems < card.cost)
            {
                Debug.Log("Not enough mana to cast " + card.name);
                return false;
            }
            playerOneHand.Play(card);
            playerOneBench.Play((UnitCard)card);
            playerOneMana.manaGems -= card.cost;

            passCount = 0;
            switchActivePlayer();
            return true;
        }
        else
        {
            if (playerTwoBench.IsFull())
            {
                Debug.Log(card.name + " cannot be played to a full bench.");
                return false;
            }
            if (playerTwoMana.manaGems < card.cost)
            {
                Debug.Log("Not enough mana to cast " + card.name);
                return false;
            }
            playerTwoHand.Play(card);
            playerTwoBench.Play((UnitCard)card);
            playerTwoMana.manaGems -= card.cost;

            passCount = 0;
            switchActivePlayer();
            return true;
        }
    }

    public void AdvanceRound()
    {
        roundNumber += 1;
        //Debug.Log("Advanced to round " + roundNumber + ".");

        playerOneAttackToken = (roundNumber % 2 == 1);
        playerTwoAttackToken = !playerOneAttackToken;

        if (playerOneAttackToken)
        {
            activePlayer = 1;
        }
        else
        {
            activePlayer = 2;
        }

        //refill mana
        playerOneMana.spellMana = Math.Min(playerOneMana.manaGems, 3);
        playerTwoMana.spellMana = Math.Min(playerTwoMana.manaGems, 3);
        playerOneMana.maxMana = Math.Min(playerOneMana.maxMana + 1, 10);
        playerTwoMana.maxMana = Math.Min(playerTwoMana.maxMana + 1, 10);
        playerOneMana.manaGems = playerOneMana.maxMana;
        playerTwoMana.manaGems = playerTwoMana.maxMana;

        //draw for turn
        Card playerOneDraw = playerOneDeck.Draw();
        if (playerOneDraw != null)
        {
            if (!playerOneHand.Add(playerOneDraw))
            {
                //discard card if hand full
                //Debug.Log(playerOneDraw.ToString() + " was discarded by player 1");
            }
        }
        Card playerTwoDraw = playerTwoDeck.Draw();
        if (playerTwoDraw != null)
        {
            if (!playerTwoHand.Add(playerTwoDraw))
            {
                //discard card if hand full
                //Debug.Log(playerTwoDraw.ToString() + " was discarded by player 2");
            }
        }

        //activate round start effects

        passCount = 0;
    }

    public void DeclareAttack(List<UnitCard> attackingUnits)
    {
        attackingPlayer = activePlayer;

        if (attackingPlayer == 1)
        {
            playerOneAttackToken = false;
        }
        else
        {
            playerTwoAttackToken = false;
        }

        Bench attackingBench = null;

        //assign variables
        if (attackingPlayer == 1)
        {
            attackingBench = playerOneBench;
        }
        else
        {
            attackingBench = playerTwoBench;
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
            defendingBench = playerTwoBench;
        }
        else
        {
            defendingBench = playerOneBench;
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
            defendingBench = playerTwoBench;
        }
        else
        {
            defendingBench = playerOneBench;
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
            defendingNexus = playerTwoNexus;
            attackingBench = playerOneBench;
            defendingBench = playerTwoBench;
        }
        else
        {
            defendingNexus = playerOneNexus;
            attackingBench = playerTwoBench;
            defendingBench = playerOneBench;
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
        boardString += "Player One Hand: \n" + playerOneHand.ToString();
        boardString += "Player Two Hand: \n" + playerTwoHand.ToString();
        return boardString;
    }
}
