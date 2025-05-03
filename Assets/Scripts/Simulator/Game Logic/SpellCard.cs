using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A SpellCard represents a spell/effect that can resolve immediately or be placed on the stack.
/// </summary>
public class SpellCard : Card
{
    public SpellType spellType;

    public List<TargetType> targetTypes;

    public TargetType nextTargetType;
    public List<object> targets;

    public SpellCard(string name, Region region, int cost, SpellType spellType, List<TargetType> targetTypes)
    {
        this.name = name;
        this.region = region;

        this.initialCost = cost;
        this.cost = initialCost;

        this.cardType = CardType.SpellCard;

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
        if (NeedsTargets())
        {
            nextTargetType = targetTypes[targets.Count];
        }
        else
        {
            nextTargetType = TargetType.Null;
        }
    }

    /// <summary>
    /// Assigns the next target (nexus) of the spell.
    /// </summary>
    public void AssignNextTarget(Nexus target)
    {
        targets.Add(target);
        if (NeedsTargets())
        {
            nextTargetType = targetTypes[targets.Count];
        }
        else
        {
            nextTargetType = TargetType.Null;
        }
    }

    /// <summary>
    /// Returns true if the spell still needs specified targets to resolve.
    /// </summary>
    public bool NeedsTargets()
    {
        if (targetTypes == null || targetTypes.Count == 0) return false;

        return targetTypes.Count != targets.Count;
    }

    /// <summary>
    /// Returns an exact copy of the given spell card.
    /// </summary>
    public static SpellCard CopyCard(SpellCard card)
    {
        if (card == null) return null;
        SpellCard newCard = new SpellCard(card.name, card.region, card.cost, card.spellType, card.targetTypes);
        return newCard;
    }

    /// <summary>
    /// Resets this card's targets.
    /// </summary>
    public override void Revert()
    {
        if (targetTypes != null && targetTypes.Count > 0)
        {
            nextTargetType = targetTypes[0];
            targets = new List<object>();
        }
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
    AlliedNexus,
    EnemyUnitOrNexus,
    EnemyUnit,
    EnemyNexus,
    AnyUnit,
    Anything,
    Null
}
