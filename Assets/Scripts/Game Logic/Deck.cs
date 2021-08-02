using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class Deck
{

    public List<Card> deckList;
    public List<Card> cards;
    public string name;
    private static System.Random rng = new System.Random();

    public Deck(string name, List<Card> cards)
    {
        this.name = name;
        this.deckList = cards;
        this.cards = new List<Card>(deckList);
        Shuffle();
    }

    //shuffle method from grenade https://stackoverflow.com/questions/273313/randomize-a-listt
    public void Shuffle()
    {
        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = cards[k];
            cards[k] = cards[n];
            cards[n] = value;
        }
    }

    /// <summary>
    /// Removes and returns the top card of the deck.
    /// </summary>
    public Card Draw()
    {
        if (cards.Count > 0)
        {
            Card cardDrawn = cards[0];
            cards.RemoveAt(0);
            return cardDrawn;
        }
        Debug.Log("Deck is out of cards.");
        return null;
    }

    /// <summary>
    /// Resets the deck's card list to the original deck list.
    /// </summary>
    public void Reset()
    {
        cards = new List<Card>(deckList);
        foreach (Card card in cards)
        {
            card.Revert();
        }
        Shuffle();
    }

    /// <summary>
    /// Replaces a random card in the deck with a random card from the card pool.
    /// </summary>
    public void RandomMutate(List<Card> cardPool)
    {
        Card chosenCard = cardPool[rng.Next(cardPool.Count)];
        int randIndex = rng.Next(40);

        if (chosenCard is UnitCard)
        {
            //Debug.Log(deckList[randIndex].name + " was replaced by " + chosenCard.name + ".");
            deckList[randIndex] = UnitCard.CopyCard((UnitCard)chosenCard);
        }
        else if (chosenCard is SpellCard)
        {
            deckList[randIndex] = SpellCard.CopyCard((SpellCard)chosenCard);
        }
    }

    /// <summary>
    /// Returns an exact copy of the original deck.
    /// </summary>
    public static Deck CopyDeck(Deck deck)
    {
        List<Card> newCards = new List<Card>(deck.deckList);
        Deck newDeck = new Deck(deck.name, newCards);
        return newDeck;
    }

    /// <summary>
    /// Creates a random deck from the given card pool.
    /// </summary>
    public static Deck RandomDeck(string name, List<Card> cardPool)
    {
        List<Card> newCards = new List<Card>();
        for (int i = 0; i < 40; i++)
        {
            Card newCard = cardPool[rng.Next(cardPool.Count)];
            if (newCard is UnitCard)
            {
                newCards.Add(UnitCard.CopyCard((UnitCard)newCard));
            }
            else if (newCard is SpellCard)
            {
                newCards.Add(SpellCard.CopyCard((SpellCard)newCard));
            }
        }
        return new Deck(name, newCards);
    }

    public override string ToString()
    {
        string deckString = "Deck " + name + "'s decklist: \n";
        foreach (Card card in deckList)
        {
            deckString += card.ToString() + "\n";
        }
        return deckString;
    }

    public string ToJSON()
    {
        string deckJSON = "{\n\"name\": \"" + name + "\",\n\"cards\": [\n";
        foreach (Card card in deckList.OrderBy(x => x.name))
        {
            deckJSON += "\"" + card.name + "\",\n";
        }
        deckJSON += "]\n}";
        return deckJSON;
    }
}
