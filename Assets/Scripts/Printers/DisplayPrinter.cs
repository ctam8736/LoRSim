using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class DisplayPrinter : Printer
{
    public List<Sprite> cardImages;

    // Start is called before the first frame update
    void Start()
    {
        FindTextReferences();

        InitializeDictionary();

        playerADeck = LoadDeckFromJson("Assets/Decks/spelltest3.json");
        playerBDeck = LoadDeckFromJson("Assets/Decks/spelltest3.json");

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
}