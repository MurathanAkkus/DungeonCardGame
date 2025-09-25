using System.Collections.Generic;

public class DealDamageGA : GameAction, IHaveCaster
{
    public int BaseAmount { get; private set; }
    public int Amount { get; private set; }
    public List<CombatantView> Targets { get; private set; }
    public CombatantView Caster { get; private set; }

    public bool IgnoreArmor { get; private set; }

    public DealDamageGA(int amount, List<CombatantView> targets, CombatantView caster, bool ignoreArmor = false)
    {
        BaseAmount = amount;
        Targets = targets == null ? new List<CombatantView>() : new List<CombatantView>(targets);
        Caster = caster;
        IgnoreArmor = ignoreArmor;
        Amount = CalculateModifiedAmount();
    }

    private int CalculateModifiedAmount()
    {
        int modified = BaseAmount;

        if (Caster != null)
        {
            // OLD: 
            //modifiedAmount += Caster.GetStatusEffectStackCount(StatusEffectType.STRENGTH);

            // NEW: CombatantView üstünden birleşik flat bonus
            modified += Caster.GetAttackFlatBonus();
        }

        return modified < 0 ? 0 : modified;
    }
}