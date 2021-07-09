using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card
{
    public int power;
    public int health;
    public int initialPower;
    public int initialHealth;
    public string type;
    public Effect effect;

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

    public UnitCard(string name, int cost, int power, int health, List<Keyword> keywords = null, Effect effect = null, string type = null)
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

        this.effect = effect;
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

    public override void Revert()
    {
        power = initialPower;
        health = initialHealth;
    }

    public string ToString()
    {
        return name + " (" + power + "/" + health + ")";
    }

    public static UnitCard CopyCard(UnitCard card)
    {
        UnitCard newCard = new UnitCard(card.name, card.cost, card.power, card.health, card.keywords);
        return newCard;
    }
}