using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string name;
    public Region region;
    public int cost;
    public Card() { }
    public Card(string name, int cost)
    {
        this.name = name;
        this.cost = cost;
    }
    public override string ToString()
    {
        return name;
    }

    public virtual void Revert()
    {

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