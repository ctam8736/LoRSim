using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractivePrinter : DisplayPrinter
{
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > turnDelay)
        {
            timer = timer % turnDelay;
            HandleGameEnd();
            PlayGameTurn(); //maybe wait for the human player - (if action is not null, then play game turn)
            UpdateText();
        }
    }

    //Resets board, deck, and player state upon game end.
    protected override void ResetGame(bool start = false)
    {
        WriteEndGameOutput();

        playerAGoingFirst = !playerAGoingFirst; //swap player turn for even matches

        if (!start)
        {
            playedGamesInMatch += 1;
        }

        if (playedGamesInMatch >= numberOfGamesInMatch) //match finished, reset results
        {
            finishedMatches += 1;
            results = new int[3];
            playedGamesInMatch = 0;
            playerAGoingFirst = true;
        }

        board = new LoRBoard();
        playerADeck.Reset();    //revert decks to decklists 
        playerBDeck.Reset();    //revert decks to decklists 

        //players reset with new board and decks, separate w/ respect to who's going first (may clean up)
        playerAFirst = new InteractivePlayer(board, 1, playerADeck);
        playerASecond = new InteractivePlayer(board, 2, playerADeck);
        playerBFirst = new PlayerX(board, 1, playerBDeck);
        playerBSecond = new PlayerX(board, 2, playerBDeck);

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

        if (!showRounds)
        {
            game.debugging = false;
        }
        else
        {
            game.debugging = true;
        }
    }

    public override void WriteEndGameOutput()
    {
    }
}