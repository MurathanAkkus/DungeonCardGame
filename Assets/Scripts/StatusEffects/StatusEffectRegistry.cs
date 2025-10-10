using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "StatusEffect/Status Effect Registry")]
public class StatusEffectRegistry : ScriptableObject
{
    [SerializeField] private List<StatusEffectDescriptor> descriptors = new();

    private Dictionary<StatusEffectType, StatusEffectDescriptor> _map;

    public StatusEffectDescriptor Get(StatusEffectType type)
    {
        if (_map == null)
        {
            _map = new Dictionary<StatusEffectType, StatusEffectDescriptor>();
            foreach (var d in descriptors)
                if (d != null) _map[d.Type] = d;
        }
        _map.TryGetValue(type, out var desc);
        return desc;
    }
}