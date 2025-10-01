using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField] private StatusEffectRegistry registry;

    private readonly Dictionary<StatusEffectType, StatusEffectUI> _uiByType = new();

    public void Upsert(StatusEffectViewModel vm)
    {
        // Süre 0 veya stacks 0 ise UI’yı kaldır
        if (vm.Duration == 0 || vm.Stacks == 0)
        {
            Remove(vm.Type);
            return;
        }

        var desc = registry.Get(vm.Type);
        if (desc == null) { Remove(vm.Type); return; }

        if (!_uiByType.TryGetValue(vm.Type, out var ui) || ui == null)
        {
            ui = Instantiate(statusEffectUIPrefab, transform);
            _uiByType[vm.Type] = ui;
        }

        ui.Bind(desc, vm);
        ResortByPriority();
    }

    public void Remove(StatusEffectType type)
    {
        if (_uiByType.TryGetValue(type, out var ui) && ui != null)
        {
            Destroy(ui.gameObject);
        }
        _uiByType.Remove(type);
    }

    public void ClearAll()
    {
        foreach (var kv in _uiByType) if (kv.Value) Destroy(kv.Value.gameObject);
        _uiByType.Clear();
    }

    private void ResortByPriority()
    {
        // Çocukları priority’ye göre sırala
        var ordered = _uiByType
            .Select(kv => (kv.Value, Desc: registry.Get(kv.Key)))
            .Where(x => x.Value != null && x.Desc != null)
            .OrderBy(x => x.Desc.Priority)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].Value.transform.SetSiblingIndex(i);
        }
    }
}