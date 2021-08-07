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

            //Round one: both players play Cithria
            game.ExecuteAction(new Action("Play", board.playerOneSide.hand.cards[0])); //cith
            game.ExecuteAction(new Action("Play", board.playerTwoSide.hand.cards[0])); //cith
            game.ExecuteAction(new Action("Pass"));
            game.ExecuteAction(new Action("Pass"));

            Assert.AreEqual(2, board.roundNumber);

            //Round two: player two attacks with cithia, player one blocks, player two targets blocker with mystic shot
            UnitCard cith1 = board.playerOneSide.bench.units[0];
            UnitCard cith2 = board.playerTwoSide.bench.units[0];
            game.ExecuteAction(new Action("Attack", new List<UnitCard> { cith2 }));
            List<Battlefield.BattlePair> pairs = new List<Battlefield.BattlePair>();
            pairs.Add(new Battlefield.BattlePair(cith2, cith1));
            game.ExecuteAction(new Action("Block", pairs));
            game.ExecuteAction(new Action("Play", board.playerTwoSide.hand.cards[0])); //mystic shot
            game.ExecuteAction(new Action("Target", cith1));
            game.ExecuteAction(new Action("Pass")); //p2 confirms shot
            game.ExecuteAction(new Action("Pass")); //p1 allows shot and combat resolves
            game.ExecuteAction(new Action("Pass"));
            game.ExecuteAction(new Action("Pass"));
            Assert.AreEqual(0, board.playerOneSide.bench.units.Count);
            Assert.AreEqual(1, board.playerTwoSide.bench.units.Count);
            Assert.AreEqual(20, board.playerOneSide.nexus.health);

            Assert.AreEqual(3, board.roundNumber);

            //Round three: player one plays a cithria and attacks, player two blocks
            game.ExecuteAction(new Action("Play", board.playerOneSide.hand.cards[1]));
            game.ExecuteAction(new Action("Pass"));
            cith1 = board.playerOneSide.bench.units[0];
            cith2 = board.playerTwoSide.bench.units[0];
            game.ExecuteAction(new Action("Attack", new List<UnitCard> { cith1 }));
            pairs = new List<Battlefield.BattlePair>();
            pairs.Add(new Battlefield.BattlePair(cith1, cith2));
            game.ExecuteAction(new Action("Block", pairs));
            game.ExecuteAction(new Action("Pass"));
            game.ExecuteAction(new Action("Pass"));
            game.ExecuteAction(new Action("Pass"));

            Assert.AreEqual(4, board.roundNumber);
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
