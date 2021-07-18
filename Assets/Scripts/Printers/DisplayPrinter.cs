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

    public void FillImageDictionary()
    {
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
            boardDisplay.transform.Find("Canvas").Find("Player One Hand").Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = cardSprite;
            cardIndex += 1;
        }

        while (cardIndex <= 10)
        {
            boardDisplay.transform.Find("Canvas").Find("Player One Hand").Find("Card " + cardIndex).gameObject.GetComponent<Image>().sprite = nullImage;
            cardIndex += 1;
        }
    }

    void UpdatePlayerOneBench()
    {

    }

    void UpdatePlayerOneBattlefield()
    {

    }

    void UpdatePlayerTwoHand()
    {

    }

    void UpdatePlayerTwoBench()
    {

    }

    void UpdatePlayerTwoBattlefield()
    {

    }
}