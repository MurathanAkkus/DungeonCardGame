using UnityEngine;

[CreateAssetMenu(menuName = "CardGame/Status Effect Descriptor")]
public class StatusEffectDescriptor : ScriptableObject
{
    public StatusEffectType Type;

    [Header("Visuals")]
    public Sprite Icon;
    public Color Tint = Color.white;

    [Header("Label Format")]
    [Tooltip("Örn: 'Burn {mag} (⏳{dur})' veya 'Str +{mag}' veya 'x{stk}'")]
    public string LabelFormat = "{type} x{stk}";

    [Header("Show Toggles")]
    public bool ShowStacks = true;
    public bool ShowMagnitude = false;
    public bool ShowDuration = false;

    [Header("Sorting")]
    [Tooltip("Küçük rakam, daha üstte görünür")]
    public int Priority = 100;

    public string BuildLabel(string typeName, int stacks, int magnitude, int duration)
    {
        string durText = duration < 0 ? "∞" : duration.ToString();
        return LabelFormat
            .Replace("{type}", typeName)
            .Replace("{stk}", stacks.ToString())
            .Replace("{mag}", magnitude.ToString())
            .Replace("{dur}", durText);
    }
}