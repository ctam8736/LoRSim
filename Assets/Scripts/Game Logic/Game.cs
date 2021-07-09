using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public LoRBoard board;
    public bool debugging = true;

    public Game(LoRBoard board)
    {
        this.board = board;
    }

    public void ExecuteAction(Action action)
    {
        switch (action.command)
        {
            case "Play":

                if (debugging)
                {
                    Debug.Log("Player " + board.activePlayer + " plays " + action.target.name + ".");
                }

                board.PlayUnit(action.target);
                break;

            case "Attack":

                if (debugging)
                {
                    string unitString = "";
                    foreach (UnitCard unit in action.units)
                    {
                        unitString += unit.ToString() + ", ";
                    }
                    unitString = unitString.Substring(0, unitString.Length - 2);
                    Debug.Log("Player " + board.activePlayer + " attacks with: " + unitString);
                }

                board.DeclareAttack(action.units);
                break;

            case "Block":

                if (debugging)
                {
                    foreach (Battlefield.BattlePair pair in action.pairs)
                    {
                        Debug.Log("Player " + board.activePlayer + " blocks " + pair.attacker.ToString() + " with " + pair.blocker.ToString());
                    }
                }

                board.DeclareBlock(action.pairs);
                break;

            case "Pass":

                if (debugging)
                {
                    if (board.inCombat)
                    {
                        Debug.Log("Player " + board.activePlayer + " passes combat.");
                    }
                    else
                    {
                        Debug.Log("Player " + board.activePlayer + " passes.");
                    }
                }

                board.Pass();
                break;

            default:
                break;
        }
    }

    public int GameResult()
    {
        int result = -1;
        if (board.playerOneSide.nexus.health <= 0 && board.playerTwoSide.nexus.health <= 0)
        {
            result = 0;
        }
        else if (board.playerOneSide.nexus.health <= 0)
        {
            result = 2;
        }
        else if (board.playerTwoSide.nexus.health <= 0)
        {
            result = 1;
        }

        else if (board.playerOneSide.deck.cards.Count == 0 && board.playerTwoSide.deck.cards.Count == 0)
        {
            result = 0;
        }
        else if (board.playerOneSide.deck.cards.Count == 0)
        {
            result = 2;
        }
        else if (board.playerTwoSide.deck.cards.Count == 0)
        {
            result = 1;
        }

        if (debugging)
        {
            if (result == 0)
            {
                Debug.Log("Game ended in a tie.");
            }
            else if (result == 1)
            {
                Debug.Log("Player 1 won.");
            }
            else if (result == 2)
            {
                Debug.Log("Player 2 won.");
            }
        }

        return result;
    }
}