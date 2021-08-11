using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpellEffects
{
    public LoRBoard board;
    public int castingPlayer;
    public LoRBoardSide castingSide;

    public SpellEffects(LoRBoard board)
    {
        this.board = board;
    }

    public void Resolve(SpellCard card, int currentCastingPlayer)
    {
        castingPlayer = currentCastingPlayer;

        if (castingPlayer == 1)
        {
            castingSide = board.playerOneSide;
        }
        else
        {
            castingSide = board.playerTwoSide;
        }

        switch (card.name)
        {
            case "Health Potion":
                if (card.targets[0] is UnitCard)
                {
                    HealthPotion((UnitCard)card.targets[0]);
                }
                else
                {
                    HealthPotion((Nexus)card.targets[0]);
                }
                break;
            case "Mystic Shot":
                if (card.targets[0] is UnitCard)
                {
                    MysticShot((UnitCard)card.targets[0]);
                }
                else
                {
                    MysticShot((Nexus)card.targets[0]);
                }
                break;
            case "Decimate":
                Decimate((Nexus)card.targets[0]);
                break;
            case "Radiant Strike":
                RadiantStrike((UnitCard)card.targets[0]);
                break;
            case "Chain Vest":
                ChainVest((UnitCard)card.targets[0]);
                break;
            case "Sumpworks Map":
                SumpworksMap((UnitCard)card.targets[0]);
                break;
            case "Succession":
                Succession();
                break;
            case "Unlicensed Innovation":
                UnlicensedInnovation();
                break;
            case "Laurent Bladekeeper Play":
                LaurentBladekeeperPlay((UnitCard)card.targets[0]);
                break;
            case "Vanguard Sergeant Summon":
                VanguardSergeantSummon();
                break;
            case "Vanguard Bannerman Summon":
                VanguardBannermanSummon();
                break;
            case "For Demacia!":
                ForDemacia();
                break;
            case "Stand Alone":
                StandAlone();
                break;
            case "Single Combat":
                SingleCombat((UnitCard)card.targets[0], (UnitCard)card.targets[1]);
                break;
            case "Reinforcements":
                Reinforcements();
                break;
            case "Relentless Pursuit":
                RelentlessPursuit();
                break;
            case "Mobilize":
                Mobilize();
                break;
            case "Tianna Crownguard Summon":
                TiannaCrownguardSummon();
                break;
            case "Laurent Duelist Play":
                LaurentDuelistPlay((UnitCard)card.targets[0]);
                break;
            case "Brightsteel Protector Play":
                BrightsteelProtectorPlay((UnitCard)card.targets[0]);
                break;
            case "Back to Back":
                BackToBack((UnitCard)card.targets[0], (UnitCard)card.targets[1]);
                break;
            case "Prismatic Barrier":
                PrismaticBarrier((UnitCard)card.targets[0]);
                break;
            case "Riposte":
                Riposte((UnitCard)card.targets[0]);
                break;
            case "Redoubled Valor":
                RedoubledValor((UnitCard)card.targets[0]);
                break;
            case "En Garde":
                EnGarde();
                break;
            default:
                Debug.Log("Spell not found: " + card.name);
                break;
        }
        castingPlayer = 0;
    }

    public void HealthPotion(UnitCard target)
    {
        target.Heal(3);
    }

    public void HealthPotion(Nexus target)
    {
        target.Heal(3);
    }

    public void MysticShot(UnitCard target)
    {
        target.TakeDamage(2);
    }

    public void MysticShot(Nexus target)
    {
        target.TakeDamage(2);
    }

    public void Decimate(Nexus target)
    {
        target.TakeDamage(4);
    }

    public void RadiantStrike(UnitCard target)
    {
        target.ReceiveRoundBuff(1, 1);
    }

    public void ChainVest(UnitCard target)
    {
        target.ReceiveKeyword(Keyword.Tough);
    }

    public void SumpworksMap(UnitCard target)
    {
        target.ReceiveKeyword(Keyword.Elusive);
    }

    public void Succession()
    {
        castingSide.bench.Add(new UnitCard("Dauntless Vanguard", Region.Demacia, 3, 3, 3, type: "Elite"));
    }

    public void UnlicensedInnovation()
    {
        castingSide.bench.Add(new UnitCard("Illegal Contraption", Region.Demacia, 6, 5, 5));
    }

    public void LaurentBladekeeperPlay(UnitCard target)
    {
        target.ReceiveBuff(2, 2);
    }

    public void VanguardSergeantSummon()
    {
        castingSide.hand.Add(new SpellCard("For Demacia!", Region.Demacia, 6, SpellType.Slow, null));
    }

    public void VanguardBannermanSummon()
    {
        Deck castingDeck = castingSide.deck;

        if (castingDeck.cards.Count > 0)
        {
            if (castingDeck.cards[0].region != Region.Demacia)
            {
                return;
            }
        }

        Bench castingBench = castingSide.bench;

        for (int i = 0; i < castingBench.units.Count - 1; i++) //every unit except last one
        {
            castingBench.units[i].ReceiveBuff(1, 1);
        }
    }

    public void ForDemacia()
    {
        foreach (UnitCard unit in castingSide.bench.units)
        {
            unit.ReceiveRoundBuff(3, 3);
        }
    }

    public void StandAlone()
    {
        //NOTE: needs to account for if unit is battling
        castingSide.bench.units[0].ReceiveBuff(3, 3);
    }

    public void SingleCombat(UnitCard unit1, UnitCard unit2)
    {
        unit1.Strike(unit2);
        unit2.Strike(unit1);
    }

    public void Reinforcements()
    {
        Bench castingBench = castingSide.bench;
        castingBench.Add(new UnitCard("Dauntless Vanguard", Region.Demacia, 3, 3, 3, type: "Elite"));
        castingBench.Add(new UnitCard("Dauntless Vanguard", Region.Demacia, 3, 3, 3, type: "Elite"));
        foreach (UnitCard unit in castingBench.units)
        {
            if (unit.type == "Elite")
            {
                unit.ReceiveBuff(1, 1);
            }
        }
    }

    public void RelentlessPursuit()
    {
        Rally();
    }

    public void TiannaCrownguardSummon()
    {
        Rally();
    }

    public void Mobilize()
    {
        foreach (Card card in castingSide.hand.cards)
        {
            if (card is UnitCard)
            {
                card.ReduceCost(1);
            }
        }
    }

    public void Rally()
    {
        castingSide.GainAttackToken();
    }

    public void LaurentDuelistPlay(UnitCard unit)
    {
        unit.ReceiveRoundKeyword(Keyword.Challenger);
    }

    public void BrightsteelProtectorPlay(UnitCard unit)
    {
        unit.ReceiveRoundKeyword(Keyword.Barrier);
    }

    public void BackToBack(UnitCard unit1, UnitCard unit2)
    {
        unit1.ReceiveRoundBuff(3, 3);
        unit2.ReceiveRoundBuff(3, 3);
    }

    public void PrismaticBarrier(UnitCard unit)
    {
        unit.ReceiveKeyword(Keyword.Barrier);
    }

    public void RedoubledValor(UnitCard unit)
    {
        unit.health = unit.grantedHealth;
        unit.ReceiveBuff(unit.power, unit.health);
    }

    public void EnGarde()
    {
        foreach (UnitCard unit in castingSide.bench.units)
        {
            unit.ReceiveRoundKeyword(Keyword.Challenger);
        }
    }

    public void Riposte(UnitCard unit)
    {
        unit.ReceiveRoundBuff(3, 0);
        unit.ReceiveRoundKeyword(Keyword.Barrier);
    }
}