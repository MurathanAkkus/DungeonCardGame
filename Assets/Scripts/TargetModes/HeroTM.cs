using System.Collections.Generic;

// Bu class, bir dövüþ sisteminde kahraman için bir hedef modu tanýmlar ve eylemlerin doðrudan kahramaný hedeflemesine olanak tanýr.
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
