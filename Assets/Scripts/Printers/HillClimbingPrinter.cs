using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class HillClimbingPrinter : MonoBehaviour
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
    TextMeshProUGUI attackTokenText;

    public float turnDelay = 1f;
    float timer = 0f;
    Player playerOne;
    Player playerTwo;

    Player playerAFirst;
    Player playerASecond;
    Player playerBFirst;
    Player playerBSecond;

    Deck bestDeck;
    Deck playerADeck;
    Deck playerBDeck;
    Game game;

    bool gameEnded = false;
    bool playerAGoingFirst = false;
    public bool showRounds = false;

    int[] decks = new int[41];
    public float bestOutcome = 50f;
    int[] results = new int[3];
    int playedGamesInMatch = 0;
    public int numberOfGamesInMatch = 1;
    int deckIndexA = 0;
    int deckIndexB = 1;
    string writeFile = "Assets/test.txt";


    int currentChallenger = 0;
    int currentOpponent = 0;
    Deck[] deathmatchParticipants = new Deck[20];
    bool[] survived = new bool[20];
    int[] wins = new int[20];

    public bool mutation = false;
    public bool evolution = true;
    System.Random rng;

    Dictionary<string, Card> cardDictionary;
    List<Card> cardPool;

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
        attackTokenText = GetTextComponent("Attack Token Text");
    }

    private List<Card> ConvertToList(Dictionary<string, Card>.ValueCollection valueCollection)
    {
        List<Card> list = new List<Card>();
        list.AddRange(valueCollection);
        return list;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindTextReferences();

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
            {"Bull Elnuk", new UnitCard("Bull Elnuk", 4, 4, 5)},
            {"Trifarian Shieldbreaker", new UnitCard("Trifarian Shieldbreaker", 5, 6, 5, new List<Keyword> { Keyword.Fearsome })},
            {"Alpha Wildclaw", new UnitCard("Alpha Wildclaw", 6, 7, 6, new List<Keyword> { Keyword.Overwhelm })}
        };

        cardPool = ConvertToList(cardDictionary.Values);

        if (mutation)
        {
            playerADeck = LoadDeckFromJson("Assets/Decks/biglistevolve2.json");
            playerBDeck = LoadDeckFromJson("Assets/Decks/biglistevolve2.json");
            bestDeck = Deck.CopyDeck(playerADeck);

            StreamWriter writer = new StreamWriter(writeFile, true);
            writer.Write("50");
            writer.Close();

            for (int i = 0; i < 3; i++)
            {
                playerADeck.RandomMutate(cardPool);
            }
        }

        else if (evolution)
        {

            for (int i = 0; i < deathmatchParticipants.Length; i++)
            {
                deathmatchParticipants[i] = Deck.RandomDeck("deckdeck" + i, cardPool);
            }

            rng = new System.Random();

            playerADeck = deathmatchParticipants[currentChallenger];
            currentOpponent = rng.Next(deathmatchParticipants.Length);
            while (currentOpponent == currentChallenger)
            {
                currentOpponent = rng.Next(deathmatchParticipants.Length);
            }
            playerBDeck = deathmatchParticipants[currentOpponent];

        }

        else
        {
            playerADeck = LoadDeckFromJson("Assets/Decks/biglistevolve2.json");
            playerBDeck = LoadDeckFromJson("Assets/Decks/biglistevolve2.json");
        }

        ResetGame(true);
    }

    private void ResetGame(bool start = false)
    {

        StreamWriter writer;


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



        if (playedGamesInMatch >= numberOfGamesInMatch) //match finished
        {
            if (mutation)
            {
                float score = (results[1] * 100f + results[0] * 50f) / numberOfGamesInMatch;

                writer = new StreamWriter(writeFile, true);
                writer.Write("\n" + score);
                writer.Close();

                if (score > bestOutcome)
                {
                    Debug.Log("Mutation successful with score: " + score);
                    Debug.Log(playerADeck.ToJSON());
                    bestDeck = Deck.CopyDeck(playerADeck);
                    bestOutcome = score;
                }
                else
                {
                    Debug.Log("Mutation unsuccessful with score: " + score);
                    playerADeck = Deck.CopyDeck(bestDeck);
                }

                for (int i = 0; i < 3; i++)
                {
                    playerADeck.RandomMutate(cardPool);
                }

                results = new int[3];
                playedGamesInMatch = 0;
                playerAGoingFirst = true;
            }
            else if (evolution)
            {
                float score = (results[1] * 100f + results[0] * 50f) / numberOfGamesInMatch;

                if (score >= 45)
                {
                    survived[currentChallenger] = true;
                    wins[currentChallenger] += 1;
                }
                else
                {
                    survived[currentChallenger] = false;
                    wins[currentChallenger] = 0;
                }

                List<Deck> survivors = new List<Deck>();
                if (currentChallenger == deathmatchParticipants.Length - 1)
                {
                    for (int i = 0; i < survived.Length; i++)
                    {
                        if (survived[i])
                        {
                            survivors.Add(deathmatchParticipants[i]);
                        }
                    }

                    for (int i = 0; i < survived.Length; i++)
                    {
                        if (!survived[i])
                        {
                            deathmatchParticipants[i] = Deck.CopyDeck(survivors[rng.Next(survivors.Count)]);
                            for (int j = 0; j < 3; j++)
                            {
                                deathmatchParticipants[j].RandomMutate(cardPool);
                            }
                        }
                    }

                    currentChallenger = 0;
                    playerADeck = deathmatchParticipants[currentChallenger];
                    currentOpponent = rng.Next(deathmatchParticipants.Length);
                    while (currentOpponent == currentChallenger)
                    {
                        currentOpponent = rng.Next(deathmatchParticipants.Length);
                    }
                    playerBDeck = deathmatchParticipants[currentOpponent];

                    string debugString = "";
                    int maxWins = 0;
                    int maxWinsIndex = 0;
                    for (int i = 0; i < wins.Length; i++)
                    {
                        debugString += wins[i] + ", ";
                        if (wins[i] > maxWins)
                        {
                            maxWins = wins[i];
                            maxWinsIndex = i;
                        }
                    }
                    Debug.Log(debugString);
                    Debug.Log("Deck with " + maxWins + " wins: \n" + deathmatchParticipants[maxWinsIndex].ToJSON());
                }

                else
                {
                    currentChallenger += 1;
                    playerADeck = deathmatchParticipants[currentChallenger];
                    currentOpponent = rng.Next(deathmatchParticipants.Length);
                    while (currentOpponent == currentChallenger)
                    {
                        currentOpponent = rng.Next(deathmatchParticipants.Length);
                    }
                    playerBDeck = deathmatchParticipants[currentOpponent];
                }
                results = new int[3];
                playedGamesInMatch = 0;
                playerAGoingFirst = true;
            }
        }

        /**
        //test law of large numbers
        if (!mutation && !evolution)
        {
            float score = (results[1] * 100f + results[0] * 50f) / (results[0] + results[1] + results[2]);

            writer = new StreamWriter(writeFile, true);
            writer.Write("\n" + score);
            writer.Close();
        }
        **/

        //playerADeck = new DeckBuilder2(deckIndexA).deck;
        //playerBDeck = new DeckBuilder2(deckIndexB).deck;

        playerADeck.Reset();    //revert decks to decklists 
        playerBDeck.Reset();    //revert decks to decklists 

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
        return deck.ToDeck(cardDictionary);
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