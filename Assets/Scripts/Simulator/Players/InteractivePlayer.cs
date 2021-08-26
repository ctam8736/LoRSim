using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//TODO: The InteractivePlayer class translates user actions into game Actions to manipulate the internal board.
public class InteractivePlayer : Player
{
    LoRBoardSide mySide;
    LoRBoardSide opposingSide;
    Nexus nexus;
    Nexus opposingNexus;

    List<object> intendedTargets = new List<object>();

    public InteractivePlayer(LoRBoard board, int playerNumber, Deck deck)
    {
        this.board = board;
        this.playerNumber = playerNumber;
        this.deck = deck;

        if (playerNumber == 1)
        {
            mySide = board.playerOneSide;
            opposingSide = board.playerTwoSide;
        }
        else
        {
            mySide = board.playerTwoSide;
            opposingSide = board.playerOneSide;
        }

        SetUpRelativeVariables();
    }

    private void SetUpRelativeVariables()
    {
        bench = mySide.bench;
        hand = mySide.hand;
        mana = mySide.mana;
        nexus = mySide.nexus;

        opposingBench = opposingSide.bench;
        opposingHand = opposingSide.hand;
        opposingMana = opposingSide.mana;
        opposingNexus = opposingSide.nexus;
    }

    public override Action MakeAction()
    {
        return null;
    }

    public override Deck Deck()
    {
        return deck;
    }
}