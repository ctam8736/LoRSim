using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetfeatherTrackerEffect : Effect
{
    public UnitCard tracker;
    public override bool ActivateOnSummon(UnitCard unit)
    {
        if (tracker.IsDead()) return false;
        tracker.ReceiveKeyword(Keyword.Challenger);
        return true;
    }

}

public class BattlesmithEffect : Effect
{
    public UnitCard smith;
    public override bool ActivateOnSummon(UnitCard unit)
    {
        if (smith.IsDead()) return false;
        if (unit.HasType(UnitType.Elite))
        {
            unit.ReceiveBuff(1, 1);
        }
        return true;
    }
}