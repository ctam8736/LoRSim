using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a given Legends of Runeterra game. Takes inputs in the form of Actions to the method ExecuteAction, which modifies the internal LoRBoard.
/// </summary>
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
    public bool ExecuteAction(Action action)
    {

        switch (action.command)
        {

            case "Attack":

                if (action.units != null)
                {
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
                }
                else if (action.attacker != null)
                {
                    if (debugging)
                    {
                        Debug.Log("Player " + board.activePlayer + " attacks with " + action.attacker.ToString());
                    }

                    board.DeclareSingleAttack(action.attacker);
                    break;
                }
                else
                {
                    break;
                }

            case "Block":

                if (action.pairs != null)
                {

                    if (debugging)
                    {
                        foreach (Battlefield.BattlePair pair in action.pairs)
                        {
                            Debug.Log("Player " + board.activePlayer + " blocks " + pair.attacker.ToString() + " with " + pair.blocker.ToString());
                        }
                    }

                    board.DeclareBlock(action.pairs);
                    break;
                }
                else if (action.attacker != null)
                {
                    if (debugging)
                    {
                        Debug.Log("Player " + board.activePlayer + " blocks " + action.attacker.ToString() + " with " + action.blocker.ToString());
                    }

                    if (action.attacker.HasKeyword(Keyword.Elusive) && !action.blocker.HasKeyword(Keyword.Elusive)) //check elusive, does not handle sharpsight yet :P
                    {
                        Debug.Log("Illegal: " + action.attacker.name + " cannot be blocked by " + action.blocker.name);
                        return false;
                    }
                    else
                    {
                        board.DeclareSingleBlock(action.attacker, action.blocker);
                        break;
                    }
                }
                else
                {
                    break;
                }

            case "Challenge":

                if (debugging)
                {
                    Debug.Log("Player " + board.activePlayer + " is challenging " + action.blocker.name + " with " + action.attacker.name + ".");
                }

                board.DeclareChallenge(action.attacker, action.blocker);
                break;

            case "Pass":

                if (debugging)
                {
                    if (board.declaringAttacks)
                    {
                        Debug.Log("Player " + board.activePlayer + " confirms attacks.");
                    }
                    else if (board.declaringBlocks)
                    {
                        Debug.Log("Player " + board.activePlayer + " confirms blocks.");
                    }
                    else if (board.casting)
                    {
                        Debug.Log("Player " + board.activePlayer + " confirms spell casts.");
                    }
                    else if (board.inCombat)
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

            default:
                break;
        }

        if (board.roundNumber > currentRoundNumber)
        {
            if (debugging)
            {
                Debug.Log("\nAdvanced to round " + board.roundNumber + ".");
                currentRoundNumber += 1;
            }
        }

        return true;
    }
}