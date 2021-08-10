using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{
    public LoRBoard board;
    public bool debugging = true;

    int currentRoundNumber = 0;

    public Game(LoRBoard board)
    {
        this.board = board;
    }

    /// <summary>
    /// Makes a call to LoRBoard to execute the given action.
    /// </summary>
    public void ExecuteAction(Action action)
    {
        if (board.roundNumber > currentRoundNumber)
        {
            if (debugging)
            {
                Debug.Log("\nAdvanced to round " + board.roundNumber + ".");
                currentRoundNumber += 1;
            }
        }

        switch (action.command)
        {
            case "Play":

                if (debugging)
                {
                    if (action.target is Card)
                    {
                        Debug.Log("Player " + board.activePlayer + " plays " + ((Card)action.target).name + ".");
                    }
                }

                if (action.target is UnitCard)
                {
                    board.PlayUnit((UnitCard)action.target);
                }

                if (action.target is SpellCard)
                {
                    board.PlaySpell((SpellCard)action.target);
                }
                break;

            case "Target":
                if (debugging)
                {
                    if (action.target is UnitCard)
                    {
                        Debug.Log("Player " + board.activePlayer + " has targeted " + ((UnitCard)action.target).name + " with " + board.activeSpell.name + ".");
                    }
                    else if (action.target is Nexus)
                    {
                        Debug.Log("Player " + board.activePlayer + " has targeted a nexus with " + board.activeSpell.name + ".");
                    }
                }

                if (action.target is UnitCard)
                {
                    board.AssignTarget((UnitCard)action.target);
                }
                else if (action.target is Nexus)
                {
                    board.AssignTarget((Nexus)action.target);
                }
                break;

            case "Attack":

                if (debugging)
                {
                    if (action.units.Count != 0)
                    {
                        string unitString = "";
                        foreach (UnitCard unit in action.units)
                        {
                            unitString += unit.ToString() + ", ";
                        }
                        unitString = unitString.Substring(0, unitString.Length - 2);
                        Debug.Log("Player " + board.activePlayer + " attacks with: " + unitString);
                    }
                    else
                    {
                        Debug.Log("Player " + board.activePlayer + " confirms attacks.");
                    }
                }

                board.DeclareAttack(action.units);
                break;

            case "Challenge":

                if (debugging)
                {
                    Debug.Log("Player " + board.activePlayer + " is challenging " + action.pair.blocker.name + " with " + action.pair.attacker.name + ".");
                }

                board.DeclareChallenge(action.pair);
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

    /// <summary>
    //Returns 1 if Player 1 won, 2 if Player 2 won, 0 if tie, and -1 if unterminated.
    /// </summary>
    public int GameResult()
    {
        //win by nexus health
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

        //win by decking
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