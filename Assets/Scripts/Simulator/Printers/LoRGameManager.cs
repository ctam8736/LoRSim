using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoRGameManager : MonoBehaviour
{
    public LoRBoard board;
    public Game game;

    public event Action OnGameStateUpdated;
    public event Action OnGameEnd;
    private Coroutine gameLoopCoroutine;

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
    public Deck playerADeck;
    public Deck playerBDeck;

    [HideInInspector]
    public string playerGoingFirst;

    public Player playerOne;
    public Player playerTwo;

    //--- RNG ---
    protected System.Random rng;

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
            game.ExecuteAction(action);
        }
        else
        {
            GameAction action = playerTwo.MakeAction();
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
            ResetGame();
            OnGameEnd?.Invoke();

            return true;
        }
        return false;
    }

    //Resets board, deck, and player state upon game end.
    protected virtual void ResetGame(bool start = false)
    {
        board = new LoRBoard();

        playerADeck.Reset();    //revert decks to decklists 
        playerBDeck.Reset();    //revert decks to decklists 

        if (playerGoingFirst == "A")
        {
            playerA = new PlayerX(board, 1, playerADeck);
            playerB = new PlayerX(board, 2, playerBDeck);
            playerOne = playerA;
            playerTwo = playerB;
        }
        else if (playerGoingFirst == "B")
        {
            playerA = new PlayerX(board, 2, playerADeck);
            playerB = new PlayerX(board, 1, playerBDeck);
            playerOne = playerB;
            playerTwo = playerA;
        }

        board.Initialize(playerOne.Deck(), playerTwo.Deck());

        game = new Game(board);
        game.debugging = showRounds;
    }



    // Start is called before the first frame update
    void Awake()
    {
        playerGoingFirst = "A";

        playerADeck = CardData.LoadDeckFromJson(playerADeckString);
        playerBDeck = CardData.LoadDeckFromJson(playerBDeckString);

        ResetGame(true);
        gameLoopCoroutine = StartCoroutine(GameLoop());
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
