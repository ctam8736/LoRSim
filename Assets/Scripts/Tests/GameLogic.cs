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
            SetUpPrebuiltDecks("Assets/Decks/cithria.json", "Assets/Decks/cithria.json");

            // Make sure all cards loaded correctly.
            Assert.AreEqual(40, playerOneDeck.cards.Count);
            Assert.AreEqual(40, playerTwoDeck.cards.Count);

            board.Initialize(playerOneDeck, playerTwoDeck);

            // Board is in a ready state.
            Assert.AreEqual(1, board.roundNumber);
            Assert.AreEqual(1, board.activePlayer);
            Assert.IsTrue(board.playerOneSide.hasAttackToken);
            Assert.IsFalse(board.playerTwoSide.hasAttackToken);

            //game.ExecuteAction(new Action("Pass"));
        }

        [Test]
        public void DeadBlockerTest()
        {
            ResetGame();
            List<Card> playerOneCards = new List<Card>();
            List<Card> playerTwoCards = new List<Card>();

            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Mystic Shot"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerOneCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));

            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Mystic Shot"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));
            playerTwoCards.Add(CardData.ConvertToCard("Cithria of Cloudfield"));

            playerOneDeck = new Deck("DBTest1", playerOneCards, false);
            playerTwoDeck = new Deck("DBTest2", playerTwoCards, false);

            board.Initialize(playerOneDeck, playerTwoDeck);
            Debug.Log(board.playerOneSide.hand);
            Debug.Log(board.playerTwoSide.hand);
            game.ExecuteAction(new Action("Play", board.playerOneSide.hand.cards[0]));
            game.ExecuteAction(new Action("Play", board.playerTwoSide.hand.cards[0]));
            game.ExecuteAction(new Action("Pass"));
            game.ExecuteAction(new Action("Pass"));
            Assert.AreEqual(2, board.roundNumber);
            game.ExecuteAction(new Action("Play", board.playerTwoSide.hand.cards[0]));
            game.ExecuteAction(new Action("Target", board.playerOneSide.bench.units[0]));
            game.ExecuteAction(new Action("Pass"));
            Assert.AreEqual(0, board.playerOneSide.bench.units.Count);
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
