﻿using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// A Deck represents a collection of Cards that can be reused by a single player across games.
/// </summary>
public class Deck
{
    public Dictionary<string, int> cardCounts = new Dictionary<string, int>();
    public List<Card> deckList;
    public List<Card> cards;
    public string name;
    private static System.Random rng = new System.Random();

    public Deck(string name, List<Card> cards, bool shuffle = true)
    {
        this.name = name;
        this.deckList = cards;
        foreach (Card card in cards)
        {
            if (cardCounts.ContainsKey(card.name))
            {
                cardCounts[card.name] += 1;
            }
            else
            {
                cardCounts[card.name] = 1;
            }
        }
        this.cards = new List<Card>(deckList);
        if (shuffle) { Shuffle(); }
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
        //Debug.Log("Deck is out of cards.");
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

        //don't add a card that's already a 3-of
        while (cardCounts.ContainsKey(chosenCard.name) && cardCounts[chosenCard.name] > 2)
        {
            chosenCard = cardPool[rng.Next(cardPool.Count)];
        }
        //increment card count
        if (cardCounts.ContainsKey(chosenCard.name))
        {
            cardCounts[chosenCard.name] += 1;
        }
        else
        {
            cardCounts[chosenCard.name] = 1;
        }

        int randIndex = rng.Next(40);

        //don't replace the same card
        while (deckList[randIndex].name == chosenCard.name)
        {
            randIndex = rng.Next(40);
        }
        //remove from card count
        cardCounts[deckList[randIndex].name] -= 1;

        if (chosenCard is UnitCard)
        {
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

    /// <summary>
    /// Returns a JSON representation of the deck.
    /// </summary>
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

    public override string ToString()
    {
        string deckString = "Deck " + name + "'s decklist: \n";
        foreach (Card card in deckList)
        {
            deckString += card.ToString() + "\n";
        }
        return deckString;
    }
}
