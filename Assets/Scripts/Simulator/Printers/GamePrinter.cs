using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using System.Linq;

public abstract class GamePrinter : MonoBehaviour
{
    //~~~ Class Fields ~~~

    //--- Game Logic ---
    public LoRGameManager lorGameManager;
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

    //Changes game text to reflect board state.
    protected void UpdateText()
    {
        int[] results = lorGameManager.results;
        Player playerOne = lorGameManager.playerOne;
        Player playerTwo = lorGameManager.playerTwo;

        playerOneHandText.text = "Player One Hand: \n" + board.playerOneSide.hand.ToString();
        playerTwoHandText.text = "Player Two Hand: \n" + board.playerTwoSide.hand.ToString();
        playerOneBenchText.text = "Player One Bench: \n" + board.playerOneSide.bench.ToString();
        playerTwoBenchText.text = "Player Two Bench: \n" + board.playerTwoSide.bench.ToString();
        ManaText.text = "Player One Mana: \n" + board.playerOneSide.mana.ToString() + "\n\nPlayer Two Mana: \n" + board.playerTwoSide.mana.ToString();
        roundNumberText.text = "Round Number: " + board.roundNumber;
        nexusHealthText.text = "Player One Nexus Health: " + board.playerOneSide.nexus.health + "\nPlayer Two Nexus Health: " + board.playerTwoSide.nexus.health;
        playerInfoText.text = "Player 1 is: " + playerOne.name +
                                "\nPlayer 2 is: " + playerTwo.name;
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
        resultText.text = "Ties: " + results[0] + "\nPlayer One Wins: " + results[1] + "\nPlayer Two Wins: " + results[2] + "\nScore: " + ((results[1] * 100f + results[0] * 50f) / results.Sum()) + "%";
    }
}