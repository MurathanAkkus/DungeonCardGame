using System.Collections.Generic;
using UnityEngine;

public class Perk
{
    public Sprite Image => data.Image;
    private readonly PerkData data;
    private readonly PerkCondition condition; // Perk'in tetiklenme koþulu
    private readonly AutoTargetEffect effect;
    public Perk(PerkData perkData)
    {
        data = perkData;            
        condition = data.PerkCondition;
        effect = data.AutoTargetEffect;
    }

    public void OnAdd()
    {
        condition.SubscribeCondition(Reaction);
    }
    public void OnRemove()
    {
        condition.UnsubscribeCondition(Reaction);
    }

    private void Reaction(GameAction gameAction)
    {
        if (condition.SubConditionMet(gameAction))
        {
            List<CombatantView> targets = new List<CombatantView>();                  // Etki için hedefleri tutacak liste

            if (data.UseActionCasterAsTarget && gameAction is IHaveCaster haveCaster) // Eylemde bir caster varsa ve perk bu caster'ý hedef olarak kullanacaksa
            {
                targets.Add(haveCaster.Caster);                                       // Eylemdeki caster'ý hedef olarak ekle
            }
            if (data.UseAutoTarget)                                                   // Eðer otomatik hedefleme kullanýlýyorsa
            {
                targets.AddRange(effect.TargetMode.GetTargets());                     // Otomatik hedefleme modundan hedefleri ekle
            }
            GameAction perkEffectAction = effect.Effect.GetGameAction(targets, HeroSystem.Instance.HeroView); // Etki eylemini oluþtur, hedefleri ve caster'ý kullanarak
            ActionSystem.Instance.AddReaction(perkEffectAction);                                              // Etki eylemini ActionSystem'e ekle
        }
    }
}