using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SpellEffects
{
    public LoRBoard board;
    public int castingPlayer;

    public SpellEffects(LoRBoard board)
    {
        this.board = board;
    }

    public void Resolve(SpellCard card, int currentCastingPlayer)
    {
        castingPlayer = currentCastingPlayer;
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
            default:
                Debug.Log("Spell not found.");
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
        if (castingPlayer == 1)
        {
            board.playerOneSide.bench.Add(new UnitCard("Dauntless Vanguard", 3, 3, 3));
        }
        else
        {
            board.playerTwoSide.bench.Add(new UnitCard("Dauntless Vanguard", 3, 3, 3));
        }
    }

    public void UnlicensedInnovation()
    {
        if (castingPlayer == 1)
        {
            board.playerOneSide.bench.Add(new UnitCard("Illegal Contraption", 6, 5, 5));
        }
        else
        {
            board.playerTwoSide.bench.Add(new UnitCard("Illegal Contraption", 6, 5, 5));
        }
    }

    public void LaurentBladekeeperPlay(UnitCard target)
    {
        target.ReceiveBuff(2, 2);
    }
}