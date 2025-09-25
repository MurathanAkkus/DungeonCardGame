using System.Collections;
using UnityEngine;

public class BurnSystem : MonoBehaviour
{
    [SerializeField] private GameObject burnVFX;

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<ApplyBurnGA>(ApplyBurnPerformer);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<ApplyBurnGA>();
    }

    private IEnumerator ApplyBurnPerformer(ApplyBurnGA ga)
    {
        CombatantView target = ga.Target;
        if (burnVFX != null) Instantiate(burnVFX, target.transform.position, Quaternion.identity);

        // NEW: hasarı CombatantView'dan magnitude olarak çek
        int burnDamage = target.GetStatusEffectMagnitude(StatusEffectType.BURN);
        if (burnDamage > 0)
            target.Damage(burnDamage, ignoreArmor: true);

        // NEW: stacks yerine duration düşür
        target.DecreaseDuration(StatusEffectType.BURN, 1);

        yield return new WaitForSeconds(1f);
    }
}
