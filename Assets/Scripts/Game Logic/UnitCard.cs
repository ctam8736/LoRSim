﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitCard : Card
{
    public int power;
    public int health;
    public int grantedPower;
    public int grantedHealth;
    public int initialPower;
    public int initialHealth;
    public string type;

    //public List<Buff> buffs;
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

    public UnitCard(string name, int cost, int power, int health, List<Keyword> keywords = null, string type = null)
    {
        this.name = name;
        this.cost = cost;
        this.initialPower = power;
        this.grantedPower = power;
        this.power = power;
        this.initialHealth = health;
        this.grantedHealth = health;
        this.health = health;

        if (keywords == null)
        {
            this.keywords = new List<Keyword>();
            this.grantedKeywords = new List<Keyword>(this.keywords);
            this.initialKeywords = new List<Keyword>(this.keywords);
        }
        else
        {
            this.keywords = keywords;
            this.grantedKeywords = new List<Keyword>(this.keywords);
            this.initialKeywords = new List<Keyword>(this.keywords);
        }

        this.type = type;
    }

    public bool HasKeyword(Keyword keyword)
    {
        return keywords.Contains(keyword);
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

    public void RevertRoundBuff(int power, int health)
    {
        power = grantedPower;
        health = Math.Min(health, grantedHealth);
        keywords = new List<Keyword>(grantedKeywords);
    }

    public void ReceiveBuff(int buffPower, int buffHealth)
    {
        grantedPower += buffPower;
        grantedHealth += buffHealth;
        power = grantedPower;
        health = grantedHealth;
    }

    public void ReceiveRoundKeyword(Keyword buffKeyword)
    {
        keywords.Add(buffKeyword);
    }

    public void ReceiveKeyword(Keyword buffKeyword)
    {
        keywords.Add(buffKeyword);
        grantedKeywords.Add(buffKeyword);
    }

    public void Silence()
    {
        grantedPower = initialPower;
        grantedHealth = initialHealth;
        power = grantedPower;
        health = grantedHealth;
    }

    public void TriggerPlayEffect()
    {

    }

    public void TriggerSummonEffect()
    {

    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0) return;

        if (HasKeyword(Keyword.Tough))
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
    }

    public static UnitCard CopyCard(UnitCard card)
    {
        if (card == null) return null;

        UnitCard newCard = new UnitCard(card.name, card.cost, card.initialPower, card.initialHealth, card.keywords);
        newCard.power = card.power;
        newCard.health = card.health;
        newCard.grantedKeywords = card.grantedKeywords;
        newCard.grantedPower = card.grantedPower;
        newCard.grantedHealth = card.grantedHealth;

        return newCard;
    }

    public bool IsDamaged()
    {
        return health < grantedHealth;
    }

    public override string ToString()
    {
        return name + " (" + power + "/" + health + ")";
    }
}