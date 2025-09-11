using System.Collections.Generic;

public class OtherTM : TargetMode
{
    public static CombatantView MainTarget;

    public override List<CombatantView> GetTargets()
    {
        List<CombatantView> targets = new List<CombatantView>(EnemySystem.Instance.Enemies);
        if (MainTarget != null)
        {
            targets.Remove(MainTarget);
        }
        return targets;
    }
}