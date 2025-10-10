using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text label;

    public void Bind(StatusEffectDescriptor desc, StatusEffectViewModel vm)
    {
        if (icon != null) { icon.sprite = desc.Icon; icon.color = desc.Tint; }
        else Debug.LogWarning("[StatusEffectUI] icon assign edilmemiş!");

        if (label != null)
        {
            string typeName = desc.name;
            int stk = desc.ShowStacks ? vm.Stacks : 0;
            int mag = desc.ShowMagnitude ? vm.Magnitude : 0;
            int dur = desc.ShowDuration ? vm.Duration : -1;
            label.text = desc.BuildLabel(typeName, stk, mag, dur);
        }
        else Debug.LogWarning("[StatusEffectUI] label assign edilmemiş!");
    }
}