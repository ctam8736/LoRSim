using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;
using System.Linq;
using System;
using Codice.Client.BaseCommands.Import;
using System.Text;
using UnityEditor.Experimental.GraphView;

public class EvalPlayer5225 : Player
{
    LegalMoveGenerator lmg;

    public EvalPlayer5225(LoRBoard board, int playerNumber, Deck deck)
    {
        this.board = board;
        this.playerNumber = playerNumber;
        this.deck = deck;
        name = "EvalPlayer";

        lmg = new LegalMoveGenerator(board);
    }

    // Idk exactly why this is necessary
    public override Deck Deck()
    {
        return deck;
    }

    string FormatDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Dictionary contents:");

        foreach (var kvp in dict)
        {
            sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
        }

        return sb.ToString();
    }

    public override GameAction MakeAction()
    {
        List<GameAction> legalMoves = lmg.LegalMoves();

        float bestMoveValue = -1;
        GameAction bestMove = legalMoves[0];

        for (int i = 0; i < legalMoves.Count; i++)
        {
            LoRBoard tmpBoard = ObjectExtensions.Copy(board); // deep copy
            Game tmpGame = new Game(tmpBoard, false);
            LegalMoveGenerator tmplmg = new LegalMoveGenerator(tmpBoard);

            var prevEvaluation = EvaluateLoRBoardState(tmpBoard);
            var move = tmplmg.LegalMoves()[i];
            tmpGame.ExecuteAction(move);
            var currentEvaluation = EvaluateLoRBoardState(tmpBoard);

            var prevValue = prevEvaluation.Values.Sum();
            var currentValue = currentEvaluation.Values.Sum();

            Debug.Log($"Move: {move}, Previous Evaluation: {prevValue}, New Evaluation: {currentValue} \n {FormatDictionary(currentEvaluation)}");
            if (currentValue > bestMoveValue){
                bestMoveValue = currentValue;
                bestMove = legalMoves[i];
            }
        }

        return bestMove;
    }

    public float UnitValue(UnitCard unit)
    {
        return unit.power + unit.health + (unit.HasKeyword(Keyword.Tough) ? 1 : 0) + (unit.HasKeyword(Keyword.Challenger) ? 1 : 0) + 1;
    }

    public Dictionary<string, float> EvaluateLoRBoardState(LoRBoard board)
    {
        LoRBoardSide evalSide;
        LoRBoardSide opposingSide;
        if (playerNumber == 1)
        {
            evalSide = board.playerOneSide;
            opposingSide = board.playerTwoSide;
        }
        else
        {
            evalSide = board.playerTwoSide;
            opposingSide = board.playerOneSide;
        }

        float nexusMultiplier = 1.2f;
        Dictionary<string, float> evalSummary = new Dictionary<string, float>();

        void UpdateEvalDictValue(string key, float value)
        {
            evalSummary[key] = evalSummary.GetValueOrDefault(key) + value;
        }

        if (board.declaringAttacks && board.attackingPlayer == playerNumber) //for declaring attacks
        {
            //UpdateEvalDictValue("Outnumbering When Attacking", Math.Min((evalSide.bench.units.Count + board.battlefield.battlingUnits.Count - opposingSide.bench.units.Count - 1) * 2f, 0));

            List<int> attackerPowers = new List<int>();
            foreach (UnitCard unit in evalSide.bench.units)
            {
                attackerPowers.Add(unit.power);
            }

            List<Battlefield.BattlePair> sortedBattlePairs = board.battlefield.battlingUnits.OrderBy(x=>x.attacker.power).ToList();
            List<int> bestBlockerIndices = new List<int>();
            foreach (Battlefield.BattlePair pair in sortedBattlePairs)
            {
                attackerPowers.Add(pair.attacker.power);
                int attackerEffectiveHealth = pair.attacker.HasKeyword(Keyword.Tough) ? pair.attacker.health + 1 : pair.attacker.health;
                if (pair.blocker != null) //challenging
                {
                    int blockerEffectiveHealth = pair.blocker.HasKeyword(Keyword.Tough) ? pair.blocker.health + 1 : pair.blocker.health;

                    if (!pair.attacker.HasKeyword(Keyword.Barrier))
                    {
                        if (!(pair.blocker.power >= attackerEffectiveHealth))
                        {
                            UpdateEvalDictValue("Attacker Unit Count", UnitValue(pair.attacker));
                            UpdateEvalDictValue("Incoming Combat Damage From Blockers", -(pair.blocker.power - (pair.attacker.HasKeyword(Keyword.Tough) ? 1 : 0)));
                        }
                    }
                    if (!pair.attacker.HasKeyword(Keyword.Barrier))
                    {
                        if (pair.attacker.power >= blockerEffectiveHealth)
                        {
                            UpdateEvalDictValue("Blockers Killed", UnitValue(pair.blocker));
                        } else {
                            UpdateEvalDictValue("Outgoing Combat Damage From Attackers", (pair.attacker.power - (pair.blocker.HasKeyword(Keyword.Tough) ? 1 : 0)));
                        }
                    }
                } else {
                    int bestBlockerIndex = -1;
                    float valueGivenBestBlocker = (pair.attacker.power * nexusMultiplier) + UnitValue(pair.attacker);
                    for (int blockerIndex = 0; blockerIndex < opposingSide.bench.units.Count; blockerIndex++)
                    {
                        if (bestBlockerIndices.Contains(blockerIndex)){
                            continue;
                        }
                        var blocker = opposingSide.bench.units[blockerIndex];
                        float valueGivenCurrentBlocker = 0;
                        int blockerEffectiveHealth = blocker.HasKeyword(Keyword.Tough) ? blocker.health + 1 : blocker.health;

                        if (!pair.attacker.HasKeyword(Keyword.Barrier))
                        {
                            if (!(blocker.power >= attackerEffectiveHealth))
                            {
                                valueGivenCurrentBlocker += UnitValue(pair.attacker); //attacker alive
                                valueGivenCurrentBlocker -= blocker.power - (pair.attacker.HasKeyword(Keyword.Tough) ? 1 : 0); //incoming blocker damage
                            }
                        }
                        if (!pair.attacker.HasKeyword(Keyword.Barrier))
                        {
                            if (pair.attacker.power >= blockerEffectiveHealth)
                            {
                                valueGivenCurrentBlocker += UnitValue(blocker); //blocker killed
                            }
                            else
                            {
                                valueGivenCurrentBlocker += pair.attacker.power - (blocker.HasKeyword(Keyword.Tough) ? 1 : 0); //outgoing combat damage
                            }
                        }

                        if (valueGivenCurrentBlocker < valueGivenBestBlocker) //minimize this
                        {
                            valueGivenBestBlocker = valueGivenCurrentBlocker;
                            bestBlockerIndex = blockerIndex;
                        }
                    }
                    if (bestBlockerIndex != -1)
                    {
                        bestBlockerIndices.Add(bestBlockerIndex);
                    }
                    UpdateEvalDictValue("Attack Value", valueGivenBestBlocker);
                }
            }

            int[] sorted = attackerPowers.OrderBy(n => n).ToArray();
            int outgoingDamage = sorted.Take(evalSide.bench.units.Count + board.battlefield.battlingUnits.Count - opposingSide.bench.units.Count).Sum();
            if (outgoingDamage >= opposingSide.nexus.health)
            {
                UpdateEvalDictValue("Lethal Detected", 10000);
            } //else
            //{
            //    UpdateEvalDictValue("Outgoing Nexus Damage", outgoingDamage);
            //}
        }

        else if (board.inCombat) //for already in combat
        {
            if (evalSide.isAttacking)
            {
                foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                {
                    int attackerEffectiveHealth = pair.attacker.HasKeyword(Keyword.Tough) ? pair.attacker.health + 1 : pair.attacker.health;
                    UpdateEvalDictValue("Attacker Unit Count", UnitValue(pair.attacker));
                }
            }
            else
            {
                int damageToNexus = 0;
                foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                {
                    if (pair.blocker != null)
                    {
                        int attackerEffectiveHealth = pair.attacker.HasKeyword(Keyword.Tough) ? pair.attacker.health + 1 : pair.attacker.health;
                        int blockerEffectiveHealth = pair.attacker.HasKeyword(Keyword.Tough) ? pair.blocker.health + 1 : pair.blocker.health;

                        if (!pair.attacker.HasKeyword(Keyword.Barrier)) //damage onto attacker
                        {
                            if (pair.blocker.power >= attackerEffectiveHealth)
                            {
                                UpdateEvalDictValue("Attackers Killed", UnitValue(pair.attacker));
                            }
                            else
                            {
                                UpdateEvalDictValue("Outgoing Combat Damage From Blockers", pair.blocker.power - (pair.attacker.HasKeyword(Keyword.Tough) ? 1 : 0));
                            }
                        }
                        if (!(pair.attacker.power >= blockerEffectiveHealth)) //damage onto blocker
                        {
                            if (!pair.blocker.HasKeyword(Keyword.Barrier))
                            {
                                UpdateEvalDictValue("Incoming Combat Damage From Attackers", -(pair.attacker.power - (pair.blocker.HasKeyword(Keyword.Tough) ? 1 : 0)));
                            }
                            UpdateEvalDictValue("Blocker Unit Count", UnitValue(pair.blocker));
                        }
                    } else
                    {
                        damageToNexus += pair.attacker.power;
                    }
                }

                if (damageToNexus > evalSide.nexus.health)
                {
                    UpdateEvalDictValue("Lethal Incoming", -10000);
                }
                UpdateEvalDictValue("Incoming Damage", -(damageToNexus * nexusMultiplier));
            }
        }

        if (board.battlefield.battlingUnits.Count > 0 && board.activeSpell != null && board.activeSpell.name == "Single Combat") //just don't do it in combat
        {
            UpdateEvalDictValue("Single Combat", -10);
        }

        for (int i = 0; i <  board.spellStack.spells.Count; i++) //spell on stack value
        {
            if (board.spellStack.castingOrder[i] == playerNumber)
            {
                switch (board.spellStack.spells[i].name)
                {
                    case "Reinforcements":
                        UpdateEvalDictValue("Reinforcements", 18);
                        break;
                    case "For Demacia!":
                        if (evalSide.hasAttackToken && (evalSide.bench.units.Count - opposingSide.bench.units.Count >= 0)) { UpdateEvalDictValue("For Demacia!", evalSide.bench.units.Count * 4); }
                        break;
                    case "Succession":
                        UpdateEvalDictValue("Succession", 7);
                        break;
                    case "Relentless Pursuit":
                        if (!evalSide.hasAttackToken) { UpdateEvalDictValue("Relentless Pursuit", (evalSide.bench.units.Count - 1 - opposingSide.bench.units.Count) * 2); }
                        break;
                    case "Single Combat":
                        if (!board.spellStack.spells[i].NeedsTargets()){
                            UnitCard alliedTarget = ((UnitCard)board.spellStack.spells[i].targets[0]);
                            UnitCard enemyTarget;
                            if (board.spellStack.spells[i].targets.Count == 1)
                            {
                                enemyTarget = opposingSide.bench.units.OrderBy(x => x.power).First();
                            }
                            else
                            {
                                enemyTarget = ((UnitCard)board.spellStack.spells[i].targets[1]);
                            }
                            if (!enemyTarget.HasKeyword(Keyword.Barrier))
                            {
                                if (alliedTarget.power >= enemyTarget.health + (enemyTarget.HasKeyword(Keyword.Tough) ? 1 : 0)) //see barrier
                                {
                                    UpdateEvalDictValue("Single Combat", UnitValue(enemyTarget));
                                }
                                else
                                {
                                    UpdateEvalDictValue("Single Combat", (alliedTarget).power - (enemyTarget.HasKeyword(Keyword.Tough) ? 1 : 0));
                                }
                            }
                            if (!alliedTarget.HasKeyword(Keyword.Barrier))
                            {
                                if (enemyTarget.power >= alliedTarget.health + (alliedTarget.HasKeyword(Keyword.Tough) ? 1 : 0))
                                {
                                    UpdateEvalDictValue("Single Combat", -UnitValue(alliedTarget));
                                }
                                else
                                {
                                    UpdateEvalDictValue("Single Combat", -(enemyTarget.power - (alliedTarget.HasKeyword(Keyword.Tough) ? 1 : 0)));
                                }
                            }
                        }
                        break;
                }
            }
        }

        UpdateEvalDictValue("Nexus Health", evalSide.nexus.health * nexusMultiplier);
        if (evalSide.nexus.health < 1)
        {
            UpdateEvalDictValue("Incoming Lethal", -10000);
        }

        if ((board.passCount == 1 && board.activePlayer != playerNumber && !board.inCombat) || (evalSide.mana.manaGems == board.roundNumber &&  opposingSide.mana.manaGems == board.roundNumber)) // for pass burn
        {
            UpdateEvalDictValue("Benched Units (Passing)", evalSide.bench.units.Sum(x => x.grantedPower + Math.Min(x.health, x.grantedHealth) + (x.HasKeyword(Keyword.Tough) ? 1 : 0)));
            UpdateEvalDictValue("Mana (Passing)", Math.Min(3 * .7f, (evalSide.mana.manaGems + evalSide.mana.spellMana) * .7f));
        }
        else
        {
            UpdateEvalDictValue("Benched Units", evalSide.bench.units.Sum(x => UnitValue(x)));
            UpdateEvalDictValue("Mana", evalSide.mana.manaGems + evalSide.mana.spellMana * .7f);
        }

        //value -= opposingSide.mana.manaGems + opposingSide.mana.spellMana * .7f; //discount opponent manaGems (idea, prevent pass-pass)
        return evalSummary;
    }
}