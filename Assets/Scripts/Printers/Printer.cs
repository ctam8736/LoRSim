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
    protected TextMeshProUGUI spellStackText;

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
    protected List<Card> cardPool;

    //--- RNG ---
    protected System.Random rng;

    //~~~ Class Initialization Methods ~~~

    //Finds TMPro text object by name.
    protected TextMeshProUGUI GetTextComponentSelf(string name)
    {
        return GetTextComponent(this.gameObject, name);
    }

    //Finds TMPro text object by name.
    protected TextMeshProUGUI GetTextComponent(GameObject parent, string name)
    {
        return parent.transform.Find(name).gameObject.GetComponent<TextMeshProUGUI>();
    }

    //Links TMProUGUI GameObjects to variables.
    protected void FindTextReferences()
    {

        playerOneHandText = GetTextComponentSelf("Player One Hand Text");
        playerTwoHandText = GetTextComponentSelf("Player Two Hand Text");
        ManaText = GetTextComponentSelf("Mana Text");
        playerOneBenchText = GetTextComponentSelf("Player One Bench Text");
        playerTwoBenchText = GetTextComponentSelf("Player Two Bench Text");
        battlefieldText = GetTextComponentSelf("Battlefield Text");
        roundNumberText = GetTextComponentSelf("Round Number Text");
        nexusHealthText = GetTextComponentSelf("Nexus Health Text");
        resultText = GetTextComponentSelf("Result Text");
        playerInfoText = GetTextComponentSelf("Player Info Text");
        attackTokenText = GetTextComponentSelf("Attack Token Text");
        spellStackText = GetTextComponentSelf("Spell Stack Text");
    }

    //Fills dictionary with cards.
    protected virtual void InitializeDictionary()
    {

        cardPool = ConvertToList(CardData.cardDictionary.Values);
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
        spellStackText.text = "Spell Stack: \n" + board.spellStack.ToString();
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
    protected virtual bool HandleGameEnd()
    {
        //update results
        int result = board.gameResult;
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
    protected virtual void ResetGame(bool start = false)
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

        if (!showRounds)
        {
            game.debugging = false;
        }
        else
        {
            game.debugging = true;
        }
    }
}