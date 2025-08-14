using System.Collections.Generic;

// Bu class, bir d�v�� sisteminde kahraman i�in bir hedef modu tan�mlar ve eylemlerin do�rudan kahraman� hedeflemesine olanak tan�r.
public class HeroTM : TargetMode
{
    public override List<CombatantView> GetTargets()
    {
        List<CombatantView> targets = new List<CombatantView>() 
        {
            HeroSystem.Instance.HeroView
        };
        return targets;
    }
}
