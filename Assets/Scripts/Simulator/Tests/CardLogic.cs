using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    //integration tests that play out scenarios for specific cards
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
            //initialize custom card lists
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

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria, trade in combat
            UnitCard cithria1 = (UnitCard)board.playerOneSide.hand.cards[0];
            UnitCard cithria2 = (UnitCard)board.playerTwoSide.hand.cards[0];

            MakeMove(new Action("Play", cithria1)); //cith

            Assert.AreEqual(board.playerOneSide.bench.units[0], cithria1);

            MakeMove(new Action("Play", cithria2)); //cith

            Assert.AreEqual(board.playerTwoSide.bench.units[0], cithria2);

            MakeMove(new Action("Attack", cithria1)); //attack with cith
            MakeMove(new Action("Pass")); //confirm attacks
            MakeMove(new Action("Block", cithria1, cithria2)); //block with cith
            MakeMove(new Action("Pass")); //confirm blocks
            MakeMove(new Action("Pass")); //resolve battle

            Assert.AreEqual(0, board.playerOneSide.bench.units.Count); //check both cithrias are dead
            Assert.AreEqual(0, board.playerTwoSide.bench.units.Count);

            MakeMove(new Action("Pass")); //pass turn
            MakeMove(new Action("Pass")); //pass turn

            Assert.AreEqual(2, board.roundNumber);

            //Round two: both players play Cithria, no block, p1 takes damage
            cithria1 = (UnitCard)board.playerOneSide.hand.cards[0];
            cithria2 = (UnitCard)board.playerTwoSide.hand.cards[0];

            MakeMove(new Action("Play", cithria2)); //cith

            Assert.AreEqual(board.playerTwoSide.bench.units[0], cithria2);

            MakeMove(new Action("Play", cithria1)); //cith

            Assert.AreEqual(board.playerOneSide.bench.units[0], cithria1);

            MakeMove(new Action("Attack", cithria2)); //attack with cith
            MakeMove(new Action("Pass")); //confirm attacks
            MakeMove(new Action("Pass")); //no blocks, resolve battle

            Assert.AreEqual(18, board.playerOneSide.nexus.health);

            MakeMove(new Action("Pass")); //pass turn
            MakeMove(new Action("Pass")); //pass turn
        }

        [Test]
        public void PluckyPoroTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Plucky Poro");
            AddToPlayerOneDeck("Plucky Poro");
            AddToPlayerOneDeck("Plucky Poro");
            AddToPlayerOneDeck("Plucky Poro");

            AddToPlayerTwoDeck("Plucky Poro");
            AddToPlayerTwoDeck("Plucky Poro");
            AddToPlayerTwoDeck("Plucky Poro");
            AddToPlayerTwoDeck("Plucky Poro");

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria, trade in combat
            UnitCard plucky1 = (UnitCard)board.playerOneSide.hand.cards[0];
            UnitCard plucky2 = (UnitCard)board.playerTwoSide.hand.cards[0];

            MakeMove(new Action("Play", plucky1));
            MakeMove(new Action("Play", plucky2));

            MakeMove(new Action("Attack", plucky1)); //attack
            MakeMove(new Action("Pass")); //confirm attacks
            MakeMove(new Action("Block", plucky1, plucky2)); //block
            MakeMove(new Action("Pass")); //confirm blocks
            MakeMove(new Action("Pass")); //resolve battle

            Assert.AreEqual(board.playerOneSide.bench.units[0], plucky1); //check both poros are still alive
            Assert.AreEqual(board.playerTwoSide.bench.units[0], plucky2);

            MakeMove(new Action("Pass")); //pass turn
            MakeMove(new Action("Pass")); //pass turn

            Assert.AreEqual(2, board.roundNumber);
        }

        [Test]
        public void SuccessionTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Succession");

            AddToPlayerTwoDeck("Succession");

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players pass

            MakeMove(new Action("Pass")); //pass turn
            MakeMove(new Action("Pass")); //pass turn

            Assert.AreEqual(2, board.roundNumber);

            //Round two: p2 plays succession

            SpellCard scs = (SpellCard)board.playerTwoSide.hand.cards[0];
            MakeMove(new Action("Play", scs));
            MakeMove(new Action("Pass")); //p2 confirms spell casts
            MakeMove(new Action("Pass")); //allow
            MakeMove(new Action("Pass")); //p1 passes back to p2

            Assert.AreEqual(1, board.playerTwoSide.bench.units.Count);
            Assert.AreEqual("Dauntless Vanguard", board.playerTwoSide.bench.units[0].name);

            MakeMove(new Action("Pass")); //p2 passes

            Assert.AreEqual(3, board.roundNumber);
        }






































        //initialization methods

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

        //helper methods

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
