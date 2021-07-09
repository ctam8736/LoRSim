using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand
{
    public List<Card> cards;
    int maxHandSize = 10;

    public Hand()
    {
        cards = new List<Card>();
    }

    public bool Add(Card card)
    {
        if (cards.Count < maxHandSize)
        {
            cards.Add(card);
            return true;
        }
        else
        {
            //handle discard
            return false;
        }
    }

    public void Play(Card card)
    {
        cards.RemoveAt(cards.FindIndex(0, cards.Count, x => x == card));
    }

    public string ToString()
    {
        string handString = "";
        foreach (Card card in cards)
        {
            handString += card.ToString() + "\n";
        }
        return handString;
    }

}