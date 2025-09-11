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

    private IEnumerator AddStatusEffectPerformer(AddStatusEffectGA addStatusEffectGA)
    {
        foreach (CombatantView target in addStatusEffectGA.Targets) // Her hedefe durum efekti ekle
        {
            target.AddStatusEffect(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);   // Durum efektini hedefe ekle
            StatusEffectsUI statusEffectsUI = target.GetComponent<StatusEffectsUI>();   // Durum efektleri UI'sini al
            if (statusEffectsUI != null)    // Eðer hedefte durum efektleri UI'si varsa
            {
                statusEffectsUI.UpdateStatusEffectUI(addStatusEffectGA.StatusEffectType, addStatusEffectGA.StackCount);
            }
        }
        yield return null;
    }
}