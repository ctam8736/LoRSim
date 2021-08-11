using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PlayerY : Player
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

    List<object> intendedTargets = new List<object>();

    private bool declaringAttack = false;

    public PlayerY(LoRBoard board, int playerNumber, Deck deck)
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
            if (intendedTargets.Count > 0)
            {
                object target = intendedTargets[0];
                intendedTargets.RemoveAt(0);
                if (target is UnitCard)
                {
                    return new Action("Target", (UnitCard)target);
                }
                else if (target is Nexus)
                {
                    return new Action("Target", (Nexus)target);
                }
                else
                {
                    Debug.Log("Intended target not found...");
                }
            }
        }

        if (board.SpellsAreActive())
        {
            /**
            //---All-In Mystic Shot Burn---
            SpellCard mShot = null;
            foreach (Card card in hand.cards)
            {
                if (card.name == "Mystic Shot" && mana.manaGems + mana.spellMana >= card.cost)
                {
                    mShot = (SpellCard)card;
                    break;
                }
            }

            if (mShot != null)
            {
                intendedTargets.Add(opposingNexus);
                return new Action("Play", mShot);
            }
            **/
            return new Action("Pass");
        }

        if (board.inCombat)
        {
            if (board.blocked)
            {
                //---Save Unit With Combat Trick---
                SpellCard combatTrick = null;

                int grantedPower = 0;
                int grantedHealth = 0;

                foreach (Card card in hand.cards)
                {
                    if (mana.manaGems + mana.spellMana >= card.cost)
                    {
                        if (card.name == "Radiant Strike")
                        {
                            combatTrick = (SpellCard)card;
                            grantedPower = 1;
                            grantedHealth = 1;
                            break;
                        }
                    }
                }

                if (combatTrick != null) //note: this can only use one combat trick at a time...
                {
                    foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
                    {
                        UnitCard attacker = pair.attacker;
                        UnitCard blocker = pair.blocker;

                        if (pair.blocker == null)
                        {
                            if (attacker.power + grantedPower >= opposingNexus.health && attacker.power < opposingNexus.health) //would threaten lethal
                            {
                                intendedTargets.Add(attacker);
                                return new Action("Play", combatTrick);
                            }
                            continue;
                        };

                        if (IsAttacking())
                        {
                            //would help kill or survive
                            if ((blocker.power >= attacker.health && blocker.power - attacker.health < grantedHealth) || (blocker.health > attacker.power && blocker.health - attacker.power < grantedPower))
                            {
                                intendedTargets.Add(attacker);
                                //Debug.Log("Using Radiant Strike offensively to buff a " + attacker.power + "/" + attacker.health + " against a " + blocker.power + "/" + blocker.health);
                                return new Action("Play", combatTrick);
                            }
                        }
                        else
                        {
                            //would help kill or survive
                            if ((attacker.power >= blocker.health && attacker.power - blocker.health < grantedHealth) || (attacker.health > blocker.power && attacker.health - blocker.power < grantedPower))
                            {
                                intendedTargets.Add(blocker);
                                //Debug.Log("Using Radiant Strike defensively to buff a " + blocker.power + "/" + blocker.health + " against a " + attacker.power + "/" + attacker.health);
                                return new Action("Play", combatTrick);
                            }
                        }
                    }
                }
                return new Action("Pass");
            }


            //---Determine Blocks---
            List<Battlefield.BattlePair> blocks = new List<Battlefield.BattlePair>();
            List<UnitCard> blockers = new List<UnitCard>();
            int incomingDamage = 0;
            bool facingLethal = false;

            foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
            {
                if (pair.blocker == null)
                {
                    incomingDamage += pair.attacker.power;
                }
            }

            if (incomingDamage >= nexus.health)
            {
                facingLethal = true;
            }
            else
            {
                facingLethal = false;
            }


            foreach (Battlefield.BattlePair pair in board.battlefield.battlingUnits)
            {
                if (pair.blocker != null) continue;

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
                        if (unit.power >= attacker.health && (!attacker.HasKeyword(Keyword.QuickAttack) || unit.health > attacker.power) && (!attacker.HasKeyword(Keyword.Barrier)) && !((unit.power == attacker.health) && attacker.HasKeyword(Keyword.Tough)))
                        {
                            if (bestBlocker == null || UnitValue(unit) < UnitValue(bestBlocker))
                            {
                                bestBlocker = unit;
                            }
                        }

                        //or survive
                        if (unit.health > attacker.power || (unit.health == attacker.power && unit.HasKeyword(Keyword.Tough) || unit.HasKeyword(Keyword.Barrier)))
                        {
                            if (bestBlocker == null || UnitValue(unit) * 1.5 < UnitValue(bestBlocker))
                            {
                                bestBlocker = unit;
                            }
                        }

                        /**
                        //or prevent damage (not that impactful...)
                        if (attacker.power > UnitValue(unit) * 3)
                        {
                            if (bestBlocker == null)
                            {
                                bestBlocker = unit;
                            }
                        }
                        **/

                        //or prevent lethal
                        if (facingLethal)
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
                    if (attacker.HasKeyword(Keyword.Overwhelm) && attacker.power > bestBlocker.health)
                    {
                        incomingDamage -= bestBlocker.health;
                    }
                    else
                    {
                        incomingDamage -= attacker.power;
                    }

                    if (incomingDamage >= nexus.health)
                    {
                        facingLethal = true;
                    }
                    else
                    {
                        facingLethal = false;
                    }
                }
            }

            if (blocks.Count > 0)
            {
                return new Action("Block", blocks);
            }

            return new Action("Pass");
        }

        //already starting to declare attackers...cannot play units or spells
        if (declaringAttack)
        {
            //continue challenging units
            if (opposingBench.units.Count > 0)
            {
                UnitCard challengerUnit = null;
                foreach (UnitCard unit in bench.units)
                {
                    if (unit.HasKeyword(Keyword.Challenger))
                    {
                        challengerUnit = unit;
                    }
                }

                if (challengerUnit != null)
                {
                    UnitCard bestDrag = DetermineBestTrade(challengerUnit);
                    if (bestDrag != null)
                    {
                        return new Action("Challenge", new Battlefield.BattlePair(challengerUnit, bestDrag));
                    }
                }
            }

            //commit all other units and perform attack
            declaringAttack = false;

            List<UnitCard> attackers = HandleAttackOptions();

            if (attackers != null && attackers.Count > 0)
            {
                return new Action("Attack", attackers);
            }

            return new Action("Attack", bench.units);
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
                    if (bestSpell == null || playValue((SpellCard)card) > playValue(bestSpell))
                    {
                        bestSpell = (SpellCard)card;
                    }
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

        //---Play a Spell---
        if (bestSpell != null && (bestUnit == null || playValue(bestSpell) > playValue(bestUnit)) && playValue(bestSpell) > (bestSpell.cost / 6))
        {
            Action spellAction = HandleSpellOptions(bestSpell);
            if (spellAction != null)
            {
                return spellAction;
            }
        }

        //---Play Best Unit---
        if (bestUnit != null && !bench.IsFull())
        {
            if (bestUnit.name == "Laurent Bladekeeper" && bench.units.Count > 0)
            {
                intendedTargets.Add(bench.units[0]);
            }

            if (bestUnit.name == "Brightsteel Protector" && bench.units.Count > 0)
            {
                intendedTargets.Add(bench.units[0]);
            }

            if (bestUnit.name == "Laurent Duelist" && bench.units.Count > 0)
            {
                UnitCard challengeTarget = bench.units[0];
                foreach (UnitCard unit in bench.units)
                {
                    if (!unit.HasKeyword(Keyword.Challenger))
                    {
                        challengeTarget = unit;
                    }
                }
                intendedTargets.Add(challengeTarget);
            }

            return new Action("Play", bestUnit);
        }

        //---Declare Attack With All---
        if (HasAttackToken() && bench.units.Count > 0)
        {

            //start challenging units
            if (opposingBench.units.Count > 0)
            {
                UnitCard challengerUnit = null;
                foreach (UnitCard unit in bench.units)
                {
                    if (unit.HasKeyword(Keyword.Challenger))
                    {
                        challengerUnit = unit;
                    }
                }

                if (challengerUnit != null)
                {
                    UnitCard bestDrag = DetermineBestTrade(challengerUnit);
                    if (bestDrag != null)
                    {
                        declaringAttack = true;
                        return new Action("Challenge", new Battlefield.BattlePair(challengerUnit, bestDrag));
                    }
                }
            }

            List<UnitCard> attackers = HandleAttackOptions();

            if (attackers != null && attackers.Count > 0)
            {
                return new Action("Attack", attackers);
            }

            //return new Action("Attack", bench.units);
        }

        //---Pass If Nothing Else---
        return new Action("Pass");
    }

    public float playValue(UnitCard unit)
    {
        float value = unit.power + unit.health;

        switch (unit.name)
        {
            /**
            case "Vanguard Bannerman":
                value += 2 * bench.units.Count; //grants board-wide buff
                break;
            case "Laurent Duelist":
                value += 1; //grants challenger
                break;
            case "Brightsteel Protector":
                if (HasAttackToken() || OpponentHasAttackToken())
                {
                    value += 1.5f; //barrier prevents combat damage
                }
                break;
            case "Tianna Crownguard":
                if (!HasAttackToken())
                {
                    //value += 3f; //rally trigger
                }
                break;
            **/
            default:
                break;
        }

        foreach (Keyword keyword in unit.keywords)
        {
            value += 1;
        }

        return value / 2;
        //return (unit.power + unit.health) * (OpponentHasAttackToken() && unit.HasKeyword(Keyword.CantBlock) ? 0 : 1);
    }


    public float playValue(SpellCard spell)
    {
        switch (spell.name)
        {
            case "For Demacia!":
                if ((!HasAttackToken() && !OpponentHasAttackToken()) || bench.units.Count == 0)
                {
                    return 0;
                }
                else
                {
                    //return spell.cost;
                    return bench.units.Count * 3;
                }
            case "Relentless Pursuit":
                if (HasAttackToken() || bench.units.Count == 0)
                {
                    return 0;
                }
                else
                {
                    //return spell.cost;
                    return (bench.units.Count - opposingBench.units.Count) * 2f;
                }
            case "Mobilize":
                int unitCount = 0;
                foreach (Card card in hand.cards)
                {
                    if (card is UnitCard)
                    {
                        unitCount += 1;
                    }
                }
                return unitCount;
            case "Stand Alone":
                if (!(bench.units.Count == 1))
                {
                    return 0;
                }
                return 5;
            case "Back to Back":
                if ((!HasAttackToken() && !OpponentHasAttackToken()) || bench.units.Count < 2) //no attacks, or less than two allies
                {
                    return 0;
                }
                return spell.cost;
            default:
                return spell.cost;
        }
    }
    public Action HandleSpellOptions(SpellCard bestSpell)
    {
        /**
        if (bestSpell.name == "Health Potion" && nexus.health < 20)
        {
            intendedTargets.Add(nexus);
            return new Action("Play", bestSpell);
        }

        if (bestSpell.name == "Mystic Shot")
        {
            intendedTargets.Add(opposingNexus);
            return new Action("Play", bestSpell);
        }

        //Cast elusive on highest power
        if (bestSpell.name == "Sumpworks Map")
        {
            UnitCard smTarget = null;
            int bestPower = 0;
            foreach (UnitCard unit in bench.units)
            {
                if (!unit.HasKeyword(Keyword.Elusive) && (smTarget == null || unit.power > bestPower))
                {
                    smTarget = unit;
                    bestPower = unit.power;
                }
            }
            if (smTarget != null)
            {
                intendedTargets.Add(smTarget);
                return new Action("Play", bestSpell);
            }
        }
        **/

        //Cast vest on highest health
        if (bestSpell.name == "Chain Vest")
        {
            UnitCard cvTarget = null;
            int bestHealth = 0;
            foreach (UnitCard unit in bench.units)
            {
                if (!unit.HasKeyword(Keyword.Tough) && (cvTarget == null || unit.health > bestHealth))
                {
                    cvTarget = unit;
                    bestHealth = unit.power;
                }
            }
            if (cvTarget != null)
            {
                intendedTargets.Add(cvTarget);
                return new Action("Play", bestSpell);
            }
        }

        if (bestSpell.name == "Succession" || bestSpell.name == "Unlicensed Innovation" || bestSpell.name == "Reinforcements" || bestSpell.name == "For Demacia!" || bestSpell.name == "Relentless Pursuit" || bestSpell.name == "Mobilize" || bestSpell.name == "Back to Back")
        {
            return new Action("Play", bestSpell);
        }

        if (bestSpell.name == "Stand Alone" && bench.units.Count == 1)
        {
            return new Action("Play", bestSpell);
        }

        if (bestSpell.name == "Single Combat")
        {
            //choose the ally with greatest power
            UnitCard bestAlly = null;
            UnitCard bestEnemy = null;
            float bestValue = 0;

            foreach (UnitCard allyUnit in bench.units)
            {
                if (bestAlly == null)
                {
                    bestAlly = allyUnit;
                }
                float allyValue = allyUnit.power + allyUnit.health;

                UnitCard enemyUnit = DetermineBestTrade(allyUnit);

                if (enemyUnit == null) continue;

                float tradeValue = TradeValue(allyUnit, enemyUnit);

                if (tradeValue > bestValue)
                {
                    bestAlly = allyUnit;
                    bestEnemy = enemyUnit;
                    bestValue = tradeValue;
                }
            }

            if (bestAlly != null && bestEnemy != null)
            {
                intendedTargets.Add(bestAlly);
                intendedTargets.Add(bestEnemy);
                return new Action("Play", bestSpell);
            }
        }

        return null;
    }

    private UnitCard DetermineBestTrade(UnitCard allyUnit)
    {
        UnitCard bestEnemy = null;
        float bestValue = 0;

        int allyValue = allyUnit.power + allyUnit.health;

        foreach (UnitCard enemyUnit in opposingBench.units)
        {
            int enemyValue = enemyUnit.power + enemyUnit.health;
            float tradeValue = TradeValue(allyUnit, enemyUnit);
            if (tradeValue > bestValue)
            {
                bestEnemy = enemyUnit;
                bestValue = tradeValue;
            }
        }

        return bestEnemy;
    }

    private float TradeValue(UnitCard allyUnit, UnitCard enemyUnit)
    {
        float allyValue = UnitValue(allyUnit);
        float enemyValue = UnitValue(enemyUnit);
        int dealtDamage = 0;
        int receivedDamage = 0;
        bool canKill = false;
        bool canSurvive = true;

        //calculate damage ally would take
        if (!allyUnit.HasKeyword(Keyword.Barrier))
        {
            if (allyUnit.HasKeyword(Keyword.Tough))
            {
                receivedDamage = enemyUnit.power - 1;
            }
            else
            {
                receivedDamage = enemyUnit.power;
            }
            if (receivedDamage >= allyUnit.health) canSurvive = false;
        }

        //calculate damage enemy would take
        if (!enemyUnit.HasKeyword(Keyword.Barrier))
        {
            if (enemyUnit.HasKeyword(Keyword.Tough))
            {
                dealtDamage = allyUnit.power - 1;
            }
            else
            {
                dealtDamage = allyUnit.power;
            }
            if (dealtDamage >= enemyUnit.health) canKill = true;
        }

        if (canKill)
        {
            if (canSurvive) //can also survive
            {
                return enemyValue - receivedDamage;
            }
            else
            {
                return enemyValue - allyValue; //even trade
            }
        }
        else //cannot kill
        {
            if (canSurvive)
            {
                return dealtDamage - receivedDamage; //both survive
            }
            else
            {
                return dealtDamage - allyValue; //lost trade
            }
        }
    }

    private float UnitValue(UnitCard unit)
    {
        int value = 0;
        foreach (Keyword keyword in unit.keywords)
        {
            value += 1;
        }
        return value + unit.power + unit.health;
    }

    //doesn't seem to do much tbh
    private List<UnitCard> HandleAttackOptions()
    {
        List<UnitCard> attackers = new List<UnitCard>();
        foreach (UnitCard unit in bench.units)
        {
            bool shouldAttack = true;

            foreach (UnitCard opposingUnit in opposingBench.units)
            {
                if (!(opposingUnit.HasKeyword(Keyword.CantBlock)))
                {
                    if (TradeValue(unit, opposingUnit) < -10f)
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
            return attackers;
        }

        return null;
    }
}