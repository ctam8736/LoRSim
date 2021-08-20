using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

//WIP returns board evaluations
public class EvaluatorA
{
    LoRBoard board;

    public EvaluatorA(LoRBoard board)
    {
        this.board = board;
    }

    public float BoardEvaluation(int player)
    {
        float value = 0;
        LoRBoardSide evalSide;
        LoRBoardSide opposingSide;
        if (player == 1)
        {
            evalSide = board.playerOneSide;
            opposingSide = board.playerTwoSide;
        }
        else
        {
            evalSide = board.playerTwoSide;
            opposingSide = board.playerOneSide;
        }

        value += evalSide.bench.units.Sum(x => x.power + x.health);
        value -= opposingSide.bench.units.Sum(x => x.power + x.health);
        return value;
    }

    public float ActionEvaluation()
    {
        return 0;
    }
}