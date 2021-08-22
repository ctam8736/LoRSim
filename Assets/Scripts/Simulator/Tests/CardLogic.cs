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
            UnitCard cithria1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard cithria2 = (UnitCard)PlayerTwoHandAt(0);

            Play(cithria1);

            Assert.AreEqual(board.playerOneSide.bench.units[0], cithria1);

            Play(cithria2);

            Assert.AreEqual(board.playerTwoSide.bench.units[0], cithria2);

            AttackWith(cithria1); //attack with cith
            Pass(); //confirm attacks
            Block(cithria1, cithria2); //block with cith
            Pass(); //confirm blocks
            Pass(); //resolve battle

            Assert.AreEqual(0, board.playerOneSide.bench.units.Count); //check both cithrias are dead
            Assert.AreEqual(0, board.playerTwoSide.bench.units.Count);

            MutualPass();
            Assert.AreEqual(2, board.roundNumber);

            //Round two: both players play Cithria, no block, p1 takes damage
            cithria1 = (UnitCard)PlayerOneHandAt(0);
            cithria2 = (UnitCard)PlayerTwoHandAt(0);

            Play(cithria2);

            Assert.AreEqual(board.playerTwoSide.bench.units[0], cithria2);

            Play(cithria1);

            Assert.AreEqual(board.playerOneSide.bench.units[0], cithria1);

            AttackWith(cithria2); //attack with cith
            Pass(); //confirm attacks
            Pass(); //no blocks, resolve battle

            Assert.AreEqual(18, board.playerOneSide.nexus.health);

            MutualPass();
            Assert.AreEqual(3, board.roundNumber);
        }

        [Test]
        public void PluckyPoroTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Plucky Poro");

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
            UnitCard plucky1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard plucky2 = (UnitCard)PlayerTwoHandAt(0);

            Play(plucky1);
            Play(plucky2);

            AttackWith(plucky1); //attack
            Pass(); //confirm attacks
            Block(plucky1, plucky2); //block
            Pass(); //confirm blocks
            Pass(); //resolve battle

            Assert.AreEqual(board.playerOneSide.bench.units[0], plucky1); //check both poros are still alive
            Assert.AreEqual(board.playerTwoSide.bench.units[0], plucky2);

            MutualPass();
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

            MutualPass();
            Assert.AreEqual(2, board.roundNumber);

            //Round two: p2 plays succession

            SpellCard scs = (SpellCard)PlayerTwoHandAt(0);
            Play(scs);
            Pass(); //p2 confirms spell casts
            Pass(); //allow
            Pass(); //p1 passes back to p2

            Assert.AreEqual(1, board.playerTwoSide.bench.units.Count);
            Assert.AreEqual("Dauntless Vanguard", board.playerTwoSide.bench.units[0].name);

            Pass(); //p2 passes
            Assert.AreEqual(3, board.roundNumber);
        }

        [Test]
        public void SilverwingDiverTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Silverwing Diver");

            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Silverwing Diver");

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria
            UnitCard cithria1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard cithria2 = (UnitCard)PlayerTwoHandAt(0);

            Play(cithria1); //cith
            Play(cithria2); //cith

            MutualPass();
            Assert.AreEqual(2, board.roundNumber);

            //Round 2-3 pass

            MutualPass();

            Assert.AreEqual(3, board.roundNumber);
            MutualPass();

            Assert.AreEqual(4, board.roundNumber);

            //Round four: play silverwing divers, attempt blocks with cith and divers
            UnitCard swdiver1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard swdiver2 = (UnitCard)PlayerTwoHandAt(0);

            Play(swdiver2); //p2 diver
            Play(swdiver1); //p1 diver

            AttackWith(swdiver2);
            Pass(); //confirm attacks
            Assert.IsFalse(Block(swdiver2, cithria1));
            Block(swdiver2, swdiver1);
            Pass(); //confirm blocks
            Pass(); //resolve combat

            Assert.AreEqual(2, swdiver1.health);
            Assert.AreEqual(2, swdiver2.health);

            MutualPass();
            Assert.AreEqual(5, board.roundNumber);
        }

        [Test]
        public void RadiantStrikeTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Radiant Strike");
            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Radiant Strike");

            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Radiant Strike");
            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Radiant Strike");

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria
            UnitCard cithria1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard cithria2 = (UnitCard)PlayerTwoHandAt(0);
            SpellCard rs1 = (SpellCard)PlayerOneHandAt(1);
            SpellCard rs2 = (SpellCard)PlayerTwoHandAt(1);
            UnitCard cithria3 = (UnitCard)PlayerOneHandAt(2);
            UnitCard cithria4 = (UnitCard)PlayerTwoHandAt(2);
            SpellCard rs3 = (SpellCard)PlayerOneHandAt(3);
            SpellCard rs4 = (SpellCard)PlayerTwoHandAt(3);

            Play(cithria1); //cith
            Play(cithria2); //cith

            MutualPass();
            Assert.AreEqual(2, board.roundNumber);

            //Round two: player2 attacks with cithria, player1 blocks and uses radiant strike, player 2 responds with radiant strike

            AttackWith(cithria2);
            Pass();

            Block(cithria2, cithria1);
            Play(rs1); //player 1 plays radiant strike
            Target(cithria1);
            Pass();

            Play(rs2); //player 2 plays radiant strike
            Target(cithria2);
            Pass();

            Pass(); //exit combat
            Assert.AreEqual(0, board.playerOneSide.bench.units.Count); //check both cithrias are dead
            Assert.AreEqual(0, board.playerTwoSide.bench.units.Count);

            Play(cithria3); //play the next set of cithrias
            Play(cithria4);

            MutualPass();
            Assert.AreEqual(3, board.roundNumber);

            //Round three: player 1 attack with cithria, player 2 blocks, player 1 buffs cithria, p2 does not
            AttackWith(cithria3);
            Pass();

            Block(cithria3, cithria4);
            Pass();

            Play(rs3); //player 1 plays radiant strike
            Target(cithria3);
            Pass();

            Pass(); //exit combat
            Assert.AreEqual(1, board.playerOneSide.bench.units.Count); //check both cithrias are dead
            Assert.AreEqual(0, board.playerTwoSide.bench.units.Count);

            MutualPass();
            Assert.AreEqual(4, board.roundNumber);
        }

        [Test]
        public void SingleCombatTest()
        {
            //initialize custom card lists
            playerOneCards = new List<Card>();
            playerTwoCards = new List<Card>();

            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Single Combat");
            AddToPlayerOneDeck("Cithria of Cloudfield");
            AddToPlayerOneDeck("Single Combat");

            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Single Combat");
            AddToPlayerTwoDeck("Cithria of Cloudfield");
            AddToPlayerTwoDeck("Single Combat");

            playerOneDeck = new Deck("Test1", playerOneCards, false);
            playerTwoDeck = new Deck("Test2", playerTwoCards, false);

            ResetGame();
            board.Initialize(playerOneDeck, playerTwoDeck, deckTerminationDisabled: true);


            ///
            ///game start!
            ///

            Assert.AreEqual(1, board.roundNumber);

            //Round one: both players play Cithria
            UnitCard cithria1 = (UnitCard)PlayerOneHandAt(0);
            UnitCard cithria2 = (UnitCard)PlayerTwoHandAt(0);
            SpellCard sc1 = (SpellCard)PlayerOneHandAt(1);
            SpellCard sc2 = (SpellCard)PlayerTwoHandAt(1);
            UnitCard cithria3 = (UnitCard)PlayerOneHandAt(2);
            UnitCard cithria4 = (UnitCard)PlayerTwoHandAt(2);
            SpellCard sc3 = (SpellCard)PlayerOneHandAt(3);
            SpellCard sc4 = (SpellCard)PlayerTwoHandAt(3);

            Play(cithria1); //cith
            Play(cithria2); //cith

            MutualPass();
            Assert.AreEqual(2, board.roundNumber);

            //Round two: player2 uses single combat outside of combat

            //Round three: both players play cithria, player 1 uses single combat on attack, player two responds with single combat
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

        private void AttackWith(UnitCard unit)
        {
            MakeMove(new Action("Attack", unit));
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
