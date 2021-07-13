using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public abstract class Printer : MonoBehaviour
{
    //~~~ Class Fields ~~~

    //--- Game Logic ---
    public LoRBoard board;
    protected Game game;

    //--- Text Object References ---
    protected TextMeshProUGUI playerOneHandText;
    protected TextMeshProUGUI playerTwoHandText;
    protected TextMeshProUGUI ManaText;
    protected TextMeshProUGUI playerOneBenchText;
    protected TextMeshProUGUI playerTwoBenchText;
    protected TextMeshProUGUI battlefieldText;
    protected TextMeshProUGUI roundNumberText;
    protected TextMeshProUGUI nexusHealthText;
    protected TextMeshProUGUI resultText;
    protected TextMeshProUGUI playerInfoText;
    protected TextMeshProUGUI attackTokenText;

    //--- Plotter ---
    public DataPoints plotter;

    //--- Play Timer Logic ---
    public float turnDelay = 1f;
    protected float timer = 0f;

    //--- Player Logic ---
    protected Player playerOne;
    protected Player playerTwo;
    protected Player playerAFirst;
    protected Player playerASecond;
    protected Player playerBFirst;
    protected Player playerBSecond;
    protected Deck playerADeck;
    protected Deck playerBDeck;

    //--- Control variables ---
    protected bool gameEnded = false;
    protected bool playerAGoingFirst = false;
    public bool showRounds = false;

    //--- Match variables ---
    protected int[] results = new int[3];
    protected int playedGamesInMatch = 0;
    public int numberOfGamesInMatch = 1000;
    protected int finishedMatches = 0;

    //--- Card libraries ---
    protected Dictionary<string, Card> cardDictionary;
    protected List<Card> cardPool;

    //--- RNG ---
    protected System.Random rng;





    //~~~ Class Initialization Methods ~~~

    //Finds TMPro text object by name.
    protected TextMeshProUGUI GetTextComponent(string name)
    {
        return transform.Find(name).gameObject.GetComponent<TextMeshProUGUI>();
    }

    //Links TMProUGUI GameObjects to variables.
    protected void FindTextReferences()
    {

        playerOneHandText = GetTextComponent("Player One Hand Text");
        playerTwoHandText = GetTextComponent("Player Two Hand Text");
        ManaText = GetTextComponent("Mana Text");
        playerOneBenchText = GetTextComponent("Player One Bench Text");
        playerTwoBenchText = GetTextComponent("Player Two Bench Text");
        battlefieldText = GetTextComponent("Battlefield Text");
        roundNumberText = GetTextComponent("Round Number Text");
        nexusHealthText = GetTextComponent("Nexus Health Text");
        resultText = GetTextComponent("Result Text");
        playerInfoText = GetTextComponent("Player Info Text");
        attackTokenText = GetTextComponent("Attack Token Text");
    }

    //Fills dictionary with cards.
    protected void InitializeDictionary()
    {
        cardDictionary = new Dictionary<string, Card>(){
            {"Cithria of Cloudfield", new UnitCard("Cithria of Cloudfield", 1, 2, 2)},
            {"Bloodthirsty Marauder", new UnitCard("Bloodthirsty Marauder", 1, 3, 1)},
            {"Legion Rearguard", new UnitCard("Legion Rearguard", 1, 3, 2, new List<Keyword> { Keyword.CantBlock })},
            {"Prowling Cutthroat", new UnitCard("Prowling Cutthroat", 1, 1, 1, new List<Keyword> { Keyword.Elusive, Keyword.Fearsome })},
            {"Precious Pet", new UnitCard("Precious Pet", 1, 2, 1, new List<Keyword> { Keyword.Fearsome })},
            {"Sinister Poro", new UnitCard("Sinister Poro", 1, 1, 1, new List<Keyword> { Keyword.Fearsome })},
            {"Daring Poro", new UnitCard("Daring Poro", 1, 1, 1, new List<Keyword> { Keyword.Elusive })},
            {"Nimble Poro", new UnitCard("Nimble Poro", 1, 1, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Plucky Poro", new UnitCard("Plucky Poro", 1, 1, 1, new List<Keyword> { Keyword.Tough })},
            {"Startled Stomper", new UnitCard("Startled Stomper", 2, 2, 3, new List<Keyword> { Keyword.Overwhelm })},
            {"Vanguard Defender", new UnitCard("Vanguard Defender", 2, 2, 2, new List<Keyword> { Keyword.Tough })},
            {"Ruthless Raider", new UnitCard("Ruthless Raider", 2, 3, 1, new List<Keyword> { Keyword.Overwhelm, Keyword.Tough })},
            {"Academy Prodigy", new UnitCard("Academy Prodigy", 2, 3, 1, new List<Keyword> { Keyword.QuickAttack })},
            {"Arachnid Horror", new UnitCard("Arachnid Horror", 2, 3, 2, new List<Keyword> { Keyword.Fearsome })},
            {"Loyal Badgerbear", new UnitCard("Loyal Badgerbear", 3, 3, 4)},
            {"Golden Crushbot", new UnitCard("Golden Crushbot", 3, 2, 5)},
            {"Iron Ballista", new UnitCard("Iron Ballista", 3, 4, 3, new List<Keyword> { Keyword.Overwhelm })},
            {"Reckless Trifarian", new UnitCard("Reckless Trifarian", 3, 5, 4, new List<Keyword> { Keyword.CantBlock })},
            {"Silverwing Diver", new UnitCard("Silverwing Diver", 4, 2, 3, new List<Keyword> { Keyword.Elusive, Keyword.Tough })},
            {"Bull Elnuk", new UnitCard("Bull Elnuk", 4, 4, 5)},
            {"Trifarian Shieldbreaker", new UnitCard("Trifarian Shieldbreaker", 5, 6, 5, new List<Keyword> { Keyword.Fearsome })},
            {"Alpha Wildclaw", new UnitCard("Alpha Wildclaw", 6, 7, 6, new List<Keyword> { Keyword.Overwhelm })},
            {"The Empyrean", new UnitCard("The Empyrean", 7, 6, 5, new List<Keyword> { Keyword.Elusive })},

            {"Health Potion", new SpellCard("Health Potion", SpellType.Burst)}
        };

        cardPool = ConvertToList(cardDictionary.Values);
    }

    //Converts the card dictionary to a list of Cards.
    protected List<Card> ConvertToList(Dictionary<string, Card>.ValueCollection valueCollection)
    {
        List<Card> list = new List<Card>();
        list.AddRange(valueCollection);
        return list;
    }

    //Changes game text to reflect board state.
    protected void UpdateText()
    {
        float score = ((results[1] * 100f + results[0] * 50f) / playedGamesInMatch);

        playerOneHandText.text = "Player One Hand: \n" + board.playerOneSide.hand.ToString();
        playerTwoHandText.text = "Player Two Hand: \n" + board.playerTwoSide.hand.ToString();
        playerOneBenchText.text = "Player One Bench: \n" + board.playerOneSide.bench.ToString();
        playerTwoBenchText.text = "Player Two Bench: \n" + board.playerTwoSide.bench.ToString();
        ManaText.text = "Player One Mana: \n" + board.playerOneSide.mana.ToString() + "\n\nPlayer Two Mana: \n" + board.playerTwoSide.mana.ToString();
        roundNumberText.text = "Round Number: " + board.roundNumber;
        nexusHealthText.text = "Player One Nexus Health: " + board.playerOneSide.nexus.health + "\nPlayer Two Nexus Health: " + board.playerTwoSide.nexus.health;
        playerInfoText.text = "Player 1 is: " + (playerAGoingFirst ? "playerX" : "playerDefault") +
                                " playing the deck " + (playerAGoingFirst ? playerADeck.name : playerBDeck.name) +
                                "\nPlayer 2 is: " + (!playerAGoingFirst ? "playerX" : "playerDefault") +
                                " playing the deck " + (!playerAGoingFirst ? playerADeck.name : playerBDeck.name);
        attackTokenText.text = "Player One Attack Token: " + board.playerOneSide.hasAttackToken + "\n" +
                                    "Player Two Attack Token: " + board.playerTwoSide.hasAttackToken;
        if (board.inCombat)
        {
            if (board.attackingPlayer == 1)
            {
                battlefieldText.text = "Player 1 attacking\nBattlefield: \n" + board.battlefield.ToString();
            }
            else
            {
                battlefieldText.text = "Player 2 attacking\nBattlefield: \n" + board.battlefield.ToString();
            }
        }
        else
        {
            battlefieldText.text = "Battlefield: \n" + board.battlefield.ToString();
        }
        resultText.text = "Ties: " + results[0] + "\nPlayer One Wins: " + results[1] + "\nPlayer Two Wins: " + results[2] + "\nScore: " + ((results[1] * 100f + results[0] * 50f) / playedGamesInMatch) + "%";
    }

    //Creates a deck from a json file given path.
    public Deck LoadDeckFromJson(string filePath)
    {
        string deckOneJson;
        using (StreamReader reader = new StreamReader(filePath))
        {
            deckOneJson = reader.ReadToEnd();
        }
        DeckBuilder deck = JsonUtility.FromJson<DeckBuilder>(deckOneJson);
        return deck.ToDeck(cardDictionary);
    }

    public class DeckBuilder
    {
        public string name;
        public string[] cards;

        public DeckBuilder()
        {
        }
        public Deck ToDeck(Dictionary<string, Card> cardDictionary)
        {
            List<Card> newCards = new List<Card>();
            foreach (string cardName in cards)
            {
                if (cardDictionary.ContainsKey(cardName))
                {
                    Card newCard = cardDictionary[cardName];
                    if (newCard is UnitCard)
                    {
                        newCards.Add(UnitCard.CopyCard((UnitCard)newCard));
                    }
                }
            }
            Deck deck = new Deck(name, newCards);
            return deck;
        }
    }





    //~~~ Class Game Logic Methods ~~~

    //Completes one game turn, calling for an action from the active player.
    protected void PlayGameTurn()
    {
        if (board.activePlayer == 1)
        {
            Action action = playerOne.MakeAction();
            game.ExecuteAction(action);
        }
        else
        {
            Action action = playerTwo.MakeAction();
            game.ExecuteAction(action);
        }
    }

    //Updates results and handles reset if game has terminated.
    protected bool HandleGameEnd()
    {
        //update results
        int result = game.GameResult();
        if (result != -1)
        {
            if (!playerAGoingFirst)
            {
                if (result != 0)
                {
                    results[3 - result] += 1;
                    ResetGame();
                    return true;
                }
            }
            results[result] += 1;
            ResetGame();
            return true;
        }
        return false;
    }

    //Resets board, deck, and player state upon game end.
    protected void ResetGame(bool start = false)
    {

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