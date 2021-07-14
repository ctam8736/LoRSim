using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{
    public SpellType spellType;

    public List<TargetType> targetTypes;

    public TargetType nextTargetType;
    public List<object> targets;

    public SpellCard(string name, int cost, SpellType spellType, List<TargetType> targetTypes)
    {
        this.name = name;
        this.spellType = spellType;
        this.targetTypes = targetTypes;
        if (targetTypes != null && targetTypes.Count > 0)
        {
            nextTargetType = targetTypes[0];
            targets = new List<object>();
        }
    }

    /// <summary>
    /// Assigns the next target (unit card) of the spell.
    /// </summary>
    public void AssignNextTarget(UnitCard target)
    {
        targets.Add(target);
    }

    /// <summary>
    /// Assigns the next target (nexus) of the spell.
    /// </summary>
    public void AssignNextTarget(Nexus target)
    {
        targets.Add(target);
    }

    /// <summary>
    /// Returns true if the spell still needs specified targets to resolve.
    /// </summary>
    public bool NeedsTargets()
    {
        if (targetTypes == null || targetTypes.Count == 0) return false;

        return targetTypes.Count == targets.Count;
    }

    public static SpellCard CopyCard(SpellCard card)
    {
        SpellCard newCard = new SpellCard(card.name, card.cost, card.spellType, card.targetTypes);
        return newCard;
    }
}

public enum SpellType
{
    Burst,
    Fast,
    Slow,
    Focus
}

public enum TargetType
{
    AlliedUnitOrNexus,
    AlliedUnit,
    EnemyUnitOrNexus,
    EnemyUnit,
    Anything
}
