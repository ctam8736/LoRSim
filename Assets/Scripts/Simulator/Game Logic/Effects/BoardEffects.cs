using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardEffects
{
    public List<Effect> onSummonEffects;
    public List<Effect> onSpellCastEffects;
    public List<Effect> onDeathEffects;
    public List<Effect> roundStartEffects;
    public List<Effect> roundEndEffects;

    public List<int> indicesToRemove = new List<int>();

    /// <summary>
    /// Activates all effects that trigger on the summoning of the specified unit.
    /// </summary>
    public void TriggerSummonEffects(UnitCard unit)
    {
        for (int i = onSummonEffects.Count - 1; i > -1; i--)
        {
            if (!onSummonEffects[i].ActivateOnSummon(unit))
            {
                indicesToRemove.Add(i);
            }
        }

        foreach (int index in indicesToRemove)
        {
            onSummonEffects.RemoveAt(index);
        }

        indicesToRemove.Clear();
    }

    /// <summary>
    /// To implement: Activates all effects that trigger on the death of the specified unit.
    /// </summary>
    public void TriggerDeathEffects(UnitCard unit)
    {
        for (int i = onDeathEffects.Count - 1; i > -1; i--)
        {
            if (!onDeathEffects[i].ActivateOnDeath(unit))
            {
                indicesToRemove.Add(i);
            }
        }

        foreach (int index in indicesToRemove)
        {
            onDeathEffects.RemoveAt(index);
        }

        indicesToRemove.Clear();
    }
}