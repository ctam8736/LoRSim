using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{
    public string name;
    public int cost;
    public Card() { }
    public Card(string name, int cost)
    {
        this.name = name;
        this.cost = cost;
    }
    public string ToString()
    {
        return name;
    }

    public virtual void Revert()
    {

    }

    public static Card CopyCard(Card card)
    {
        Card newCard = new Card(card.name, card.cost);
        return newCard;
    }
}
