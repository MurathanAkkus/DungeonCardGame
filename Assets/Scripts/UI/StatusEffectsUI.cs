using System.Collections.Generic;
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField] private Sprite armorSprite, burnSprite;

    private Dictionary<StatusEffectType, StatusEffectUI> statusEffectUIs = new();

    public void UpdateStatusEffectUI(StatusEffectType statusEffectType, int stackCount)
    {
        if (stackCount == 0)
        {   // Eðer yýðýn sayýsý 0 ise, bu durum efekti için UI'yi kaldýr
            if (statusEffectUIs.ContainsKey(statusEffectType))
            {   // Eðer bu durum efekti için bir UI varsa
                StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType]; // Bu durum efekti için mevcut UI'yý edir
                statusEffectUIs.Remove(statusEffectType);                          // Dictionary'den kaldýr
                Destroy(statusEffectUI.gameObject);
            }
        }
        else
        {
            if (!statusEffectUIs.ContainsKey(statusEffectType))                    // Eðer bu durum efekti için bir UI yoksa
            {
                StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform);   // Yeni bir UI oluþtur
                statusEffectUIs.Add(statusEffectType, statusEffectUI);                          // Dictionary'ye ekle
            }
            Sprite sprite = GetSpriteByType(statusEffectType);                              // Durum efektine göre sprite'ý al
            statusEffectUIs[statusEffectType].Set(sprite, stackCount);                      // UI'ya sprite ve yýðýn sayýsýný ayarla
        }
    }

    private Sprite GetSpriteByType(StatusEffectType statusEffectType)
    {   // Durum efektine göre sprite'ý döndür
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.BURN => burnSprite,
            _ => null
        };
    }
}