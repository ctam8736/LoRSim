using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public virtual bool ActivateOnSummon(UnitCard unit)
    {
        return true;
    }

    public virtual bool ActivateOnDeath(UnitCard unit)
    {
        return true;
    }


}