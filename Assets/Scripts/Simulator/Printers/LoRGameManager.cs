using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoRGameManager : MonoBehaviour
{
    public LoRBoard board;
    public Game game;

    public delegate void OnPlayerMoveHandler(MoveData data);
    public event Action OnGameStateUpdated;
    public event Action OnGameEnd;
    public event Action<MoveData> OnPlayerMove;

    //--- Public variables ---
    public string playerADeckString;
    public string playerBDeckString;

    //--- Play Timer Logic ---
    public float turnDelay = 1f;
    public bool showRounds;
    public int gamesPerFrame = 100;
    protected float timer = 0f;

    //--- Match variables ---
    [HideInInspector]
    public int[] results = new int[3]; // win, draw, loss

    //--- Card libraries ---
    protected List<Card> cardPool;

    //--- Player Logic ---
    public Player playerA;
    public Player playerB;
    public Deck deckOne;
    public Deck deckTwo;
    public Deck deckOneCopy;
    public Deck deckTwoCopy;

    [HideInInspector]
    public string playerGoingFirst;

    public Player playerOne;
    public Player playerTwo;

    //--- RNG ---
    protected System.Random rng;

    private int gameInPair = 1; // 1 (first) or 2 (second)

    //Fills dictionary with cards.
    protected virtual void InitializeDictionary()
    {
        List<Card> list = new List<Card>();
        list.AddRange(CardData.cardDictionary.Values);
        cardPool = list;
    }

    //~~~ Class Game Logic Methods ~~~

    //Completes one game turn, calling for an action from the active player.
    protected void PlayGameTurn()
    {
        if (board.activePlayer == 1)
        {
            GameAction action = playerOne.MakeAction();
            OnPlayerMove?.Invoke(new MoveData(action, 1, board.roundNumber));
            game.ExecuteAction(action);
        }
        else
        {
            GameAction action = playerTwo.MakeAction();
            OnPlayerMove?.Invoke(new MoveData(action, 2, board.roundNumber));
            game.ExecuteAction(action);
        }
    }

    //Updates results and handles reset if game has terminated.
    protected virtual bool HandleGameEnd()
    {
        //update results
        int result = board.gameResult;
        if (result != -1)
        {
            if (result == 0)
            {
                results[result] += 1;
            }

            else if (playerGoingFirst == "A")
            {
                results[result] += 1;
            }
            
            else if (playerGoingFirst == "B")
            {
                results[3 - result] += 1;
            }

            playerGoingFirst = (playerGoingFirst == "B") ? "A" : "B";
            gameInPair = 3 - gameInPair;
            ResetGame();
            OnGameEnd?.Invoke();

            return true;
        }

        return false;
    }

    //Resets board, deck, and player state upon game end.
    protected virtual void ResetGame()
    {
        board = new LoRBoard();

        if (gameInPair == 1)
        {
            deckOne.Reset();    //revert decks to decklists 
            deckTwo.Reset();    //revert decks to decklists
            deckOneCopy = ObjectExtensions.Copy(deckOne);
            deckTwoCopy = ObjectExtensions.Copy(deckTwo);
        }

        if (playerGoingFirst == "A")
        {
            playerOne = new EvalPlayer(board, 1, deckOne);
            playerTwo = new EvalPlayer5225(board, 2, deckTwo);
        }
        else if (playerGoingFirst == "B")
        {
            playerOne = new EvalPlayer5225(board, 1, deckOneCopy);
            playerTwo = new EvalPlayer(board, 2, deckTwoCopy);
        }

        board.Initialize(playerOne.Deck(), playerTwo.Deck());

        game = new Game(board);
        game.debugging = showRounds;
    }



    // Start is called before the first frame update
    void Awake()
    {
        playerGoingFirst = "A";

        deckOne = CardData.LoadDeckFromJson(playerADeckString);
        deckTwo = CardData.LoadDeckFromJson(playerBDeckString);

        ResetGame();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            if (showRounds)
            {
                yield return new WaitForSeconds(turnDelay);
                HandleGameEnd();
                PlayGameTurn();

                OnGameStateUpdated?.Invoke();
            } else {
                yield return null;
                for (int i = 0; i < gamesPerFrame; i++)
                {
                    while (!HandleGameEnd())
                    {
                        PlayGameTurn();
                    }
                }
            }
            OnGameStateUpdated?.Invoke();
        }
    }
}

public class MoveData
{
    public GameAction action;
    public int playerNumber;
    public int roundNumber;

    public MoveData(GameAction action, int playerNumber, int roundNumber)
    {
        this.action = action;
        this.playerNumber = playerNumber;
        this.roundNumber = roundNumber;
    }
}
