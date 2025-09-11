using System.Collections.Generic;
using UnityEngine;

// <summary>
// DealDamageEffect.cs
// Bu class, bir hedefe hasar veren bir etkiyi temsil eder.
// </summary>
public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    [SerializeField] private bool ignoreArmor = false;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        DealDamageGA dealDamageGA = new DealDamageGA(damageAmount, targets, caster, ignoreArmor: ignoreArmor);
        return dealDamageGA;
    }
}