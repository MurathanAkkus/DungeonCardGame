using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Tek bir durum efekti kullanýcý arayüzü öðesini temsil eder
public class StatusEffectUI : MonoBehaviour 
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;

    public void Set(StatusEffectType statusEffectType, Sprite sprite, int stackCount, Color color)
    {
        image.sprite = sprite;
        image.enabled = sprite != null;

        bool showStackCount = stackCount > 1;

        if (statusEffectType == StatusEffectType.STRENGTH)
        {
            showStackCount = stackCount != 0;
        }
        else if (stackCount < 0)
        {
            showStackCount = true;
        }

        if (showStackCount)
        {
            stackCountText.text = GetFormattedStackText(statusEffectType, stackCount);
            stackCountText.color = color;
            stackCountText.enabled = true;
        }
        else
        {
            stackCountText.enabled = false;
        }
    }
    private static string GetFormattedStackText(StatusEffectType statusEffectType, int stackCount)
    {
        if (statusEffectType == StatusEffectType.STRENGTH && stackCount > 0)
        {
            return $"+{stackCount}";
        }

        return stackCount.ToString();
    }
}