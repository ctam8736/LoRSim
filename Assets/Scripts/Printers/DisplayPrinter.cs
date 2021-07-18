using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayPrinter : Printer
{
    public List<Sprite> cardImages;
    public Dictionary<string, string> imageDictionary = new Dictionary<string, string>();
    public GameObject boardDisplay;
    public Sprite nullImage;

    public TextMeshProUGUI playerOneNexusHealthText;
    public TextMeshProUGUI playerTwoNexusHealthText;

    public void FillImageDictionary()
    {
        imageDictionary.Add("Vanguard Defender", "01DE020");
        imageDictionary.Add("Cithria of Cloudfield", "01DE039");
    }

    // Start is called before the first frame update
    void Start()
    {
        FindTextReferences();

        InitializeDictionary();
        FillImageDictionary();

        playerADeck = LoadDeckFromJson("Assets/Decks/cithria.json");
        playerBDeck = LoadDeckFromJson("Assets/Decks/cithria.json");

        ResetGame(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (showRounds)
        {
            timer += Time.deltaTime;
            if (timer > turnDelay)
            {
                timer = timer % turnDelay;
                HandleGameEnd();
                PlayGameTurn();
                UpdateText();
                PlotData();
            }
        }
        else
        {
            while (!HandleGameEnd())
            {
                PlayGameTurn();
            }
            UpdateText();
            PlotData();
        }
    }

    //Creates a data point reflecting results.
    void PlotData()
    {
        float score = (results[1] * 100f + results[0] * 50f) / playedGamesInMatch;
        if (!float.IsNaN(score))
        {
            plotter.createDataPoint(playedGamesInMatch - 500, score, finishedMatches * 10);
        }
    }

    void UpdateText()
    {
        base.UpdateText();
        playerOneNexusHealthText.text = "" + board.playerOneSide.nexus.health;
        playerTwoNexusHealthText.text = "" + board.playerTwoSide.nexus.health;
        UpdateDisplayBoard();
    }

    void UpdateDisplayBoard()
    {
        UpdatePlayerOneHand();
        UpdatePlayerOneBench();
        UpdatePlayerOneBattlefield();
        UpdatePlayerTwoHand();
        UpdatePlayerTwoBench();
        UpdatePlayerTwoBattlefield();
    }

    void UpdatePlayerOneHand()
    {
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerOneSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    Debug.Log("Image found.");
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBench()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Bench");
        int cardIndex = 1;
        foreach (Card card in board.playerOneSide.bench.units)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    Debug.Log("Image found.");
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player One Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            Card card;
            if (board.attackingPlayer == 1)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            Sprite cardSprite = null;
            if (card != null)
            {
                foreach (Sprite sprite in cardImages)
                {
                    if (sprite.name.Equals(imageDictionary[card.name]))
                    {
                        Debug.Log("Image found.");
                        cardSprite = sprite;
                    }
                }
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
                cardIndex += 1;
            }
            else
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            }
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoHand()
    {
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Hand");
        int numSlots = 10;
        int cardIndex = 1;
        foreach (Card card in board.playerTwoSide.hand.cards)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    Debug.Log("Image found.");
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoBench()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Bench");
        int cardIndex = 1;
        foreach (Card card in board.playerTwoSide.bench.units)
        {
            Sprite cardSprite = null;
            foreach (Sprite sprite in cardImages)
            {
                if (sprite.name.Equals(imageDictionary[card.name]))
                {
                    Debug.Log("Image found.");
                    cardSprite = sprite;
                }
            }
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerTwoBattlefield()
    {
        int numSlots = 6;
        Transform cardRegion = boardDisplay.transform.Find("Canvas").Find("Player Two Battlefield");
        int cardIndex = 1;

        foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
        {
            Card card;
            if (board.attackingPlayer == 2)
            {
                card = pair.attacker;
            }
            else
            {
                card = pair.blocker;
            }
            Sprite cardSprite = null;
            if (card != null)
            {
                foreach (Sprite sprite in cardImages)
                {
                    if (sprite.name.Equals(imageDictionary[card.name]))
                    {
                        Debug.Log("Image found.");
                        cardSprite = sprite;
                    }
                }
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
                cardIndex += 1;
            }
            else
            {
                cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            }
        }
        while (cardIndex <= numSlots)
        {
            cardRegion.Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }
}