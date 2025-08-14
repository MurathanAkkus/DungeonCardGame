using System.Collections.Generic;
using UnityEngine;

// <summary>
// AddStatusEffectEffect.cs
// Bu sýnýf, bir statü etkisi eklemek için kullanýlan bir efekt türüdür.
// </summary>
public class AddStatusEffectEffect : Effect
{
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {   // Yeni bir AddStatusEffectGA oluþtur ve gerekli parametreleri ata
        return new AddStatusEffectGA(statusEffectType, stackCount, targets);
    }
}
