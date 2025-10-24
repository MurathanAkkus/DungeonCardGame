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

    private struct StatusEffectState
    {
        public int Magnitude;   // BURN: tick başına hasar; STRENGTH/TEMP: +damage
        public int Stacks;      // UI/özel kurallar için
        public int Duration;    // -1 kalıcı, 0 yok, >0 kalan tur
    }

    private readonly Dictionary<StatusEffectType, StatusEffectState> statusEffects = new();

    // --- PUBLIC API ---

    public void SetupBase(int maxHealth, Sprite sprite, int startingArmor, int startingStrength)
    {
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        spriteRenderer.sprite = sprite;

        if (startingArmor > 0)
            AddStatusEffect(StatusEffectType.ARMOR, magnitude: startingArmor, stacks: startingArmor, duration: -1);

        if (startingStrength > 0)
            AddStatusEffect(StatusEffectType.STRENGTH, magnitude: startingStrength, stacks: startingStrength, duration: -1);

        UpdateHealthText();
    }

    public void Damage(int amount, bool ignoreArmor = false)
    {
        int dmg = amount;

        if (!ignoreArmor && TryGet(StatusEffectType.ARMOR, out var armor))
        {
            int used = Mathf.Min(armor.Magnitude, dmg);
            armor.Magnitude -= used;
            armor.Stacks = Mathf.Max(0, armor.Magnitude); // stacks’ı da senkron tutuyorsan
            dmg -= used;

            statusEffects[StatusEffectType.ARMOR] = armor;
            if (armor.Magnitude <= 0) 
                RemoveEntire(StatusEffectType.ARMOR);
            else 
                UpdateUI(StatusEffectType.ARMOR);
        }

        CurrentHealth = Mathf.Max(0, CurrentHealth - dmg);
        UpdateHealthText();
        // VFX/anim vs...
    }

    // ====== CORE METHODS ======

    public void AddStatusEffect(StatusEffectType type, int magnitude, int stacks, int duration)
    {
        if (statusEffects.TryGetValue(type, out var s))
        {
            // DELTA mantığı: gelen değerleri direkt ekle/çıkar
            s.Magnitude += magnitude;
            s.Stacks += stacks;

            // ARMOR için stacks'i magnitude ile senkron tut (UI düzgün görünsün)
            if (type == StatusEffectType.ARMOR)
                s.Stacks = s.Magnitude;

            // Alt sınıra kıstır
            s.Magnitude = Mathf.Max(0, s.Magnitude);
            s.Stacks = Mathf.Max(0, s.Stacks);

            // Süre kuralı (eski ile aynı)
            if (s.Duration < 0 || duration < 0) s.Duration = -1;
            else if (s.Duration >= 0 && duration >= 0) s.Duration = Mathf.Max(s.Duration, duration);

            // Boşaldıysa kaldır
            if (s.Magnitude == 0 && s.Stacks == 0 && s.Duration != -1)
            {
                RemoveEntire(type);
                return;
            }


            statusEffects[type] = s;
        }
        else
        {
            // İlk kez ekleniyorsa – negatif gelirse 0'a kıstır
            var mag = Mathf.Max(0, magnitude);
            var stk = Mathf.Max(0, stacks);
            if (type == StatusEffectType.ARMOR) stk = mag;

            statusEffects[type] = new StatusEffectState
            {
                Magnitude = mag,
                Stacks = stk,
                Duration = duration
            };
        }

        var cur = statusEffects[type];
        SyncStacks(type, ref cur);
        statusEffects[type] = cur;

        UpdateUI(type);
    }

    public void AddStatusEffect(StatusEffectType type, int stacks)
    {
        // Eski davranış: stacks hem magnitude hem de duration gibi davranıyordu.
        // Artık daha mantıklı varsayımlar:
        int magnitude = stacks;      // ör: Strength kartı böyle kullanıyorduysa bozulmasın
        int duration = stacks;      // ör: Burn için stacks kadar tur
        AddStatusEffect(type, magnitude, stacks, duration);
    }

    /// <summary> Yalnızca duration azalt – 0 olursa tamamen sil. </summary>
    public void DecreaseDuration(StatusEffectType type, int amount)
    {
        if (!statusEffects.TryGetValue(type, out var s)) 
            return;
        if (s.Duration < 0) 
            return; // kalıcı

        s.Duration = Mathf.Max(0, s.Duration - Mathf.Max(0, amount));
        if (s.Duration == 0) 
            RemoveEntire(type);
        else
        {
            SyncStacks(type, ref s);
            statusEffects[type] = s;
            UpdateUI(type);
        }
    }

    public void RemoveStatusEffect(StatusEffectType type, int stackAmount)
    {
        if (!statusEffects.TryGetValue(type, out var s)) 
            return;

        s.Stacks = Mathf.Max(0, s.Stacks - Mathf.Max(0, stackAmount));

        // Eski mantığa yakın kalmak için: stacks sıfıra inerse tamamen kaldır
        if (s.Stacks == 0 && s.Duration == 0 && s.Magnitude == 0)
            RemoveEntire(type);
        else
        {
            statusEffects[type] = s;
            UpdateUI(type);
        }
    }

    public int GetStatusEffectStackCount(StatusEffectType type)
    {
        return statusEffects.TryGetValue(type, out var s) ? s.Stacks : 0;
    }

    public int GetStatusEffectMagnitude(StatusEffectType type)
    {
        return statusEffects.TryGetValue(type, out var s) ? s.Magnitude : 0;
    }

    public int GetStatusEffectDuration(StatusEffectType type)
    {
        return statusEffects.TryGetValue(type, out var s) ? s.Duration : 0;
    }

    public int GetAttackFlatBonus()
    {
        int sum = 0;
        if (statusEffects.TryGetValue(StatusEffectType.STRENGTH, out var s1)) sum += s1.Magnitude;
        if (statusEffects.TryGetValue(StatusEffectType.TEMP_STR, out var s2))
        {
            // yalnızca süresi devam edenler katkı sağlasın
            if (s2.Duration != 0) 
                sum += s2.Magnitude;
        }
        return sum;
    }

    // ====== HELPERS ======

    private void SyncStacks(StatusEffectType type, ref StatusEffectState s)
    {
        switch (type)
        {
            case StatusEffectType.BURN:
                // BURN’de ikon üstünde kalan turu gösterir
                if (s.Duration >= 0) 
                    s.Stacks = s.Duration; // kalan turu göster
                else 
                    s.Stacks = Mathf.Max(0, s.Stacks); // -1 kalıcı ise elleme
                break;

            case StatusEffectType.ARMOR:
            case StatusEffectType.STRENGTH:
            case StatusEffectType.TEMP_STR:
                // Bu tiplerde stacks = power
                s.Stacks = Mathf.Max(0, s.Magnitude);
                break;
        }
    }

    private void RemoveEntire(StatusEffectType type)
    {
        if (!statusEffects.Remove(type)) 
            return;
        UpdateUI(type, removed: true);
    }

    private void UpdateUI(StatusEffectType type, bool removed = false)
    {
        if (statusEffectsUI == null) 
            return;

        if (removed)
        {
            statusEffectsUI?.Remove(type);
            return;
        }

        if (statusEffects.TryGetValue(type, out var s))
        {
            statusEffectsUI.Upsert(new StatusEffectViewModel(
                type,
                s.Stacks,
                s.Magnitude,
                s.Duration
            ));
        }
    }

    private bool TryGet(StatusEffectType type, out StatusEffectState s) => statusEffects.TryGetValue(type, out s);

    private void UpdateHealthText()
    {
        if (healthText != null) 
            healthText.text = $"{CurrentHealth}/{MaxHealth}";
    }
}
