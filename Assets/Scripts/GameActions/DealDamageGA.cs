using System.Collections.Generic;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int Amount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }

    public bool IgnoreArmor { get; private set; }

    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster, bool ignoreArmor = false)
    {
        Amount = amount;
        Targets = new List<CombatantView>(targets);
        Caster = caster;
        IgnoreArmor = ignoreArmor;
    }
}