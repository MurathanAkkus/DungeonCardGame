public class ApplyBurnGA : GameAction
{
    public CombatantView Target { get; private set; }
    public ApplyBurnGA(CombatantView target) { Target = target; }
}