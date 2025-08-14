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
        {   // E�er y���n say�s� 0 ise, bu durum efekti i�in UI'yi kald�r
            if (statusEffectUIs.ContainsKey(statusEffectType))
            {   // E�er bu durum efekti i�in bir UI varsa
                StatusEffectUI statusEffectUI = statusEffectUIs[statusEffectType]; // Bu durum efekti i�in mevcut UI'y� edir
                statusEffectUIs.Remove(statusEffectType);                          // Dictionary'den kald�r
                Destroy(statusEffectUI.gameObject);
            }
        }
        else
        {
            if (!statusEffectUIs.ContainsKey(statusEffectType))                    // E�er bu durum efekti i�in bir UI yoksa
            {
                StatusEffectUI statusEffectUI = Instantiate(statusEffectUIPrefab, transform);   // Yeni bir UI olu�tur
                statusEffectUIs.Add(statusEffectType, statusEffectUI);                          // Dictionary'ye ekle
            }
            Sprite sprite = GetSpriteByType(statusEffectType);                              // Durum efektine g�re sprite'� al
            statusEffectUIs[statusEffectType].Set(sprite, stackCount);                      // UI'ya sprite ve y���n say�s�n� ayarla
        }
    }

    private Sprite GetSpriteByType(StatusEffectType statusEffectType)
    {   // Durum efektine g�re sprite'� d�nd�r
        return statusEffectType switch
        {
            StatusEffectType.ARMOR => armorSprite,
            StatusEffectType.BURN => burnSprite,
            _ => null
        };
    }
}