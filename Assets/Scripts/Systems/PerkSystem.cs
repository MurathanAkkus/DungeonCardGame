using System.Collections.Generic;
using UnityEngine;

public class PerkSystem : Singleton<PerkSystem>
{
    private readonly List<Perk> perks = new List<Perk>();
    public void AddPerk(Perk perk)
    {
        if (perk == null) 
            return;
        perks.Add(perk);
        perk.OnAdd();
    }

    public void RemovePerk(Perk perk)
    {
        if (perk == null) 
            return;
        perks.Remove(perk);
        perk.OnRemove();
    }
}
