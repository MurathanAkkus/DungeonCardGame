using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public void Set(StatusEffectType statusEffectType, Sprite sprite, int stackCount, Color color)

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
            stackCountText.text = GetFormattedStackText(statusEffectType, stackCount);

    private static string GetFormattedStackText(StatusEffectType statusEffectType, int stackCount)
    {
        if (statusEffectType == StatusEffectType.STRENGTH && stackCount > 0)
        {
            return $"+{stackCount}";
        }

        return stackCount.ToString();
    }
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text stackCountText;

    public void Set(Sprite sprite, int stackCount, Color color)
    {
        image.sprite = sprite;
        image.enabled = sprite != null;
        if (stackCount > 1)
        {
            stackCountText.text = stackCount.ToString();
            stackCountText.color = color;
            stackCountText.enabled = true;
        }
        else
        {
            stackCountText.enabled = false;
        }
    }
}
