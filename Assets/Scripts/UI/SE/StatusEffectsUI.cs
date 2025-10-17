using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusEffectsUI : MonoBehaviour
{
    [SerializeField] private StatusEffectUI statusEffectUIPrefab;
    [SerializeField] private StatusEffectRegistry registry;     // yalnızca Inspector’dan atanacak

    public bool HasRegistry() => registry != null && statusEffectUIPrefab != null;

    private readonly Dictionary<StatusEffectType, StatusEffectUI> _uiByType = new();

    public void Upsert(StatusEffectViewModel vm)
    {
        if (!Application.isPlaying) 
            return; // Inspector’da değer değiştirirken Unity bazı objeleri “panel’e bağlı değilken” yeniden çizer; bu guard o akışı keser.

        // Süre/stacks 0 ise öğeyi kaldır
        if (vm.Duration == 0 || vm.Stacks == 0)
        {
            Remove(vm.Type);
            return;
        }

        // ---- NULL GUARDS ----
        if (registry == null)
        {
            Debug.LogError("[StatusEffectsUI] Registry assign edilmemiş!");
            return;
        }

        var desc = registry.Get(vm.Type);
        if (desc == null)
        {
            Debug.LogWarning($"[StatusEffectsUI] Descriptor bulunamadı: {vm.Type}");
            Remove(vm.Type); // güvenli
            return;
        }

        if (!_uiByType.TryGetValue(vm.Type, out var ui) || ui == null)
        {
            if (statusEffectUIPrefab == null)
            {
                Debug.LogError("[StatusEffectsUI] Prefab assign edilmemiş!");
                return;
            }
            ui = Instantiate(statusEffectUIPrefab, transform);
            _uiByType[vm.Type] = ui;
        }

        ui.Bind(desc, vm);   // Bind içinde de icon/label null guard olsun (aşağıda)
        ResortByPriority();
    }

    public void Remove(StatusEffectType type)
    {
        if (!Application.isPlaying) 
            return; // Inspector’da değer değiştirirken Unity bazı objeleri “panel’e bağlı değilken” yeniden çizer; bu guard o akışı keser.

        if (_uiByType.TryGetValue(type, out var ui) && ui != null)
            Destroy(ui.gameObject);
        _uiByType.Remove(type);
    }

    private void ResortByPriority()
    {
        var ordered = _uiByType
            .Select(kv => (kv.Value, Desc: registry?.Get(kv.Key)))
            .Where(x => x.Value != null && x.Desc != null)
            .OrderBy(x => x.Desc.Priority)
            .ToList();

        for (int i = 0; i < ordered.Count; i++)
            ordered[i].Value.transform.SetSiblingIndex(i);
    }

    public void ClearAll()
    {
        foreach (var kv in _uiByType) if (kv.Value) Destroy(kv.Value.gameObject);
        _uiByType.Clear();
    }

#if UNITY_EDITOR
    void OnValidate()  // Sahnede/Prefab’ta referansları unutursam editörde uyarı verir
    {   // Auto-wire (FindObjectOfType vs.): bu uyarılar referans unutulmasını anlamanı sağlar ve UI Toolkit hatalarını tetiklemez
        if (statusEffectUIPrefab == null)
            Debug.LogWarning("[StatusEffectsUI] 'Status Effect UI Prefab' atanmadı.", this);

        if (registry == null)
            Debug.LogWarning("[StatusEffectsUI] 'Registry' atanmadı (ScriptableObject asset bekleniyor).", this);
    }
#endif
}