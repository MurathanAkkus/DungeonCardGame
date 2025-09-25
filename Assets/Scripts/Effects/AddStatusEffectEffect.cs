using System.Collections.Generic;
using UnityEngine;

public class AddStatusEffectEffect : Effect
{
    public enum DurationMode { FromStacks, FixedTurns, Permanent }

    [SerializeField] private StatusEffectType statusEffectType = StatusEffectType.BURN;

    [Header("Effect Power")]
    [Tooltip("BURN için tur başı hasar; STRENGTH/TEMP için +damage")]
    [SerializeField] private int magnitude = 1;

    [Header("Stacks (UI ve özel kurallar için)")]
    [SerializeField] private int stackCount = 0;

    [Header("Duration")]
    [SerializeField] private DurationMode durationMode = DurationMode.FromStacks;
    [SerializeField] private int fixedDurationTurns = 1; // DurationMode.FixedTurns için

    public override GameAction GetGameAction(List<CombatantView> targets, CombatantView caster)
    {
        int duration = -1;
        switch (durationMode)
        {
            case DurationMode.FromStacks: duration = stackCount; break;
            case DurationMode.FixedTurns: duration = Mathf.Max(0, fixedDurationTurns); break;
            case DurationMode.Permanent: duration = -1; break;
        }

        return new AddStatusEffectGA(statusEffectType, magnitude, stackCount, duration, targets);
    }
}