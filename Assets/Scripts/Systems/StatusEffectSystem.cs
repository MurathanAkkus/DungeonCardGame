using System.Collections;
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
        foreach (var target in ga.Targets)
        {
            // NEW: magnitude + stacks + duration
            target.AddStatusEffect(ga.StatusEffectType, ga.Magnitude, ga.StackCount, ga.Duration);

            var ui = target.GetComponent<StatusEffectsUI>();
            if (ui != null)
            {
                int totalStacks = target.GetStatusEffectStackCount(ga.StatusEffectType);
                ui.UpdateStatusEffectUI(ga.StatusEffectType, totalStacks);
                // İstersen magn+dur ile overload’ı da çağır.
            }
        }
        yield return null;
    }
}