using System.Collections.Generic;

public class AddStatusEffectGA : GameAction
{
    public StatusEffectType StatusEffectType { get; private set; }
    public int StackCount { get; private set; }

    public int Magnitude { get; private set; }
    public int Duration { get; private set; } // -1 => permanent

    public List<CombatantView> Targets { get; private set; }

    public AddStatusEffectGA(StatusEffectType statusEffectType, int magnitude, int stackCount, int duration, List<CombatantView> targets)
    {
        StatusEffectType = statusEffectType;
        Magnitude = magnitude;
        StackCount = stackCount;
        Duration = duration;
        Targets = targets;
    }

    // BACKWARD-COMPAT (eski çağrılar bozulmasın diye):
    public AddStatusEffectGA(StatusEffectType statusEffectType, int stackCount, List<CombatantView> targets)
        : this(statusEffectType, magnitude: stackCount, stackCount: stackCount, duration: stackCount, targets: targets)
    { }
}