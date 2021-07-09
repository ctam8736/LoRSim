using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class RatioPrinter : MonoBehaviour
{
    public LoRBoard board;
    TextMeshProUGUI playerOneHandText;
    TextMeshProUGUI playerTwoHandText;
    TextMeshProUGUI ManaText;
    TextMeshProUGUI playerOneBenchText;
    TextMeshProUGUI playerTwoBenchText;
    TextMeshProUGUI battlefieldText;
    TextMeshProUGUI roundNumberText;
    TextMeshProUGUI nexusHealthText;
    TextMeshProUGUI resultText;
    TextMeshProUGUI playerInfoText;

    public float turnDelay = 1f;
    float timer = 0f;
    Player playerOne;
    Player playerTwo;

    Player playerAFirst;
    Player playerASecond;
    Player playerBFirst;
    Player playerBSecond;
    Deck playerADeck;
    Deck playerBDeck;
    Game game;

    bool gameEnded = false;
    bool playerAGoingFirst = false;
    public bool showRounds = false;

    int[] decks = new int[41];
    float[,] matchups = new float[41, 41];
    int[] results = new int[3];
    int playedGamesInMatch = 0;
    public int numberOfGamesInMatch = 1;
    int deckIndexA = 0;
    int deckIndexB = 1;
    string writeFile = "Assets/test.txt";

    private TextMeshProUGUI GetTextComponent(string name)
    {
        return transform.Find(name).gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void FindTextReferences()
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
    }

    // Start is called before the first frame update
    void Start()
    {
        FindTextReferences();
        for (int i = 0; i < 41; i++)
        {
            decks[i] = i;
        }

        ResetGame(true);
    }

    private void ResetGame(bool start = false)
    {
        /**
        StreamWriter writer;
        **/

        playerAGoingFirst = !playerAGoingFirst; //swap player turn for even matches

        if (!start)
        {
            playedGamesInMatch += 1;
        }
        else
        {
            /**
            writer = new StreamWriter(writeFile, true);
            writer.Write("50");
            writer.Close();
            **/
        }

        /**


        if (playedGamesInMatch >= numberOfGamesInMatch)
        {
            writer = new StreamWriter(writeFile, true);
            float score = (results[1] * 100f + results[0] * 50f) / numberOfGamesInMatch;
            writer.Write("," + score);
            matchups[deckIndexA, deckIndexB] = score;
            writer.Close();

            if (deckIndexB + 1 == deckIndexA)
            {
                deckIndexB += 2;
            }
            else
            {
                deckIndexB += 1;
            }

            if (deckIndexB > decks.Length - 1)
            {
                deckIndexA += 1;
                deckIndexB = deckIndexA + 1;

                writer = new StreamWriter(writeFile, true);
                writer.WriteLine();
                for (int i = 0; i < deckIndexA; i++)
                {
                    writer.Write(100 - matchups[i, deckIndexA]);
                    writer.Write(",");
                }
                writer.Write("50");
                writer.Close();
            }
            

            if (deckIndexA > decks.Length - 1 || deckIndexB > decks.Length - 1)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }

            results = new int[3];
            playedGamesInMatch = 0;
            playerAGoingFirst = true;
        }

        **/

        //playerADeck = new DeckBuilder2(deckIndexA).deck;
        //playerBDeck = new DeckBuilder2(deckIndexB).deck;
        playerADeck = LoadDeckFromJson("Assets/Decks/academycith.json");
        playerBDeck = LoadDeckFromJson("Assets/Decks/academycith.json");

        board = new LoRBoard();

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

    public Deck LoadDeckFromJson(string filePath)
    {
        string deckOneJson;
        using (StreamReader reader = new StreamReader(filePath))
        {
            deckOneJson = reader.ReadToEnd();
        }
        DeckBuilder deck = JsonUtility.FromJson<DeckBuilder>(deckOneJson);
        return deck.ToDeck();
    }

    void PlayGameTurn()
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

    bool HandleGameEnd()
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

    // Update is called once per frame
    void Update()
    {
        if (showRounds)
        {
            timer += Time.deltaTime;
            if (timer > turnDelay)
            {
                timer = timer % turnDelay;
                HandleGameEnd();
                PlayGameTurn();
                UpdateText();
            }
        }
        else
        {
            while (!HandleGameEnd())
            {
                PlayGameTurn();
            }
            UpdateText();
        }
    }

    void UpdateText()
    {
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
        resultText.text = "Ties: " + results[0] + "\nPlayer One Wins: " + results[1] + "\nPlayer Two Wins: " + results[2];
    }

    public class DeckBuilder
    {
        public string name;
        public string[] cards;
        public Dictionary<string, Dictionary<string, int>> cardLibrary;

        public DeckBuilder()
        {
        }
        public Deck ToDeck()
        {
            List<Card> newCards = new List<Card>();
            foreach (string cardName in cards)
            {
                switch (cardName)
                {
                    case "Cithria of Cloudfield":
                        newCards.Add(new UnitCard(cardName, 1, 2, 2));
                        break;

                    case "Bloodthirsty Marauder":
                        newCards.Add(new UnitCard(cardName, 1, 3, 1));
                        break;

                    case "Nimble Poro":
                        newCards.Add(new UnitCard(cardName, 1, 1, 1, new List<Keyword> { Keyword.QuickAttack }));
                        break;

                    case "Plucky Poro":
                        newCards.Add(new UnitCard(cardName, 1, 1, 1, new List<Keyword> { Keyword.Tough }));
                        break;

                    case "Startled Stomper":
                        newCards.Add(new UnitCard(cardName, 2, 2, 3, new List<Keyword> { Keyword.Overwhelm }));
                        break;

                    case "Ruthless Raider":
                        newCards.Add(new UnitCard(cardName, 2, 3, 1, new List<Keyword> { Keyword.Overwhelm, Keyword.Tough }));
                        break;

                    case "Academy Prodigy":
                        newCards.Add(new UnitCard(cardName, 2, 3, 1, new List<Keyword> { Keyword.QuickAttack }));
                        break;

                    case "Loyal Badgerbear":
                        newCards.Add(new UnitCard(cardName, 3, 3, 4));
                        break;

                    case "Golden Crushbot":
                        newCards.Add(new UnitCard(cardName, 3, 2, 5));
                        break;

                    case "Iron Ballista":
                        newCards.Add(new UnitCard(cardName, 3, 4, 3, new List<Keyword> { Keyword.Overwhelm }));
                        break;

                    case "Alpha Wildclaw":
                        newCards.Add(new UnitCard(cardName, 6, 7, 6, new List<Keyword> { Keyword.Overwhelm }));
                        break;

                    case "Mystic Shot":
                        //newCards.Add(new SpellCard(cardName, null));
                        break;
                }
            }
            Deck deck = new Deck(name, newCards);
            return deck;
        }
    }

    public class DeckBuilder2
    {
        public Deck deck;
        public List<Card> newCards = new List<Card>();

        public DeckBuilder2(int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                newCards.Add(new UnitCard("Academy Prodigy", 2, 3, 1, new List<Keyword> { Keyword.QuickAttack }));
            }
            for (int i = 0; i < 40 - numCards; i++)
            {
                newCards.Add(new UnitCard("Loyal Badgerbear", 3, 3, 4));
            }
            deck = new Deck("C" + numCards, newCards);
        }
    }
}