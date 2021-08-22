using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A Card represents any single card in the game, including units, spells, and landmarks.
/// </summary>
public class Card
{
    public string name;
    public Region region;
    public int initialCost;
    public int cost;
    public Card() { }
    public Card(string name, int cost)
    {
        this.name = name;
        this.initialCost = cost;
        this.cost = initialCost;
    }
    public override string ToString()
    {
        return name;
    }

    public virtual void Revert()
    {

    }

    public void ReduceCost(int i)
    {
        cost = Math.Max(cost - i, 0);
    }

    public static Card CopyCard(Card card)
    {
        if (card == null) return null;
        Card newCard = new Card(card.name, card.cost);
        return newCard;
    }
}

public enum Region
{
    Bilgewater,
    Demacia,
    Freljord,
    Ionia,
    Noxus,
    Null,
    PnZ,
    ShadowIsles,
    Shurima,
    Targon
}