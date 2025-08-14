using System.Collections.Generic;
using UnityEngine;

// <summary>
// DealDamageEffect.cs
// Bu class, bir hedefe hasar veren bir etkiyi temsil eder.
// </summary>
public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        DealDamageGA dealDamageGA = new DealDamageGA(damageAmount, targets, caster);
        return dealDamageGA;
    }
}
