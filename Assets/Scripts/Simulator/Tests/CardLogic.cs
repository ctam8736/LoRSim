using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CardLogic
    {
        LoRBoard board;
        Game game;
        Deck playerOneDeck;
        Deck playerTwoDeck;

        //for manual card draw
        List<Card> playerOneCards;
        List<Card> playerTwoCards;


        [Test]
        public void CithriaOfCloudfieldTest()
        {
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Cithria of Cloudfield");

            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Cithria of Cloudfield");

            playerOneDeck = new Deck("DBTest1", playerOneCards, false);
            playerTwoDeck = new Deck("DBTest2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck);

            ///Game start!
            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria
            MakeMove(new Action("Play", board.playerOneSide.hand.cards[0])); //cith
            MakeMove(new Action("Play", board.playerTwoSide.hand.cards[0])); //cith
            MakeMove(new Action("Attack", board.playerOneSide.bench.units)); //attack with cith
            MakeMove(new Action("Block", new List<Battlefield.BattlePair> { new Battlefield.BattlePair(board.battlefield.AttackerAt(0), board.playerTwoSide.bench.units[0]) })); //block with cith
            MakeMove(new Action("Pass")); //resolve battle
            MakeMove(new Action("Pass")); //pass turn
            MakeMove(new Action("Pass")); //pass turn

            Assert.AreEqual(2, board.roundNumber);
        }

        public void ResetGame()
        {
            board = new LoRBoard();
            game = new Game(board);
        }

        public void SetUpPrebuiltDecks(string deckOneFilePath, string deckTwoFilePath)
        {
            // Set up a two-player game with prebuilt decks.

            playerOneDeck = CardData.LoadDeckFromJson(deckOneFilePath);
            playerTwoDeck = CardData.LoadDeckFromJson(deckTwoFilePath);

            playerOneDeck.Reset();    //revert decks to decklists 
            playerTwoDeck.Reset();    //revert decks to decklists 
        }

        private Card Convert(string cardName)
        {
            return CardData.ConvertToCard(cardName);
        }

        private void AddToPlayerOneDeck(string cardName)
        {
            playerOneCards.Add(Convert(cardName));
        }

        private void AddToPlayerTwoDeck(string cardName)
        {
            playerTwoCards.Add(Convert(cardName));
        }

        private void MakeMove(Action action)
        {
            game.ExecuteAction(action);
        }
    }
}
