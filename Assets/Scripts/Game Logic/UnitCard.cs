using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnitCard : Card
{
    public int power;
    public int health;
    public int initialPower;
    public int initialHealth;
    public string type;

    //public List<Buff> buffs;
    public List<Keyword> keywords;
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
        this.power = power;
        this.initialHealth = health;
        this.health = health;

        if (keywords == null)
        {
            this.keywords = new List<Keyword>();
            this.initialKeywords = new List<Keyword>(this.keywords);
        }
        else
        {
            this.keywords = keywords;
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

    //returns overkill
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
        UnitCard newCard = new UnitCard(card.name, card.cost, card.power, card.health, card.keywords);
        return newCard;
    }

    public string ToString()
    {
        return name + " (" + power + "/" + health + ")";
    }
}