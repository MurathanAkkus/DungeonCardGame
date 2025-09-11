using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CombatantView : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private StatusEffectsUI statusEffectsUI;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    private Dictionary<StatusEffectType, int> statusEffects = new Dictionary<StatusEffectType, int>();

    protected void SetupBase(int health, Sprite image, int startingArmor)
    {
        MaxHealth = CurrentHealth = health;

        // Sprite null ise patlama
        if (spriteRenderer != null && image != null)
            spriteRenderer.sprite = image;

        UpdateHealthText();

        // Başlangıç zırhını normalize et
        startingArmor = Mathf.Max(0, startingArmor);

        // Idempotent olsun istiyorum, mevcut ARMOR’u sıfırla
        int existing = GetStatusEffectStackCount(StatusEffectType.ARMOR);
        if (existing > 0)
            RemoveStatusEffect(StatusEffectType.ARMOR, existing);

        // Başlangıç zırhı
        if (startingArmor > 0)
        {
            AddStatusEffect(StatusEffectType.ARMOR, startingArmor);
        }
    }

    private void UpdateHealthText()
    {
        healthText.text = $"HP: {CurrentHealth}";
    }

    public void Damage(int damageAmount, bool ignoreArmor = false)
    {
        // Negatif/0 koruması (isteğe bağlı)
        if (damageAmount <= 0)
        {
            // yine de küçük bir hit efekti istiyorsan buraya koyabilirsin
            return;
        }

        int currentArmor = GetStatusEffectStackCount(StatusEffectType.ARMOR);

        // Zırhı YOK SAY: direkt cana uygula, zırh stack'ini düşürme
        if (ignoreArmor)
        {
            CurrentHealth = Mathf.Max(CurrentHealth - damageAmount, 0);
        }
        else
        {
            // Zırh hasarı emer ve stack düşer
            int remainingDamage = Mathf.Max(damageAmount - currentArmor, 0);
            if (currentArmor > 0)
                RemoveStatusEffect(StatusEffectType.ARMOR, Mathf.Min(damageAmount, currentArmor));

            CurrentHealth = Mathf.Max(CurrentHealth - remainingDamage, 0);
        }

        // Ortak VFX/UI
        transform.DOShakePosition(0.2f, 0.5f);
        UpdateHealthText();

        // (Opsiyonel) event/anim hook'ları
        // OnDamaged?.Invoke(this, damageAmount, ignoreArmor);
        // if (CurrentHealth == 0) Die();
    }

    public void PenetratingDamage(int damageAmount)
    {
        Damage(damageAmount, ignoreArmor: true);
    }

    public void AddStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.TryGetValue(type, out int currentCount))
        {
            statusEffects[type] = currentCount + stackCount;
        }
        else
        {
            statusEffects[type] = stackCount;
        }
        statusEffectsUI.UpdateStatusEffectUI(type, statusEffects[type]);
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackCount)
    {
        if (statusEffects.TryGetValue(type, out int currentCount))
        {
            currentCount -= stackCount;
            if (currentCount <= 0)
            {
                statusEffects.Remove(type);
                statusEffectsUI.UpdateStatusEffectUI(type, 0);
            }
            else
            {
                statusEffects[type] = currentCount;
                statusEffectsUI.UpdateStatusEffectUI(type, GetStatusEffectStackCount(type));
            }
        }
    }

    public int GetStatusEffectStackCount(StatusEffectType type)
    {
        return statusEffects.TryGetValue(type, out int count) ? statusEffects[type] : 0;
    }
}