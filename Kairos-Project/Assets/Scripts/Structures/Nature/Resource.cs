using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : Entity
{
    public int totalResources = 1000;
    public int resourcesRemaining = 1000;

    public override float MaxHealth
    {
        get => totalResources;
        protected set
        {
            totalResources = (int)value;
        }
    }

    public override float Health
    {
        get => resourcesRemaining;
        protected set
        {
            resourcesRemaining = (int)value;
        }
    }


}

