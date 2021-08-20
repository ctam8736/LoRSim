using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HillPrinter : DisplayPrinter
{
    public bool mutation;
    public float bestOutcome = 0f;
    public string writeFile = "Assets/test.txt";
    Deck bestDeck;

    public int numberOfMutations = 1;

    protected List<Card> mutateCardPool;

    void Start()
    {

        FindTextReferences();
        FindDisplayReferences();

        InitializeDictionary();

        if (mutation)
        {
            mutateCardPool = new List<Card>(cardPool);
            mutateCardPool.RemoveAll(card => card.name == "Stand Alone");
            mutateCardPool.RemoveAll(card => card.name == "Radiant Strike");
            playerADeck = CardData.LoadDeckFromJson(playerADeckString); //deck to mutate
            playerBDeck = CardData.LoadDeckFromJson(playerBDeckString); //deck to beat (fitness)
            bestDeck = Deck.CopyDeck(playerADeck);

            for (int i = 0; i < numberOfMutations; i++)
            {
                playerADeck.RandomMutate(mutateCardPool);
            }
        }
        else
        {
            playerADeck = CardData.LoadDeckFromJson("Assets/Decks/test.json");
            playerBDeck = CardData.LoadDeckFromJson("Assets/Decks/test.json");
        }

        ResetGame(true);
        UpdateText();
    }

    protected override void InitializeDictionary()
    {
        cardPool = ConvertToList(CardData.cardDictionary.Values);
    }

    public override void WriteEndGameOutput()
    {
    }

    protected override void ResetGame(bool start = false)
    {

        StreamWriter writer;


        playerAGoingFirst = !playerAGoingFirst; //swap player turn for even matches

        if (!start)
        {
            playedGamesInMatch += 1;
        }



        if (playedGamesInMatch >= numberOfGamesInMatch) //match finished
        {
            if (mutation)
            {
                float score = (results[1] * 100f + results[0] * 50f) / numberOfGamesInMatch;

                writer = new StreamWriter(writeFile, true);
                writer.Write("\n" + score);
                writer.Close();

                if (score > bestOutcome)
                {
                    Debug.Log("Mutation successful with score: " + score);
                    Debug.Log(playerADeck.ToJSON());
                    bestDeck = Deck.CopyDeck(playerADeck);
                    bestOutcome = score;
                }
                else
                {
                    Debug.Log("Mutation unsuccessful with score: " + score);
                    playerADeck = Deck.CopyDeck(bestDeck);
                }

                for (int i = 0; i < numberOfMutations; i++)
                {
                    playerADeck.RandomMutate(mutateCardPool);
                }

                results = new int[3];
                playedGamesInMatch = 0;
                playerAGoingFirst = true;
            }
        }

        playerADeck.Reset();    //revert decks to decklists 
        playerBDeck.Reset();    //revert decks to decklists 

        board = new LoRBoard();

        playerAFirst = new PlayerX(board, 1, playerADeck);
        playerASecond = new PlayerX(board, 2, playerADeck);
        playerBFirst = new PlayerY(board, 1, playerBDeck);
        playerBSecond = new PlayerY(board, 2, playerBDeck);

        if (playerAGoingFirst)
        {
            playerOne = playerAFirst;
            playerTwo = playerBSecond;
        }
        else
        {
            playerOne = playerBFirst;
            playerTwo = playerASecond;
        }

        board.Initialize(playerOne.Deck(), playerTwo.Deck());

        game = new Game(board);
        if (!showRounds) game.debugging = false;
    }
}