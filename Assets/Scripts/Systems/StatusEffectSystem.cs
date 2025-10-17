using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSystem : MonoBehaviour
{
    private void OnEnable()
    {
        ActionSystem.AttachPerformer<AddStatusEffectGA>(AddStatusEffectPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<AddStatusEffectGA>();
    }

    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA ga)
    {
        // 1) Hedef listesi boşsa kahramanı varsay
        var targets = ga.Targets;
        if (targets == null || targets.Count == 0)
        {
            var hero = HeroSystem.Instance != null ? HeroSystem.Instance.HeroView : null;
            if (hero != null)
                targets = new List<CombatantView> { hero };
            else
                yield break; // hiç hedef yoksa sessizce çık
        }

        // 2) Uygula + UI güncelle
        foreach (var target in targets)
        {
            if (target == null) continue;

            target.AddStatusEffect(ga.StatusEffectType, ga.Magnitude, ga.StackCount, ga.Duration);

            var ui = target.GetComponentInChildren<StatusEffectsUI>(true);
            if (ui != null && Application.isPlaying && ui.HasRegistry())
            {
                ui.Upsert(new StatusEffectViewModel(
                    ga.StatusEffectType,
                    target.GetStatusEffectStackCount(ga.StatusEffectType),
                    target.GetStatusEffectMagnitude(ga.StatusEffectType),
                    target.GetStatusEffectDuration(ga.StatusEffectType)
                ));
            }
        }
        yield break;
    }
}