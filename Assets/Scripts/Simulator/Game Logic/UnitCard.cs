﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// A UnitCard represents a unit that can be placed on the bench or battlefield.
/// </summary>
public class UnitCard : Card
{
    //Reflects current power and health - includes round buffs and damage.
    public int power;
    public int health;

    //Reflects power and health permanently given to this unit.
    public int grantedPower;
    public int grantedHealth;

    //Reflects unit's initial power and health before effects.
    public int initialPower;
    public int initialHealth;

    public SpellCard onPlay;
    public SpellCard onSummon;
    public SpellCard onAttack;
    public SpellCard onStrike;

    //public List<Buff> buffs;
    public List<UnitType> types;
    public List<Keyword> keywords;
    public List<Keyword> grantedKeywords;
    public List<Keyword> initialKeywords;

    /**
    public UnitCard(string name, int cost, int power, int health)
    {
        this.name = name;
        this.cost = cost;
        this.initialPower = power;
        this.power = power;
        this.initialHealth = health;
        this.health = health;
    }
    **/

    public UnitCard(string name, Region region, int cost, int power, int health, List<Keyword> keywords = null, List<UnitType> types = null, SpellCard onPlay = null, SpellCard onSummon = null, SpellCard onAttack = null, SpellCard onStrike = null)
    {
        this.name = name;
        this.initialCost = cost;
        this.cost = initialCost;
        this.cardType = CardType.UnitCard;

        this.initialPower = power;
        this.grantedPower = power;
        this.power = power;
        this.initialHealth = health;
        this.grantedHealth = health;
        this.health = health;

        if (keywords == null)
        {
            this.initialKeywords = new List<Keyword>();
            this.keywords = new List<Keyword>(this.initialKeywords);
            this.grantedKeywords = new List<Keyword>(this.initialKeywords);
        }
        else
        {
            this.initialKeywords = keywords;
            this.keywords = new List<Keyword>(this.initialKeywords);
            this.grantedKeywords = new List<Keyword>(this.initialKeywords);
        }

        if (types == null)
        {
            this.types = new List<UnitType>();
        }
        else
        {
            this.types = types;
        }
        this.region = region;
        this.onPlay = onPlay;
        this.onSummon = onSummon;
    }

    public bool HasKeyword(Keyword keyword)
    {
        return keywords.Contains(keyword);
    }

    public bool HasType(UnitType type)
    {
        return types.Contains(type);
    }

    public void Strike(UnitCard unit)
    {
        unit.TakeDamage(power);
    }

    public void ReceiveRoundBuff(int buffPower, int buffHealth)
    {
        power += buffPower;
        health += buffHealth;
    }

    public void RevertRoundBuff()
    {
        if (HasKeyword(Keyword.Barrier))
        {
            keywords.RemoveAt(keywords.FindIndex(0, keywords.Count, x => x == Keyword.Barrier));
        }

        power = grantedPower;
        health = Math.Min(health, grantedHealth);
        keywords = new List<Keyword>(grantedKeywords);
    }

    public void RevertRoundSilence()
    {
        //?????
    }

    public void ReceiveBuff(int buffPower, int buffHealth)
    {
        grantedPower += buffPower;
        grantedHealth += buffHealth;
        power += buffPower;
        health += buffHealth;
    }

    public void ReceiveRoundKeyword(Keyword buffKeyword)
    {
        if (!HasKeyword(buffKeyword))
        {
            keywords.Add(buffKeyword);
        }
    }

    public void ReceiveKeyword(Keyword buffKeyword)
    {
        if (!HasKeyword(buffKeyword))
        {
            keywords.Add(buffKeyword);
            grantedKeywords.Add(buffKeyword);
        }
    }

    public void Silence()
    {
        grantedPower = initialPower;
        grantedHealth = initialHealth;
        power = grantedPower;
        health = grantedHealth;
    }

    public void RoundSilence()
    {
        power = initialPower;
        health = initialHealth;
    }

    public void TriggerSummonEffect()
    {

    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        if (HasKeyword(Keyword.Barrier))
        {
            //prevent damage and remove barrier
            keywords.RemoveAt(keywords.FindIndex(0, keywords.Count, x => x == Keyword.Barrier));
        }
        else if (HasKeyword(Keyword.Tough))
        {
            health -= damage - 1;
        }
        else
        {
            health -= damage;
        }
    }

    public void Heal(int amount)
    {
        //todo: unit might be buffed...
        if (amount <= 0) return;
        health = Math.Min(health + amount, initialHealth);
    }

    public override void Revert()
    {
        power = initialPower;
        health = initialHealth;
        grantedPower = initialPower;
        grantedHealth = initialHealth;
        keywords = new List<Keyword>(this.initialKeywords);
        grantedKeywords = new List<Keyword>(this.initialKeywords);
        cost = initialCost;
    }

    public static UnitCard CopyCard(UnitCard card)
    {
        if (card == null) return null;

        UnitCard newCard = new UnitCard(card.name, card.region, card.cost, card.initialPower, card.initialHealth, new List<Keyword>(card.initialKeywords), card.types, SpellCard.CopyCard(card.onPlay), SpellCard.CopyCard(card.onSummon));
        newCard.power = card.power;
        newCard.health = card.health;
        newCard.keywords = new List<Keyword>(card.keywords);
        newCard.grantedKeywords = new List<Keyword>(card.grantedKeywords);
        newCard.grantedPower = card.grantedPower;
        newCard.grantedHealth = card.grantedHealth;

        return newCard;
    }

    public bool IsDamaged()
    {
        return health < grantedHealth;
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public static UnitCard DummyCard()
    {
        return new UnitCard("Dummy", Region.Null, 0, 0, 0);
    }

    public override string ToString()
    {
        return name + " (" + power + "/" + health + ")";
    }
}

public enum Keyword
{
    Attune, //
    Augment,
    Barrier,
    CantAttack,
    CantBlock,
    Challenger,
    Deep, //
    DoubleAttack,
    Elusive,
    Ephemeral, //
    Fearsome,
    Fleeting,
    Frostbite,
    Fury, //
    Lifesteal, //
    Overwhelm,
    QuickAttack,
    Regeneration,
    Scout,
    Silenced,
    Spellshield, //
    Stunned,
    Tough,
    Vulnerable //
}

public enum UnitType
{
    Elite,
    Elnuk,
    Null,
    Poro,
    SeaMonster,
    Spider,
    Yordle
}