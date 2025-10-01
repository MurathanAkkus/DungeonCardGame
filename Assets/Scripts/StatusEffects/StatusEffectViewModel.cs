public struct StatusEffectViewModel
{
    public StatusEffectType Type;
    public int Stacks;
    public int Magnitude;
    public int Duration; // -1 -> ∞, 0 -> kaldırılmalı

    public StatusEffectViewModel(StatusEffectType type, int stacks, int magnitude, int duration)
    {
        Type = type; Stacks = stacks; Magnitude = magnitude; Duration = duration;
    }
}