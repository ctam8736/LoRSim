using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class NexusTracker : GamePrinter
{
    List<int> p1HealthStats = new List<int>();
    List<int> p2HealthStats = new List<int>();

    int currentRoundNumber = 0;
    bool p1First = true;
    void Start()
    {
        board = lorGameManager.board;
        game = lorGameManager.game;
    }

    void OnEnable()
    {
        lorGameManager.OnGameStateUpdated += UpdateHealthStats;
        lorGameManager.OnGameEnd += WriteHealthStats;
        lorGameManager.OnPlayerMove += WriteMoves;
    }

    // Start is called before the first frame update
    void UpdateHealthStats()
    {
        if (board.roundNumber != currentRoundNumber){
            p1HealthStats.Add(board.playerOneSide.nexus.health);
            p2HealthStats.Add(board.playerTwoSide.nexus.health);
            currentRoundNumber += 1;
        }
    }

    void WriteHealthStats()
    {
        p1HealthStats.Add(Math.Max(board.playerOneSide.nexus.health, 0));
        p2HealthStats.Add(Math.Max(board.playerTwoSide.nexus.health, 0));

        List<int> pAHealthStats;
        List<int> pBHealthStats;

        pAHealthStats = p1First ? p1HealthStats : p2HealthStats;
        pBHealthStats = p1First ? p2HealthStats : p1HealthStats;

        StreamWriter writer = new StreamWriter("Assets/health_stats.txt", true);
        for (int i = 0; i < pAHealthStats.Count; i++)
        {
            writer.Write($"{pAHealthStats[i]}, ");
        }
        writer.Write("\n");
        for (int i = 0; i < pBHealthStats.Count; i++)
        {
            writer.Write($"{pBHealthStats[i]}, ");
        }
        writer.Write("\n");
        writer.Close();

        p1First = !p1First;
        p1HealthStats.Clear();
        p2HealthStats.Clear();
        board = lorGameManager.board;
        game = lorGameManager.game;
        currentRoundNumber = 0;
    }

    void WriteMoves(MoveData moveData)
    {
        StreamWriter writer = new StreamWriter("Assets/move_data.txt", true);
        if (moveData.action.command == "Play") {
            writer.Write($"{moveData.playerNumber},{((Card)(moveData.action.target)).name},{moveData.roundNumber}\n");
        }
        writer.Close();
    }

    public class GameData
    {
        public Dictionary<string, int> playerOneCardsPlayed = new Dictionary<string, int>();
        public Dictionary<string, int> playerTwoCardsPlayed = new Dictionary<string, int>();
        public List<int> playerOneNexusHealthPerRound = new List<int>();
        public List<int> playerTwoNexusHealthPerRound = new List<int>();
    }
}
