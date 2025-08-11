using System.Collections.Generic;
using UnityEngine;

public class DealDamageEffect : Effect
{
    [SerializeField] private int damageAmount;
    public override GameAction GetGameAction()
    {
        List<CombatantView> targets = new List<CombatantView>(EnemySystem.Instance.Enemies);
        DealDamageGA dealDamageGA = new DealDamageGA(damageAmount, targets);
        return dealDamageGA;
    }
}
