using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// A LoRBoardSide handles one player's half of the LoRBoard.
/// </summary>
public class LoRBoardSide
{
    public Hand hand;
    public Bench bench;
    public Mana mana;
    public Deck deck;
    public bool hasAttackToken;
    public Nexus nexus;

    int playerNumber;
    public LoRBoardSide opposingSide;
    public bool isAttacking;

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
        Card draw = deck.Draw();
        if (draw != null)
        {
            hand.Add(draw);
        }
    }

    public bool CanPlayUnit(UnitCard card)
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

        return true;
    }

    /// <summary>
    /// Plays a unit card from the this player's hand to the bench.
    /// </summary>
    public bool PlayUnit(UnitCard card)
    {

        hand.Play(card);
        bench.Play(card);
        mana.manaGems -= card.cost;

        //card.TriggerPlayEffect();
        //card.TriggerSummonEffect();

        return true;
    }

    /// <summary>
    /// Plays a spell card from the this player's hand.
    /// </summary>
    public bool PlaySpell(SpellCard card)
    {
        if (mana.manaGems + mana.spellMana < card.cost)
        {
            Debug.Log("Not enough mana to cast " + card.name);
            return false;
        }

        hand.Play(card);

        int spellCost = card.cost;

        //pre-assign targets with only one value
        if (card.NeedsTargets())
        {
            if (card.nextTargetType == TargetType.AlliedNexus)
            {
                card.AssignNextTarget(nexus);
            }
            else if (card.nextTargetType == TargetType.EnemyNexus)
            {
                card.AssignNextTarget(opposingSide.nexus);
            }
        }

        //use spell mana first, then mana gems
        if (mana.spellMana > spellCost)
        {
            mana.spellMana -= spellCost;
            return true;
        }
        else
        {
            spellCost -= mana.spellMana;
            mana.spellMana = 0;
            mana.manaGems -= spellCost;
            return true;
        }
    }

    /// <summary>
    /// Updates player's mana and attack token, and draws a card for the round.
    /// </summary>
    public void UpdateRound(int roundNumber)
    {
        bench.RevertRoundBuffs();
        //bench.TriggerRoundEndEffects();

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

    /// <summary>
    /// Removes this player's attack token.
    /// </summary>
    public void GainAttackToken()
    {
        hasAttackToken = true;
    }

    /// <summary>
    /// Removes this player's attack token.
    /// </summary>
    public void RemoveAttackToken()
    {
        hasAttackToken = false;
    }
}