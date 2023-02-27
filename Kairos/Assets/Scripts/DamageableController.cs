using System.Collections.Generic;
using UnityEngine;

public class DamageableController : MonoBehaviour
{

    public List<Damageable> Damageables
    {
        get { return damageables; }
    }
    List<Damageable> damageables = new List<Damageable>();

    public void AddDamageable(Damageable damageable)
    {
        damageables.Add(damageable);
    }
}
