using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class GameLogic
    {

        [Test]
        public void GameLogicTest()
        {
            // Set up a two-player game with the Cithria deck.
            LoRBoard board;
            Game game;

            Deck playerOneDeck = CardData.LoadDeckFromJson("Assets/Decks/cithria.json");
            Deck playerTwoDeck = CardData.LoadDeckFromJson("Assets/Decks/cithria.json");

            // Make sure all cards loaded correctly.
            Assert.AreEqual(playerOneDeck.cards.Count, 40);
            Assert.AreEqual(playerTwoDeck.cards.Count, 40);

            board = new LoRBoard();
            playerOneDeck.Reset();    //revert decks to decklists 
            playerTwoDeck.Reset();    //revert decks to decklists 

            board.Initialize(playerOneDeck, playerTwoDeck);

            game = new Game(board);

            // Board is in a ready state.
            Assert.AreEqual(board.roundNumber, 1);
            Assert.AreEqual(board.activePlayer, 1);
            Assert.IsTrue(board.playerOneSide.hasAttackToken);
            Assert.IsFalse(board.playerTwoSide.hasAttackToken);

            //game.ExecuteAction(new Action("Pass"));
        }
    }
}
