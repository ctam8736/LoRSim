using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LegalMoveDisplay : MonoBehaviour
{
    public GameObject legalMoveDisplay;
    public GamePrinter printer;
    LegalMoveGenerator lgm;
    LoRBoard board;

    void Update()
    {
        board = printer.board;
        lgm = new LegalMoveGenerator(board);
        legalMoveDisplay.transform.Find("Legal Move Text").gameObject.GetComponent<TextMeshProUGUI>().text = LegalMovesString();
        if (Input.GetKey(KeyCode.Tab))
        {
            legalMoveDisplay.SetActive(true);
        }
        else
        {
            legalMoveDisplay.SetActive(false);
        }
    }

    string LegalMovesString()
    {
        string lgmString = "Active player: " + board.activePlayer;
        {
            lgmString += "\n------------------------------\nLegal Moves:\n";
            foreach (GameAction action in lgm.LegalMoves())
            {
                lgmString += action + "\n";
            }
            lgmString += "------------------------------\n";
        }
        return lgmString;
    }
}