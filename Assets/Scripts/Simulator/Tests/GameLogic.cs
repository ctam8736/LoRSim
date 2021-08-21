using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameLogic
    {
        LoRBoard board;
        Game game;
        Deck playerOneDeck;
        Deck playerTwoDeck;


        [Test]
        public void InitializationTest()
        {
            // Set up a two-player game with the Cithria deck.
            ResetGame();
            SetUpPrebuiltDecks("Assets/Decks/test.json", "Assets/Decks/test.json");

            // Make sure all cards loaded correctly.
            Assert.AreEqual(40, playerOneDeck.cards.Count);
            Assert.AreEqual(40, playerTwoDeck.cards.Count);

            board.Initialize(playerOneDeck, playerTwoDeck);

            // Board is in a ready state.
            Assert.AreEqual(1, board.roundNumber);
            Assert.AreEqual(1, board.activePlayer);
            Assert.IsTrue(board.playerOneSide.hasAttackToken);
            Assert.IsFalse(board.playerTwoSide.hasAttackToken);
        }

        public void ResetGame()
        {
            board = new LoRBoard();
            game = new Game(board);
        }

        public void SetUpPrebuiltDecks(string deckOneFilePath, string deckTwoFilePath)
        {
            // Set up a two-player game with the Cithria deck.

            playerOneDeck = CardData.LoadDeckFromJson(deckOneFilePath);
            playerTwoDeck = CardData.LoadDeckFromJson(deckTwoFilePath);

            playerOneDeck.Reset();    //revert decks to decklists 
            playerTwoDeck.Reset();    //revert decks to decklists 
        }
    }
}
