using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    //integration tests that play out scenarios for specific cards
    public class LMGTest
    {
        LoRBoard board;
        Game game;
        Deck playerOneDeck;
        Deck playerTwoDeck;
        LegalMoveGenerator legalMoveGenerator;

        //for manual card draw
        List<Card> playerOneCards;
        List<Card> playerTwoCards;

        private void PrintLegalMoves()
        {
            Debug.Log("\n------------------------------\nLegal Moves:\n");
            foreach (Action action in legalMoveGenerator.LegalMoves())
            {
                Debug.Log(action);
            }
            Debug.Log("------------------------------\n");
        }


        [Test]
        public void LegalMoveTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Silverwing Diver");

            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Silverwing Diver");

            ReinitializeDecks();


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria
            UnitCard cithria1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard cithria2 = (UnitCard)PlayerTwoHandAt(0);

            PrintLegalMoves();
            Play(cithria1); //cith

            PrintLegalMoves();
            Play(cithria2); //cith

            PrintLegalMoves();
            Pass();

            PrintLegalMoves();
            Pass();

            Assert.AreEqual(2, board.roundNumber);

            //Round 2-3 pass

            PrintLegalMoves();
            Pass();

            PrintLegalMoves();
            Pass();

            Assert.AreEqual(3, board.roundNumber);
            PrintLegalMoves();
            Pass();

            PrintLegalMoves();
            Pass();

            Assert.AreEqual(4, board.roundNumber);

            //Round four: play silverwing divers, attempt blocks with cith and divers
            UnitCard swdiver1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard swdiver2 = (UnitCard)PlayerTwoHandAt(0);

            PrintLegalMoves();
            Play(swdiver2); //p2 diver

            PrintLegalMoves();
            Play(swdiver1); //p1 diver

            PrintLegalMoves();
            AttackWith(swdiver2);

            PrintLegalMoves();
            Pass(); //confirm attacks

            //Assert.IsFalse(Block(swdiver2, cithria1));

            PrintLegalMoves();
            Block(swdiver2, swdiver1);

            PrintLegalMoves();
            Pass(); //confirm blocks

            PrintLegalMoves();
            Pass(); //resolve combat

            Assert.AreEqual(2, swdiver1.health);
            Assert.AreEqual(2, swdiver2.health);

            PrintLegalMoves();
            Pass();

            PrintLegalMoves();
            Pass();

            Assert.AreEqual(5, board.roundNumber);
        }







































        //initialization methods

        public void ResetGame()
        {
            board = new LoRBoard();
            game = new Game(board);
            legalMoveGenerator = new LegalMoveGenerator(board);
        }

        public void ReinitializeDecks()
        {
            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);
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

        private bool MakeMove(Action action)
        {
            return game.ExecuteAction(action);
        }

        private void MutualPass()
        {
            Pass();
            Pass();
        }

        private void Pass()
        {
            MakeMove(new Action("Pass"));
        }

        private void WaitUntilTurn(int turn)
        {
            while (board.roundNumber < turn)
            {
                Pass();
            }
        }

        private void AttackWith(UnitCard unit)
        {
            MakeMove(new Action("Attack", unit));
        }

        private void Challenge(UnitCard attacker, UnitCard blocker)
        {
            MakeMove(new Action("Challenge", attacker, blocker));
        }

        private bool Block(UnitCard attacker, UnitCard blocker)
        {
            return MakeMove(new Action("Block", attacker, blocker));
        }

        private void Play(Card card)
        {
            MakeMove(new Action("Play", card));
        }

        private void Target(UnitCard unit)
        {
            MakeMove(new Action("Target", unit));
        }

        private Card PlayerOneHandAt(int index)
        {
            return board.playerOneSide.hand.cards[index];
        }

        private Card PlayerTwoHandAt(int index)
        {
            return board.playerTwoSide.hand.cards[index];
        }
    }
}
