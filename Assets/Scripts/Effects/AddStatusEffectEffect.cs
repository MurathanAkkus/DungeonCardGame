using System.Collections.Generic;
using UnityEngine;

// <summary>
// AddStatusEffectEffect.cs
// Bu s�n�f, bir stat� etkisi eklemek i�in kullan�lan bir efekt t�r�d�r.
// </summary>
public class AddStatusEffectEffect : Effect
{
    [SerializeField] private StatusEffectType statusEffectType;
    [SerializeField] private int stackCount;
    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {   // Yeni bir AddStatusEffectGA olu�tur ve gerekli parametreleri ata
        return new AddStatusEffectGA(statusEffectType, stackCount, targets);
    }
}
