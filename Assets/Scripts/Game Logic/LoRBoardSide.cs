using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoRBoardSide
{
    public Hand hand;
    public Bench bench;
    public Mana mana;
    public Deck deck;
    public bool hasAttackToken;
    public Nexus nexus;

    int playerNumber;

    public LoRBoardSide(int playerNumber)
    {
        hand = new Hand();
        bench = new Bench();
        mana = new Mana();
        nexus = new Nexus();
        this.playerNumber = playerNumber;
    }

    public void SetDeck(Deck newDeck)
    {
        deck = newDeck;
        deck.Shuffle();
    }

    public void Initialize()
    {
        if (playerNumber == 1)
        {
            hasAttackToken = true;
        }
    }

    public void Draw()
    {
        hand.Add(deck.Draw());
    }

    public bool PlayUnit(Card card)
    {
        if (bench.IsFull())
        {
            Debug.Log(card.name + " cannot be played to a full bench.");
            return false;
        }

        if (mana.manaGems < card.cost)
        {
            Debug.Log("Not enough mana to cast " + card.name);
            return false;
        }

        hand.Play(card);
        bench.Play((UnitCard)card);
        mana.manaGems -= card.cost;

        return true;
    }

    public void UpdateRound(int roundNumber)
    {

        hasAttackToken = 2 - (roundNumber % 2) == playerNumber;

        //refill mana
        mana.spellMana = Math.Min(mana.manaGems, 3);
        mana.maxMana = Math.Min(mana.maxMana + 1, 10);
        mana.manaGems = mana.maxMana;

        //draw for turn
        Card drawnCard = deck.Draw();
        if (drawnCard != null)
        {
            if (!hand.Add(drawnCard))
            {
                //discard card if hand full
            }
        }

        //activate round started effects
    }

    public void removeAttackToken()
    {
        hasAttackToken = false;
    }
}