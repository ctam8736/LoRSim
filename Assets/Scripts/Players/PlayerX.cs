using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerX : Player
{
    LoRBoard board;
    LoRBoardSide mySide;
    LoRBoardSide opposingSide;
    int playerNumber;

    Deck deck;
    Bench bench;
    Bench opposingBench;
    Hand hand;
    Hand opposingHand;
    Mana mana;
    Mana opposingMana;
    Nexus nexus;
    Nexus opposingNexus;

    object intendedTarget = null;
    public PlayerX(LoRBoard board, int playerNumber, Deck deck)
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

    public override Deck Deck()
    {
        return deck;
    }

    private bool HasAttackToken()
    {
        return mySide.hasAttackToken;
    }

    private bool OpponentHasAttackToken()
    {
        return opposingSide.hasAttackToken;
    }

    private bool IsAttacking()
    {
        return playerNumber == board.attackingPlayer;
    }

    public override Action MakeAction()
    {
        //---Complete Targeting---
        if (board.targeting)
        {
            if (intendedTarget != null)
            {
                if (intendedTarget is UnitCard)
                {
                    return new Action("Target", (UnitCard)intendedTarget);
                }
                else if (intendedTarget is Nexus)
                {
                    return new Action("Target", (Nexus)intendedTarget);
                }
                else
                {
                    Debug.Log("Intended target not found...");
                }
            }
        }

        if (board.inCombat)
        {
            if (board.blocked)
            {
                //---Save Unit With Health Potion---
                SpellCard HPPot = null;
                foreach (Card card in hand.cards)
                {
                    if (card.name == "Health Potion" && mana.manaGems + mana.spellMana >= card.cost)
                    {
                        HPPot = (SpellCard)card;
                        break;
                    }
                }

                if (HPPot != null)
                {
                    foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                    {
                        UnitCard attacker = pair.attacker;
                        UnitCard blocker = pair.blocker;
                        if (pair.blocker == null) break;
                        if (IsAttacking())
                        {
                            if (blocker.power > attacker.health && blocker.power < Math.Min(attacker.health + 3, attacker.initialHealth))
                            {
                                intendedTarget = attacker;
                                return new Action("Play", HPPot);
                            }
                        }
                        else
                        {
                            if (attacker.power > blocker.health && attacker.power < Math.Min(blocker.health + 3, blocker.initialHealth))
                            {
                                intendedTarget = blocker;
                                return new Action("Play", HPPot);
                            }
                        }
                    }
                }
                return new Action("Pass");
            }


            //---Determine Blocks---
            List<Battlefield.BattlePair> blocks = new List<Battlefield.BattlePair>();
            List<UnitCard> blockers = new List<UnitCard>();
            List<UnitCard> blockedAttackers = new List<UnitCard>();
            int incomingDamage = 0;
            foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
            {
                incomingDamage += pair.attacker.power;
            }


            foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
            {
                UnitCard attacker = pair.attacker;
                UnitCard bestBlocker = null;
                foreach (UnitCard unit in bench.units)
                {
                    //rules
                    if (unit.HasKeyword(Keyword.CantBlock)) { }
                    else if (attacker.HasKeyword(Keyword.Fearsome) && unit.power < 3) { }
                    else if (attacker.HasKeyword(Keyword.Elusive) && !unit.HasKeyword(Keyword.Elusive)) { }

                    else if (!blockers.Contains(unit))
                    {

                        //can kill
                        if (unit.power >= attacker.health && !attacker.HasKeyword(Keyword.QuickAttack))
                        {
                            if (bestBlocker == null)
                            {
                                bestBlocker = unit;
                            }
                        }

                        //or survive
                        if (unit.health > attacker.power || (unit.health == attacker.power && !attacker.HasKeyword(Keyword.QuickAttack)))
                        {
                            if (bestBlocker == null)
                            {
                                bestBlocker = unit;
                            }
                        }
                    }
                }

                //add blocker
                if (bestBlocker != null)
                {
                    blocks.Add(new Battlefield.BattlePair(attacker, bestBlocker));
                    blockers.Add(bestBlocker);
                    blockedAttackers.Add(attacker);
                    if (attacker.HasKeyword(Keyword.Overwhelm) && attacker.power > bestBlocker.health)
                    {
                        incomingDamage -= bestBlocker.health;
                    }
                    else
                    {
                        incomingDamage -= attacker.power;
                    }
                }
            }

            //---Detect And Prevent Lethal---
            if (incomingDamage > nexus.health)
            {
                foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                {
                    if (incomingDamage < nexus.health) break;

                    UnitCard attacker = pair.attacker;
                    if (!blockedAttackers.Contains(attacker)) //if not blocked
                    {
                        foreach (UnitCard unit in bench.units)
                        {
                            //rules
                            if (unit.HasKeyword(Keyword.CantBlock)) { }
                            else if (attacker.HasKeyword(Keyword.Fearsome) && unit.power < 3) { }
                            else if (attacker.HasKeyword(Keyword.Elusive) && !unit.HasKeyword(Keyword.Elusive)) { }

                            else if (!blockers.Contains(unit)) //and unit is free, then block
                            {
                                UnitCard bestBlocker = unit;
                                blocks.Add(new Battlefield.BattlePair(attacker, bestBlocker));
                                blockers.Add(bestBlocker);
                                blockedAttackers.Add(attacker);
                                if (attacker.HasKeyword(Keyword.Overwhelm) && attacker.power > bestBlocker.health)
                                {
                                    incomingDamage -= bestBlocker.health;
                                }
                                else
                                {
                                    incomingDamage -= attacker.power;
                                }
                            }
                        }
                    }
                }
            }

            if (blocks.Count > 0)
            {
                return new Action("Block", blocks);
            }

            return new Action("Pass");
        }

        //---Attack With Numbers Advantage---
        if (HasAttackToken() && bench.units.Count > opposingBench.units.Count + 1)
        {
            return new Action("Attack", bench.units);
        }

        //determine strongest card in hand
        UnitCard bestUnit = null;
        SpellCard bestSpell = null;
        foreach (Card card in hand.cards)
        {
            if (card is UnitCard)
            {
                if (mana.manaGems >= card.cost)
                {
                    if (bestUnit == null || playValue(bestUnit) < playValue((UnitCard)card))
                    {
                        bestUnit = (UnitCard)card;
                    }
                }
            }

            if (card is SpellCard)
            {
                if (mana.manaGems + mana.spellMana >= card.cost)
                {
                    bestSpell = (SpellCard)card;
                }
            }
        }

        /**
        //avoid playing into stronger opponent play
        if (bestUnit != null && HasAttackToken() && bestUnit.cost < opposingMana.manaGems - 4)
        {
            return new Action("Attack", bench.units);
        }
        **/

        //---Play Best Unit---
        if (bestUnit != null && !bench.IsFull())
        {
            return new Action("Play", bestUnit);
        }

        //---Play a Spell---
        if (bestSpell != null)
        {
            if (bestSpell.name == "Health Potion" && nexus.health < 20)
            {
                intendedTarget = nexus;
            }

            if (!(board.SpellsAreActive() && (bestSpell.spellType == SpellType.Slow || bestSpell.spellType == SpellType.Focus)))
                return new Action("Play", bestSpell);
        }

        //---Declare Attack With All---
        if (HasAttackToken() && bench.units.Count > 0)
        {
            /**
            List<UnitCard> attackers = new List<UnitCard>();
            foreach (UnitCard unit in bench.units)
            {
                bool shouldAttack = true;

                foreach (UnitCard opposingUnit in opposingBench.units)
                {
                    if (!(opposingUnit.HasKeyword(Keyword.CantBlock)))
                    {
                        if (opposingUnit.power > unit.health && opposingUnit.health > unit.power)
                        {
                            shouldAttack = false;
                        }
                    }
                }

                if (shouldAttack)
                {
                    attackers.Add(unit);
                }

            }

            if (attackers.Count > 0)
            {
                return new Action("Attack", attackers);
            }
            **/

            return new Action("Attack", bench.units);
        }

        //---Pass If Nothing Else---
        return new Action("Pass");
    }

    public float playValue(UnitCard unit)
    {
        return (unit.power + unit.health);
        //return (unit.power + unit.health) * (OpponentHasAttackToken() && unit.HasKeyword(Keyword.CantBlock) ? 0 : 1);
    }
}